using System.ComponentModel.DataAnnotations;

namespace NutriTrackAI.Models.ViewModels
{
    public class RecipeFormViewModel
    {
        public int RecipeID { get; set; }

        [Required]
        public string RecipeName { get; set; }

        [Required]
        public string Instructions { get; set; }

        public string? SourceURL { get; set; }

        public decimal? Calories { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Carbs { get; set; }
        public decimal? Fat { get; set; }
        public List<RecipeIngredientViewModel> Ingredients { get; set; } = new();
    }
}