using System.ComponentModel.DataAnnotations;

namespace NutriTrackAI.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public ICollection<UserDietaryRestriction>? UserDietaryRestrictions { get; set; }
        public ICollection<PantryItem>? PantryItems { get; set; }
        public ICollection<MealPlan>? MealPlans { get; set; }

        public ICollection<Recipe>? Recipes { get; set; }
    }
}