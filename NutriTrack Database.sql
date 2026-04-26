CREATE DATABASE NutriTrackDB;
USE NutriTrackDB;

CREATE TABLE Users (
    UserID INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(150) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL
);

CREATE TABLE DietaryRestrictions (
    RestrictionID INT PRIMARY KEY AUTO_INCREMENT,
    RestrictionName VARCHAR(100) UNIQUE NOT NULL
);

INSERT INTO DietaryRestrictions (RestrictionName)
VALUES 
('Vegan'),
('Keto'),
('High Protein'),
('Gluten-Free'),
('Dairy-Free'),
('Nut-Free'),
('Low Carb');

CREATE TABLE UserDietaryRestrictions (
    UserID INT NOT NULL,
    RestrictionID INT NOT NULL,

    PRIMARY KEY (UserID, RestrictionID),

    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (RestrictionID) REFERENCES DietaryRestrictions(RestrictionID)
);

CREATE TABLE Ingredients (
    IngredientID INT PRIMARY KEY AUTO_INCREMENT,
    IngredientName VARCHAR(150) UNIQUE NOT NULL
);

CREATE TABLE Units (
    UnitID INT PRIMARY KEY AUTO_INCREMENT,
    UnitName VARCHAR(50) UNIQUE NOT NULL
);

INSERT INTO Units (UnitName)
VALUES
('grams'),
('cups'),
('tablespoons'),
('teaspoons'),
('ounces'),
('pieces');

CREATE TABLE PantryItems (
    PantryItemID INT PRIMARY KEY AUTO_INCREMENT,
    UserID INT NOT NULL,
    IngredientID INT NOT NULL,
    Quantity DECIMAL(8,2),
    UnitID INT,

    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (IngredientID) REFERENCES Ingredients(IngredientID),
    FOREIGN KEY (UnitID) REFERENCES Units(UnitID),

    UNIQUE (UserID, IngredientID, UnitID)
);

CREATE TABLE MealTypes (
    MealTypeID INT PRIMARY KEY AUTO_INCREMENT,
    MealTypeName VARCHAR(50) UNIQUE NOT NULL
);

INSERT INTO MealTypes (MealTypeName)
VALUES
('Breakfast'),
('Lunch'),
('Dinner'),
('Snack');

CREATE TABLE Recipes (
    RecipeID INT PRIMARY KEY AUTO_INCREMENT,
    RecipeName VARCHAR(200) NOT NULL,
    Instructions TEXT NOT NULL,
    SourceURL VARCHAR(500)
);

CREATE TABLE RecipeIngredients (
    RecipeIngredientID INT PRIMARY KEY AUTO_INCREMENT,
    RecipeID INT NOT NULL,
    IngredientID INT NOT NULL,
    Quantity DECIMAL(8,2),
    UnitID INT,

    FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID),
    FOREIGN KEY (IngredientID) REFERENCES Ingredients(IngredientID),
    FOREIGN KEY (UnitID) REFERENCES Units(UnitID),

    UNIQUE (RecipeID, IngredientID, UnitID)
);

CREATE TABLE RecipeNutrition (
    RecipeID INT PRIMARY KEY,
    Calories DECIMAL(8,2),
    Protein DECIMAL(8,2),
    Carbs DECIMAL(8,2),
    Fat DECIMAL(8,2),

    FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID)
);

CREATE TABLE RecipeRestrictions (
    RecipeID INT NOT NULL,
    RestrictionID INT NOT NULL,

    PRIMARY KEY (RecipeID, RestrictionID),

    FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID),
    FOREIGN KEY (RestrictionID) REFERENCES DietaryRestrictions(RestrictionID)
);

CREATE TABLE MealPlans (
    MealPlanID INT PRIMARY KEY AUTO_INCREMENT,
    UserID INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,

    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE MealPlanRecipes (
    MealPlanRecipeID INT PRIMARY KEY AUTO_INCREMENT,
    MealPlanID INT NOT NULL,
    RecipeID INT NOT NULL,
    MealTypeID INT NOT NULL,
    MealDate DATE NOT NULL,

    FOREIGN KEY (MealPlanID) REFERENCES MealPlans(MealPlanID),
    FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID),
    FOREIGN KEY (MealTypeID) REFERENCES MealTypes(MealTypeID),

    UNIQUE (MealPlanID, MealDate, MealTypeID)
);

CREATE TABLE ShoppingLists (
    ShoppingListID INT PRIMARY KEY AUTO_INCREMENT,
    MealPlanID INT NOT NULL,

    FOREIGN KEY (MealPlanID) REFERENCES MealPlans(MealPlanID)
);

CREATE TABLE ShoppingListItems (
    ShoppingListItemID INT PRIMARY KEY AUTO_INCREMENT,
    ShoppingListID INT NOT NULL,
    IngredientID INT NOT NULL,
    Quantity DECIMAL(8,2),
    UnitID INT,
    IsCheckedOff BOOLEAN DEFAULT FALSE,

    FOREIGN KEY (ShoppingListID) REFERENCES ShoppingLists(ShoppingListID),
    FOREIGN KEY (IngredientID) REFERENCES Ingredients(IngredientID),
    FOREIGN KEY (UnitID) REFERENCES Units(UnitID),

    UNIQUE (ShoppingListID, IngredientID, UnitID)
);