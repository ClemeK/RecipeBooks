using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBooks.Models
{
    public class Note2sModel
    {
        public int Note2Id { get; set; }
        public int RecipeFK { get; set; }
        public int Note2Nbr { get; set; }
        public string? Note2 { get; set; }

        // ************************************************
        public override string ToString()
        {
            string output = " Note2Id: " + Note2Id.ToString()
                + ", RecipeFK: " + RecipeFK.ToString()
                + ", Note2Nbr: " + Note2Nbr.ToString()
                + ", Note2: " + Note2;

            return output;
        }
    }
}
