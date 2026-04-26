using Microsoft.EntityFrameworkCore;
using NutriTrackAI.Models;

namespace NutriTrackAI.Data

{
    public class NutriTrackContext : DbContext
    {
        public NutriTrackContext(DbContextOptions<NutriTrackContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<DietaryRestriction> DietaryRestrictions { get; set; }
        public DbSet<UserDietaryRestriction> UserDietaryRestrictions { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<PantryItem> PantryItems { get; set; }
        public DbSet<MealType> MealTypes { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<RecipeNutrition> RecipeNutritions { get; set; }
        public DbSet<RecipeRestriction> RecipeRestrictions { get; set; }
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<MealPlanRecipe> MealPlanRecipes { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite primary key for UserDietaryRestriction
            modelBuilder.Entity<UserDietaryRestriction>()
                .HasKey(udr => new { udr.UserID, udr.DietaryRestrictionID });

            // Composite primary key for RecipeRestriction
            modelBuilder.Entity<RecipeRestriction>()
                .HasKey(rr => new { rr.RecipeID, rr.DietaryRestrictionID });

            // RecipeNutrition uses RecipeID as both PK and FK
            modelBuilder.Entity<RecipeNutrition>()
                .HasKey(rn => rn.RecipeID);

            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.RecipeNutrition)
                .WithOne(rn => rn.Recipe)
                .HasForeignKey<RecipeNutrition>(rn => rn.RecipeID);

            // ShoppingList is one-to-one with MealPlan
            modelBuilder.Entity<MealPlan>()
                .HasOne(mp => mp.ShoppingList)
                .WithOne(sl => sl.MealPlan)
                .HasForeignKey<ShoppingList>(sl => sl.MealPlanID);

            // Prevent duplicate pantry items for same user
            modelBuilder.Entity<PantryItem>()
                .HasIndex(p => new { p.UserID, p.IngredientID, p.UnitID })
                .IsUnique();

            // Prevent duplicate ingredients in same recipe
            modelBuilder.Entity<RecipeIngredient>()
                .HasIndex(ri => new { ri.RecipeID, ri.IngredientID, ri.UnitID })
                .IsUnique();

            // Prevent duplicate shopping list items
            modelBuilder.Entity<ShoppingListItem>()
                .HasIndex(sli => new { sli.ShoppingListID, sli.IngredientID, sli.UnitID })
                .IsUnique();

            // Prevent duplicate meal type on the same day
            modelBuilder.Entity<MealPlanRecipe>()
                .HasIndex(mpr => new { mpr.MealPlanID, mpr.MealDate, mpr.MealTypeID })
                .IsUnique();

            // Optional: unique names
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<DietaryRestriction>()
                .HasIndex(dr => dr.RestrictionName)
                .IsUnique();

            modelBuilder.Entity<Ingredient>()
                .HasIndex(i => i.IngredientName)
                .IsUnique();

            modelBuilder.Entity<Unit>()
                .HasIndex(u => u.UnitName)
                .IsUnique();

            modelBuilder.Entity<MealType>()
                .HasIndex(mt => mt.MealTypeName)
                .IsUnique();

            modelBuilder.Entity<RecipeNutrition>()
                .ToTable("RecipeNutrition");

            // Seed starter dietary restrictions
            modelBuilder.Entity<DietaryRestriction>().HasData(
                new DietaryRestriction { DietaryRestrictionID = 1, RestrictionName = "Vegan" },
                new DietaryRestriction { DietaryRestrictionID = 2, RestrictionName = "Keto" },
                new DietaryRestriction { DietaryRestrictionID = 3, RestrictionName = "High Protein" },
                new DietaryRestriction { DietaryRestrictionID = 4, RestrictionName = "Gluten-Free" },
                new DietaryRestriction { DietaryRestrictionID = 5, RestrictionName = "Dairy-Free" },
                new DietaryRestriction { DietaryRestrictionID = 6, RestrictionName = "Nut-Free" },
                new DietaryRestriction { DietaryRestrictionID = 7, RestrictionName = "Low Carb" }
            );

            // Seed starter units
            modelBuilder.Entity<Unit>().HasData(
                new Unit { UnitID = 1, UnitName = "grams" },
                new Unit { UnitID = 2, UnitName = "cups" },
                new Unit { UnitID = 3, UnitName = "tablespoons" },
                new Unit { UnitID = 4, UnitName = "teaspoons" },
                new Unit { UnitID = 5, UnitName = "ounces" },
                new Unit { UnitID = 6, UnitName = "pieces" }
            );

            // Seed meal types
            modelBuilder.Entity<MealType>().HasData(
                new MealType { MealTypeID = 1, MealTypeName = "Breakfast" },
                new MealType { MealTypeID = 2, MealTypeName = "Lunch" },
                new MealType { MealTypeID = 3, MealTypeName = "Dinner" },
                new MealType { MealTypeID = 4, MealTypeName = "Snack" }
            );
        }
    }
}