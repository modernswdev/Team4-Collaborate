using NutriTrackAI.Models;
namespace NutriTrackAI.Models

{
    public class ShoppingList
    {
        public int ShoppingListID { get; set; }

        public int MealPlanID { get; set; }

        public MealPlan? MealPlan { get; set; }

        public ICollection<ShoppingListItem>? ShoppingListItems { get; set; }
    }
}