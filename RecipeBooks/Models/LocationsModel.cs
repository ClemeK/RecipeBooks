namespace RecipeBooks.Models
{
    public class LocationsModel
    {
        public int LocationId { get; set; }
        public string? LocationTitle { get; set; }

        // ************************************************
        public override string ToString()
        {
            string output = " LocationId: " + LocationId.ToString()
                + ", Location: " + LocationTitle;

            return output;
        }

        // ************************************************
        public string ListBoxText()
        {
            return $"{LocationTitle.Trim()}";
        }

        // ************************************************

        public override bool Equals(object obj)
        {
            if (!(obj is LocationsModel))
                return false;

            var other = obj as LocationsModel;

            if (LocationTitle != other.LocationTitle)
                return false;

            return true;
        }

        // ************************************************
    }
}