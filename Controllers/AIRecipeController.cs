using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using NutriTrackAI.Data;
using NutriTrackAI.Models;

//Useless in this current model with no open.ai key.
namespace NutriTrackAI.Controllers
{
    public class AIRecipesController : Controller
    {
        private readonly NutriTrackContext _context;

        public AIRecipesController(NutriTrackContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new AIRecipeRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(AIRecipeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", request);
            }

            string? apiKey = Environment.GetEnvironmentVariable("MSD");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ViewBag.Error = "OPENAI_API_KEY is missing. Add it as an environment variable and restart Visual Studio.";
                return View("Index", request);
            }

            ChatClient client = new ChatClient(model: "gpt-5.1", apiKey: apiKey);

            string prompt = $@"
Generate {request.NumberOfRecipes} recipes.

Dietary preferences: {request.DietaryPreferences}
Allergies to avoid: {request.Allergies}
Meal type: {request.MealType}

Return ONLY valid JSON.
No markdown.
No explanation.

Use this exact format:
[
  {{
    ""recipeName"": ""string"",
    ""instructions"": ""string"",
    ""calories"": 0,
    ""protein"": 0,
    ""carbs"": 0,
    ""fat"": 0,
    ""ingredients"": [
      {{
        ""name"": ""string"",
        ""quantity"": 0,
        ""unit"": ""string""
      }}
    ]
  }}
]";

            ChatCompletion completion = await client.CompleteChatAsync(prompt);

            string json = completion.Content[0].Text;

            List<AIRecipeResult>? recipes;

            try
            {
                recipes = JsonSerializer.Deserialize<List<AIRecipeResult>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                ViewBag.Error = "AI response was not valid JSON. Try generating again.";
                ViewBag.RawResponse = json;
                return View("Index", request);
            }

            if (recipes == null || recipes.Count == 0)
            {
                ViewBag.Error = "No recipes were generated.";
                return View("Index", request);
            }

            foreach (var aiRecipe in recipes)
            {
                var recipe = new Recipe
                {
                    RecipeName = aiRecipe.RecipeName,
                    Instructions = aiRecipe.Instructions,
                    SourceURL = "AI Generated",
                    RecipeNutrition = new RecipeNutrition
                    {
                        Calories = aiRecipe.Calories,
                        Protein = aiRecipe.Protein,
                        Carbs = aiRecipe.Carbs,
                        Fat = aiRecipe.Fat
                    }
                };

                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();

                foreach (var aiIngredient in aiRecipe.Ingredients)
                {
                    var ingredient = await _context.Ingredients
                        .FirstOrDefaultAsync(i => i.IngredientName.ToLower() == aiIngredient.Name.ToLower());

                    if (ingredient == null)
                    {
                        ingredient = new Ingredient
                        {
                            IngredientName = aiIngredient.Name
                        };

                        _context.Ingredients.Add(ingredient);
                        await _context.SaveChangesAsync();
                    }

                    var unit = await _context.Units
                        .FirstOrDefaultAsync(u => u.UnitName.ToLower() == aiIngredient.Unit.ToLower());

                    if (unit == null)
                    {
                        unit = new Unit
                        {
                            UnitName = aiIngredient.Unit
                        };

                        _context.Units.Add(unit);
                        await _context.SaveChangesAsync();
                    }

                    _context.RecipeIngredients.Add(new RecipeIngredient
                    {
                        RecipeID = recipe.RecipeID,
                        IngredientID = ingredient.IngredientID,
                        Quantity = aiIngredient.Quantity,
                        UnitID = unit.UnitID
                    });
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Recipes");
        }
    }

    public class AIRecipeResult
    {
        public string RecipeName { get; set; }
        public string Instructions { get; set; }
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Carbs { get; set; }
        public decimal Fat { get; set; }
        public List<AIIngredientResult> Ingredients { get; set; } = new();
    }

    public class AIIngredientResult
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
}