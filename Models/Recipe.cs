namespace NutriTrackAI.Models
{
    public class Recipe
    {
        public int RecipeID { get; set; }

        public int UserID { get; set; }

        public string RecipeName { get; set; }
        public string Instructions { get; set; }
        public string? SourceURL { get; set; }

        public User? User { get; set; }
        public RecipeNutrition? RecipeNutrition { get; set; }

        public ICollection<RecipeIngredient>? RecipeIngredients { get; set; }
        public ICollection<RecipeRestriction>? RecipeRestrictions { get; set; }
        public ICollection<MealPlanRecipe>? MealPlanRecipes { get; set; }
    }
}