using NutriTrackAI.Models;
namespace NutriTrackAI.Models
{
    public class MealPlan
    {
        public int MealPlanID { get; set; }

        public int UserID { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public User? User { get; set; }

        public ICollection<MealPlanRecipe>? MealPlanRecipes { get; set; }
        public ShoppingList? ShoppingList { get; set; }
    }
}