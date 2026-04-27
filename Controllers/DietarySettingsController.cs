using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriTrackAI.Data;
using NutriTrackAI.Models;

namespace NutriTrackAI.Controllers
{
    //handles dietary restrictions for the user
    public class DietarySettingsController : Controller
    {
        private readonly NutriTrackContext _context;

        public DietarySettingsController(NutriTrackContext context)
        {
            _context = context;
        }

        //Shows all dietary restrictions and marks the user's selected ones
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var allRestrictions = await _context.DietaryRestrictions.ToListAsync();

            var selectedIds = await _context.UserDietaryRestrictions
                .Where(udr => udr.UserID == userId)
                .Select(udr => udr.DietaryRestrictionID)
                .ToListAsync();

            ViewBag.SelectedIds = selectedIds;

            return View(allRestrictions);
        }

        //Saves their restrictions based on the user
        [HttpPost]
        public async Task<IActionResult> Save(List<int> selectedRestrictions)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentRestrictions = _context.UserDietaryRestrictions
                .Where(udr => udr.UserID == userId);

            _context.UserDietaryRestrictions.RemoveRange(currentRestrictions);

            if (selectedRestrictions != null)
            {
                foreach (var restrictionId in selectedRestrictions)
                {
                    _context.UserDietaryRestrictions.Add(new UserDietaryRestriction
                    {
                        UserID = userId.Value,
                        DietaryRestrictionID = restrictionId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}