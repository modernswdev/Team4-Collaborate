using NutriTrackAI.Models;
namespace NutriTrackAI.Models

{
    public class RecipeRestriction
    {
        public int RecipeID { get; set; }
        public int DietaryRestrictionID { get; set; }

        public Recipe? Recipe { get; set; }
        public DietaryRestriction? DietaryRestriction { get; set; }
    }
}