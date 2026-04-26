namespace NutriTrackAI.Models
{
    public class MealPlanRecipe
    {
        public int MealPlanRecipeID { get; set; }
        public int MealPlanID { get; set; }
        public int RecipeID { get; set; }
        public int MealTypeID { get; set; }
        public DateTime MealDate { get; set; }

        public MealPlan? MealPlan { get; set; }
        public Recipe? Recipe { get; set; }
        public MealType? MealType { get; set; }
    }
}