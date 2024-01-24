using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBooks.Models
{
    internal class LinksModel
    {
        public int RecipeFK { get; set; }
        public int LinkFK { get; set; }

        // ************************************************
        public override string ToString()
        {
            string output = " RecipeFK: " + RecipeFK.ToString()
                + ", LinkFK: " + LinkFK.ToString();

            return output;
        }
    }
}