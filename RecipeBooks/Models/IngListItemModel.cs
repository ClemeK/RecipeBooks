namespace RecipeBooks.Models
{
    public class IngListItemModel
    {
        public int RecipeFK { get; set; }
        public int IngredientFK { get; set; }
        public float Quantity { get; set; }
        public int MType { get; set; }

        // ************************************************
        public override string ToString()
        {
            RecipesModel rcp = new();
            rcp = SQLiteDataAccess.GetRecipe(RecipeFK)[0];

            IngredientsModel ing = new();
            ing = SQLiteDataAccess.GetIngredient(IngredientFK)[0];


            string output = " RecipeFK: " + rcp.RecipeTitle 
                + " IngredientFK: " + ing.IngredientName
                + ", Quantity: " + Quantity.ToString()
                + ", MType: " + MType.ToString()
                ;

            return output;
        }
    }
}