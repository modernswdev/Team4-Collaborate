using System.ComponentModel.DataAnnotations;
namespace NutriTrackAI.Models

{
    public class Ingredient
    {
        public int IngredientID { get; set; }

        [Required]
        public string IngredientName { get; set; }

        public ICollection<PantryItem>? PantryItems { get; set; }
        public ICollection<RecipeIngredient>? RecipeIngredients { get; set; }
        public ICollection<ShoppingListItem>? ShoppingListItems { get; set; }
    }
}
