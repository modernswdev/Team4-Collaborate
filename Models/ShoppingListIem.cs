using NutriTrackAI.Models;
namespace NutriTrackAI.Models

{
    public class ShoppingListItem
    {
        public int ShoppingListItemID { get; set; }

        public int ShoppingListID { get; set; }
        public int IngredientID { get; set; }
        public int? UnitID { get; set; }

        public decimal? Quantity { get; set; }

        public bool IsCheckedOff { get; set; } = false;

        public ShoppingList? ShoppingList { get; set; }
        public Ingredient? Ingredient { get; set; }
        public Unit? Unit { get; set; }
    }
}