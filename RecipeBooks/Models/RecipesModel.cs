namespace RecipeBooks.Models
{
    public class RecipesModel
    {
        public int RecipeId { get; set; }
        public int BookFk { get; set; }
        public string? RecipeTitle { get; set; }
        public int ChapterFk { get; set; }
        public int Referance { get; set; }
        public int Makes { get; set; }
        public int Serving { get; set; }
        public float Quantity { get; set; }
        public int QtyType { get; set; }
        public string? Preparation { get; set; }
        public string? Cooking { get; set; }
        public int Cuisine { get; set; }
        public bool Favourite { get; set; }
        public string? Image { get; set; }

        // ************************************************
        public override string ToString()
        {
            string output = $" RecipeId: {RecipeId}"
                + $", BookFk: {BookFk}"
                + $", RecipeTitle: {RecipeTitle}"
                + $", ChapterFk: {ChapterFk}"
                + $", Referance:{Referance}"
                + $", Makes: {Makes}"
                + $", Servings: {Serving}"
                + $", Quantity: {Quantity}"
                + $", QtyType: {QtyType}"
                + $", Preparation: {Preparation}"
                + $", Cooking: {Cooking}"
                + $", Cuisine: {Cuisine}"
                + $", Favourite: {Favourite}"
                + $", Image: {Image}";

            return output;
        }

        // ************************************************
        public override bool Equals(object obj)
        {
            if (!(obj is RecipesModel))
                return false;

            var other = obj as RecipesModel;

            if (BookFk != other.BookFk)
                return false;

            if (RecipeTitle != other.RecipeTitle)
                return false;

            return true;
        }
    }
}