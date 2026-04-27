using System.ComponentModel.DataAnnotations;
namespace NutriTrackAI.Models

{
    public class Unit
    {
        public int UnitID { get; set; }

        [Required]
        public string UnitName { get; set; }

        public ICollection<PantryItem>? PantryItems { get; set; }
        public ICollection<RecipeIngredient>? RecipeIngredients { get; set; }
        public ICollection<ShoppingListItem>? ShoppingListItems { get; set; }
    }
}
