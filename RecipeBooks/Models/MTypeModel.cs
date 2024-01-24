using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBooks.Models
{
    public class MTypeModel
    {
        public int MTypeId { get; set; }
        public string? MTypeName { get; set; }
        public string? ToUnit { get; set; }
        public float ConvertWith { get; set; }
    }
}
