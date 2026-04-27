using System.ComponentModel.DataAnnotations.Schema;

namespace NutriTrackAI.Models

{
    public class UserDietaryRestriction
    {
        public int UserID { get; set; }
        public int DietaryRestrictionID { get; set; }

        public User? User { get; set; }
        public DietaryRestriction? DietaryRestriction { get; set; }
    }
}