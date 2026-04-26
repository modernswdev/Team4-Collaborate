namespace NutriTrackAI.Models

{
    public class PantryItem
    {
        public int PantryItemID { get; set; }

        public int UserID { get; set; }
        public int IngredientID { get; set; }
        public int? UnitID { get; set; }

        public decimal? Quantity { get; set; }

        public User? User { get; set; }
        public Ingredient? Ingredient { get; set; }
        public Unit? Unit { get; set; }
    }
}