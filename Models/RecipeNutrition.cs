namespace NutriTrackAI.Models

{
    public class RecipeNutrition
    {
        public int RecipeID { get; set; }

        public decimal? Calories { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Carbs { get; set; }
        public decimal? Fat { get; set; }

        public Recipe? Recipe { get; set; }
    }
}