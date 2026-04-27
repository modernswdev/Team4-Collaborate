
using System.ComponentModel.DataAnnotations;
namespace NutriTrackAI.Models

{
    public class DietaryRestriction
    {
        public int DietaryRestrictionID { get; set; }

        [Required]
        public string RestrictionName { get; set; }

        public ICollection<UserDietaryRestriction>? UserDietaryRestrictions { get; set; }
        public ICollection<RecipeRestriction>? RecipeRestrictions { get; set; }
    }
}