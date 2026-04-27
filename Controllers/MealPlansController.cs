using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NutriTrackAI.Data;
using NutriTrackAI.Models;


namespace NutriTrackAI.Models
{
    //handles meal planning, weekly shopping list gen, nutrition totals, and deleting meals from the plan
    public class MealPlansController : Controller
    {
        //gets the user's ID from session
        private int? GetUserId()
        {
            return HttpContext.Session.GetInt32("UserID");
        }

        //finds the meal plan and makes it if it doesn't yet exist
        private async Task<MealPlan> GetOrCreateMealPlan(int userId)
        {
            var mealPlan = await _context.MealPlans
                .FirstOrDefaultAsync(mp => mp.UserID == userId);

            if (mealPlan == null)
            {
                mealPlan = new MealPlan
                {
                    UserID = userId,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(7)
                };

                _context.MealPlans.Add(mealPlan);
                await _context.SaveChangesAsync();
            }

            return mealPlan;
        }

        private readonly NutriTrackContext _context;

        public MealPlansController(NutriTrackContext context)
        {
            _context = context;
        }

        //shows all logged in user's meal plan
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var mealPlan = await GetOrCreateMealPlan(userId.Value);

            var meals = await _context.MealPlanRecipes
                .Where(m => m.MealPlanID == mealPlan.MealPlanID)
                .Include(m => m.Recipe)
                .Include(m => m.MealType)
                .ToListAsync();

            return View(meals);
        }

        //shows the form to add a recipe to the meal plan
        public IActionResult Create()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Recipes = new SelectList(
                _context.Recipes.Where(r => r.UserID == userId.Value),
                "RecipeID",
                "RecipeName"
            );

            ViewBag.MealTypes = new SelectList(_context.MealTypes, "MealTypeID", "MealTypeName");

            return View();
        }

        //adds recipe to the meal plan for a specified date and meal type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MealPlanRecipe model)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (model.MealDate == default)
            {
                ModelState.AddModelError("MealDate", "Please select a valid date.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Recipes = new SelectList(
                    _context.Recipes.Where(r => r.UserID == userId.Value),
                    "RecipeID",
                    "RecipeName"
                );

                ViewBag.MealTypes = new SelectList(_context.MealTypes, "MealTypeID", "MealTypeName");

                return View(model);
            }

            var mealPlan = await GetOrCreateMealPlan(userId.Value);

            model.MealPlanID = mealPlan.MealPlanID;

            _context.MealPlanRecipes.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //makes shopping list by combining all ingredients from the meal plan and subtracts the pantry quanities and then creates the list
        public async Task<IActionResult> GenerateWeeklyShoppingList()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var mealPlan = await GetOrCreateMealPlan(userId.Value);

            var mealPlanRecipes = await _context.MealPlanRecipes
                .Where(mpr => mpr.MealPlanID == mealPlan.MealPlanID)
                .Include(mpr => mpr.Recipe)
                    .ThenInclude(r => r.RecipeIngredients)
                .ToListAsync();

            var pantryItems = await _context.PantryItems
                .Where(p => p.UserID == userId.Value)
                .ToListAsync();

            var shoppingList = new ShoppingList
            {
                MealPlanID = mealPlan.MealPlanID
            };

            _context.ShoppingLists.Add(shoppingList);
            await _context.SaveChangesAsync();

            var combinedIngredients = mealPlanRecipes
                .SelectMany(mpr => mpr.Recipe.RecipeIngredients)
                .GroupBy(ri => new { ri.IngredientID, ri.UnitID })
                .Select(g => new
                {
                    IngredientID = g.Key.IngredientID,
                    UnitID = g.Key.UnitID,
                    TotalQuantityNeeded = g.Sum(x => x.Quantity ?? 0)
                })
                .ToList();

            foreach (var item in combinedIngredients)
            {
                var pantryItem = pantryItems.FirstOrDefault(p =>
                    p.IngredientID == item.IngredientID &&
                    p.UnitID == item.UnitID);

                decimal pantryQuantity = pantryItem?.Quantity ?? 0;
                decimal quantityToBuy = item.TotalQuantityNeeded - pantryQuantity;

                if (quantityToBuy > 0)
                {
                    _context.ShoppingListItems.Add(new ShoppingListItem
                    {
                        ShoppingListID = shoppingList.ShoppingListID,
                        IngredientID = item.IngredientID,
                        UnitID = item.UnitID,
                        Quantity = quantityToBuy,
                        IsCheckedOff = false
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "ShoppingLists", new { id = shoppingList.ShoppingListID });
        }

        //calculates total calories, protein, carbs, and fat for the week
        public async Task<IActionResult> WeeklyNutrition()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var mealPlan = await GetOrCreateMealPlan(userId.Value);

            var meals = await _context.MealPlanRecipes
                .Where(m => m.MealPlanID == mealPlan.MealPlanID)
                .Include(m => m.Recipe)
                    .ThenInclude(r => r.RecipeNutrition)
                .ToListAsync();

            var totals = new
            {
                Calories = meals.Sum(m => m.Recipe.RecipeNutrition?.Calories ?? 0),
                Protein = meals.Sum(m => m.Recipe.RecipeNutrition?.Protein ?? 0),
                Carbs = meals.Sum(m => m.Recipe.RecipeNutrition?.Carbs ?? 0),
                Fat = meals.Sum(m => m.Recipe.RecipeNutrition?.Fat ?? 0)
            };

            return View(totals);
        }

        //shows deletion confirmation
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var meal = await _context.MealPlanRecipes
                .Include(m => m.Recipe)
                .Include(m => m.MealType)
                .Include(m => m.MealPlan)
                .FirstOrDefaultAsync(m =>
                    m.MealPlanRecipeID == id &&
                    m.MealPlan.UserID == userId.Value);

            if (meal == null)
            {
                return NotFound();
            }

            return View(meal);
        }

        //deletes a meal from user's meal plan
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var meal = await _context.MealPlanRecipes
                .Include(m => m.MealPlan)
                .FirstOrDefaultAsync(m =>
                    m.MealPlanRecipeID == id &&
                    m.MealPlan.UserID == userId.Value);

            if (meal == null)
            {
                return NotFound();
            }

            _context.MealPlanRecipes.Remove(meal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}