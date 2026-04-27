using System.ComponentModel.DataAnnotations;

namespace NutriTrackAI.Models
{
    public class MealPlanRecipeFormViewModel
    {
        [Required]
        public int RecipeID { get; set; }

        [Required]
        public int MealTypeID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime MealDate { get; set; }
    }
}