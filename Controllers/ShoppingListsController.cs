using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriTrackAI.Data;

namespace NutriTrackAI.Controllers
{
    //displays shopping list
    public class ShoppingListsController : Controller
    {
        private readonly NutriTrackContext _context;

        public ShoppingListsController(NutriTrackContext context)
        {
            _context = context;
        }

        //Shows a shopping list with ingredients and units
        public async Task<IActionResult> Details(int id)
        {
            var shoppingList = await _context.ShoppingLists
                .Include(sl => sl.ShoppingListItems)
                    .ThenInclude(sli => sli.Ingredient)
                .Include(sl => sl.ShoppingListItems)
                    .ThenInclude(sli => sli.Unit)
                .FirstOrDefaultAsync(sl => sl.ShoppingListID == id);

            if (shoppingList == null)
            {
                return NotFound();
            }

            return View(shoppingList);
        }
    }
}