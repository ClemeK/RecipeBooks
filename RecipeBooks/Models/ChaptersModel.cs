namespace RecipeBooks.Models
{
    public class ChaptersModel
    {
        public int ChapterId { get; set; }
        public string? ChapterName { get; set; }

        // ************************************************
        public override string ToString()
        {
            string output = " ChapterId: " + ChapterId.ToString()
                + ", ChapterName: " + ChapterName;

            return output;
        }

        // ************************************************
        public string ListBoxText()
        {
            return $"{ChapterName.Trim()}";
        }

        // ************************************************
        public override bool Equals(object obj)
        {
            if (!(obj is ChaptersModel))
                return false;

            var other = obj as ChaptersModel;

            if (ChapterName != other.ChapterName)
                return false;

            return true;
        }

        // ************************************************
    }
}