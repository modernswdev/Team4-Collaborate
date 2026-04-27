
using System.ComponentModel.DataAnnotations;
namespace NutriTrackAI.Models
{
    public class MealType
    {
        public int MealTypeID { get; set; }

        [Required]
        public string MealTypeName { get; set; }

        public ICollection<MealPlanRecipe>? MealPlanRecipes { get; set; }
    }
}
