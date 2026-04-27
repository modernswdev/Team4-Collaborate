using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NutriTrackAI.Data;
using NutriTrackAI.Models;
using NutriTrackAI.Models.ViewModels;

namespace NutriTrackAI.Controllers
{
    //handles crud operations for recipes
    public class RecipesController : Controller
    {
        private readonly NutriTrackContext _context;

        public RecipesController(NutriTrackContext context)
        {
            _context = context;
        }

        //displays all recipes
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            //if not logged, prompt users to log in
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //shows recipes belonging to user
            var recipes = await _context.Recipes
                .Where(r => r.UserID == userId.Value)
                .Include(r => r.RecipeNutrition)
                .ToListAsync();

            return View(recipes);
        }

        //details page with all the recipe details
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var recipe = await _context.Recipes
                .Where(r => r.UserID == userId.Value)
                .Include(r => r.RecipeNutrition)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Unit)
                .FirstOrDefaultAsync(r => r.RecipeID == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        //creating the recipe, nutrition record, and ingredient rows
        public IActionResult Create()
        {
            ViewBag.Ingredients = new SelectList(_context.Ingredients, "IngredientID", "IngredientName");
            ViewBag.Units = new SelectList(_context.Units, "UnitID", "UnitName");

            return View(new RecipeFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Ingredients = new SelectList(_context.Ingredients, "IngredientID", "IngredientName");
                ViewBag.Units = new SelectList(_context.Units, "UnitID", "UnitName");
                return View(model);
            }

  
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }


            var recipe = new Recipe
            {
                UserID = userId.Value,
                RecipeName = model.RecipeName,
                Instructions = model.Instructions,
                SourceURL = model.SourceURL,
                RecipeNutrition = new RecipeNutrition
                {
                    Calories = model.Calories,
                    Protein = model.Protein,
                    Carbs = model.Carbs,
                    Fat = model.Fat
                }
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            foreach (var item in model.Ingredients)
            {
                _context.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeID = recipe.RecipeID,
                    IngredientID = item.IngredientID,
                    Quantity = item.Quantity,
                    UnitID = item.UnitID
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //shows edit form
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var recipe = await _context.Recipes
                .Include(r => r.RecipeNutrition)
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(r => r.RecipeID == id && r.UserID == userId.Value);

            if (recipe == null)
            {
                return NotFound();
            }

            ViewBag.Ingredients = new SelectList(_context.Ingredients, "IngredientID", "IngredientName");
            ViewBag.Units = new SelectList(_context.Units, "UnitID", "UnitName");

            var model = new RecipeFormViewModel
            {
                RecipeID = recipe.RecipeID,
                RecipeName = recipe.RecipeName,
                Instructions = recipe.Instructions,
                SourceURL = recipe.SourceURL,
                Calories = recipe.RecipeNutrition?.Calories,
                Protein = recipe.RecipeNutrition?.Protein,
                Carbs = recipe.RecipeNutrition?.Carbs,
                Fat = recipe.RecipeNutrition?.Fat,

                Ingredients = recipe.RecipeIngredients.Select(ri => new RecipeIngredientViewModel
                {
                    IngredientID = ri.IngredientID,
                    Quantity = ri.Quantity,
                    UnitID = ri.UnitID
                }).ToList()
            };

            return View(model);
        }

        //updates recipe info, nutrition, and replaces old list
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RecipeFormViewModel model)
        {
            if (id != model.RecipeID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Ingredients = new SelectList(_context.Ingredients, "IngredientID", "IngredientName");
                ViewBag.Units = new SelectList(_context.Units, "UnitID", "UnitName");
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var recipe = await _context.Recipes
                .Include(r => r.RecipeNutrition)
                .FirstOrDefaultAsync(r => r.RecipeID == id && r.UserID == userId.Value);

            if (recipe == null)
            {
                return NotFound();
            }

            recipe.RecipeName = model.RecipeName;
            recipe.Instructions = model.Instructions;
            recipe.SourceURL = model.SourceURL;

            if (recipe.RecipeNutrition == null)
            {
                recipe.RecipeNutrition = new RecipeNutrition
                {
                    RecipeID = recipe.RecipeID
                };
            }

            recipe.RecipeNutrition.Calories = model.Calories;
            recipe.RecipeNutrition.Protein = model.Protein;
            recipe.RecipeNutrition.Carbs = model.Carbs;
            recipe.RecipeNutrition.Fat = model.Fat;

            var oldIngredients = _context.RecipeIngredients
                .Where(ri => ri.RecipeID == recipe.RecipeID);

            _context.RecipeIngredients.RemoveRange(oldIngredients);

            foreach (var item in model.Ingredients)
            {
                _context.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeID = recipe.RecipeID,
                    IngredientID = item.IngredientID,
                    Quantity = item.Quantity,
                    UnitID = item.UnitID
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //shows delete confirmation for recipes
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(r => r.RecipeID == id && r.UserID == userId.Value);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        //deletes recipes and dependant rows first because the database is 3nf and it won't let me delete them any other way
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var recipe = await _context.Recipes
                .Include(r => r.RecipeNutrition)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.RecipeRestrictions)
                .Include(r => r.MealPlanRecipes)
                .FirstOrDefaultAsync(r => r.RecipeID == id && r.UserID == userId.Value);

            if (recipe == null)
            {
                return NotFound();
            }

            if (recipe.MealPlanRecipes != null)
            {
                _context.MealPlanRecipes.RemoveRange(recipe.MealPlanRecipes);
            }

            if (recipe.RecipeIngredients != null)
            {
                _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
            }

            if (recipe.RecipeRestrictions != null)
            {
                _context.RecipeRestrictions.RemoveRange(recipe.RecipeRestrictions);
            }

            if (recipe.RecipeNutrition != null)
            {
                _context.RecipeNutritions.Remove(recipe.RecipeNutrition);
            }

            _context.Recipes.Remove(recipe);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //makes the shopping list by taking what the recipe calls for and subtracting what is in the pantry
        public async Task<IActionResult> GenerateShoppingList(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var recipe = await _context.Recipes
             .Where(r => r.UserID == userId.Value)
             .Include(r => r.RecipeIngredients)
             .FirstOrDefaultAsync(r => r.RecipeID == id);

            if (recipe == null)
            {
                return NotFound();
            }

            var pantryItems = await _context.PantryItems
                .Where(p => p.UserID == userId)
                .ToListAsync();

            var shoppingList = new ShoppingList
            {
                MealPlanID = 1
            };

            _context.ShoppingLists.Add(shoppingList);
            await _context.SaveChangesAsync();

            foreach (var ingredient in recipe.RecipeIngredients)
            {
                var pantryItem = pantryItems.FirstOrDefault(p =>
                    p.IngredientID == ingredient.IngredientID);

                decimal neededQty = ingredient.Quantity ?? 0;
                decimal pantryQty = pantryItem?.Quantity ?? 0;

                if (pantryQty < neededQty)
                {
                    decimal quantityToBuy = neededQty - pantryQty;

                    _context.ShoppingListItems.Add(new ShoppingListItem
                    {
                        ShoppingListID = shoppingList.ShoppingListID,
                        IngredientID = ingredient.IngredientID,
                        Quantity = quantityToBuy,
                        UnitID = ingredient.UnitID,
                        IsCheckedOff = false
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "ShoppingLists", new { id = shoppingList.ShoppingListID });
        }
    }
}