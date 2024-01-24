namespace RecipeBooks.Models
{
    public class BooksModel
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? BookSubTitle { get; set; }
        public string? FullTitle => BookTitle.Trim() + " " + BookSubTitle.Trim();
        public string? Publisher { get; set; }
        public string? Author { get; set; }
        public int Year { get; set; }
        public int LocationFK { get; set; }
        public int Copies { get; set; }
        public int Cuisine { get; set; }
        public int MediaType { get; set; }
        public int RecipeRef { get; set; }

        // ************************************************
        public override string ToString()
        {
            string output = " BookId: " + BookId.ToString()
                + ", Title: " + BookTitle
                + ", SubTitle: " + BookSubTitle
                + ", Publisher: " + Publisher
                + ", Author: " + Author
                + ", Year: " + Year.ToString()
                + ", Location: " + LocationFK.ToString()
                + ", Copies: " + Copies.ToString()
                + ", eCuisine: " + Cuisine.ToString()
                + ", eMediaType: " + Cuisine.ToString()
                + ", RecipeRef: " + RecipeRef.ToString();

            return output;
        }

        // ************************************************
        public string ListBoxText()
        {
            return $"{FullTitle} [{SQLiteDataAccess.CountRecipes(BookId)}] by {Author}";
        }

        // ************************************************
        public override bool Equals(object obj)
        {
            if (!(obj is BooksModel))
                return false;

            var other = obj as BooksModel;

            if (BookTitle != other.BookTitle)
                return false;

            if (BookSubTitle != other.BookSubTitle)
                return false;

            if (FullTitle != other.FullTitle)
                return false;

            if (Publisher != other.Publisher)
                return false;

            if (Author != other.Author)
                return false;

            if (Year != other.Year)
                return false;

            if (LocationFK != other.LocationFK)
                return false;

            if (Cuisine != other.Cuisine)
                return false;

            return true;
        }

        // ************************************************
    }
}