using System.ComponentModel.DataAnnotations;

namespace NutriTrackAI.Models
{
    public class AIRecipeRequest
    {
        [Required]
        public string DietaryPreferences { get; set; }

        public string? Allergies { get; set; }

        public string? MealType { get; set; }

        public int NumberOfRecipes { get; set; } = 3;
    }
}