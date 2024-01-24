using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBooks.Models
{
    public class Note1sModel
    {
        public int Note1Id { get; set; }
        public int RecipeFK { get; set; }
        public int Note1Nbr { get; set; }
        public string? Note1 { get; set; }

        // ************************************************
        public override string ToString()
        {
            string output = " Note1Id: " + Note1Id.ToString()
                + ", RecipeFK: " + RecipeFK.ToString()
                + ", Note1Nbr: " + Note1Nbr.ToString()
                + ", Note1: " + Note1;

            return output;
        }
    }
}
