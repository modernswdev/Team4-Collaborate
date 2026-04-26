using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NutriTrackAI.Data;
using NutriTrackAI.Models;

namespace NutriTrackAI.Controllers
{
    public class PantryItemsController : Controller
    {

        private int? GetUserId()
        {
            return HttpContext.Session.GetInt32("UserID");
        }

        private readonly NutriTrackContext _context;

        public PantryItemsController(NutriTrackContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var pantryItems = await _context.PantryItems
                .Where(p => p.UserID == userId.Value)
                .Include(p => p.Ingredient)
                .Include(p => p.Unit)
                .ToListAsync();

            return View(pantryItems);
        }

        // GET: PantryItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pantryItem = await _context.PantryItems
                .Include(p => p.Ingredient)
                .Include(p => p.Unit)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PantryItemID == id);
            if (pantryItem == null)
            {
                return NotFound();
            }

            return View(pantryItem);
        }

        // GET: PantryItems/Create
        public IActionResult Create()
        {
            ViewData["IngredientID"] = new SelectList(_context.Ingredients, "IngredientID", "IngredientName");
            ViewData["UnitID"] = new SelectList(_context.Units, "UnitID", "UnitName");
            ViewData["UserID"] = new SelectList(_context.Users, "UserID", "Email");
            return View();
        }

        // POST: PantryItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PantryItem pantryItem)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            pantryItem.UserID = userId.Value;

            if (!ModelState.IsValid)
            {
                ViewBag.Ingredients = new SelectList(_context.Ingredients, "IngredientID", "IngredientName");
                ViewBag.Units = new SelectList(_context.Units, "UnitID", "UnitName");
                return View(pantryItem);
            }

            _context.PantryItems.Add(pantryItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var pantryItem = await _context.PantryItems
                .FirstOrDefaultAsync(p => p.PantryItemID == id && p.UserID == userId.Value);

            if (pantryItem == null)
            {
                return NotFound();
            }

            ViewBag.Ingredients = new SelectList(_context.Ingredients, "IngredientID", "IngredientName");
            ViewBag.Units = new SelectList(_context.Units, "UnitID", "UnitName");

            return View(pantryItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PantryItem pantryItem)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != pantryItem.PantryItemID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Ingredients = new SelectList(_context.Ingredients, "IngredientID", "IngredientName");
                ViewBag.Units = new SelectList(_context.Units, "UnitID", "UnitName");
                return View(pantryItem);
            }

            var existingItem = await _context.PantryItems
                .FirstOrDefaultAsync(p => p.PantryItemID == id && p.UserID == userId.Value);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.IngredientID = pantryItem.IngredientID;
            existingItem.Quantity = pantryItem.Quantity;
            existingItem.UnitID = pantryItem.UnitID;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var pantryItem = await _context.PantryItems
                .Include(p => p.Ingredient)
                .Include(p => p.Unit)
                .FirstOrDefaultAsync(p => p.PantryItemID == id && p.UserID == userId.Value);

            if (pantryItem == null)
            {
                return NotFound();
            }

            return View(pantryItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var pantryItem = await _context.PantryItems
                .FirstOrDefaultAsync(p => p.PantryItemID == id && p.UserID == userId.Value);

            if (pantryItem == null)
            {
                return NotFound();
            }

            _context.PantryItems.Remove(pantryItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
