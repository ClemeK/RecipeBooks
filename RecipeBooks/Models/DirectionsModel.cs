using System.Net;
using System.Security.Policy;

namespace RecipeBooks.Models
{
    public class DirectionsModel
    {
        public int DirectId { get; set; }
        public int RecipeFK { get; set; }
        public int Position { get; set; }
        public string? Direction { get; set; }

        // ************************************************
        public override string ToString()
        {
            string output = " DirectId: " + DirectId.ToString()
                + ", RecipeFK: " + RecipeFK.ToString()
                + ", Position: " + Position.ToString()
                + ", Direction: " + Direction;

            return output;
        }
    }
}