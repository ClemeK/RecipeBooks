namespace RecipeBooks.Models
{
    public class IngredientsModel
    {
        public int IngredientId { get; set; }
        public string? IngredientName { get; set; }
        public int IngredientTypeEN { get; set; }

        // Nutrition Info
        public int Calories { get; set; }

        public int Energy { get; set; }          // in kJ
        public float Fat { get; set; }           // in g
        public float SaturatedFat { get; set; }  // in g
        public float Carbohydrate { get; set; }  // in g
        public float Sugars { get; set; }        // in g
        public float Fibre { get; set; }         // in g
        public float Protein { get; set; }       // in g
        public float Salt { get; set; }          // in g
        public bool Liquid { get; set; }
        public float Mass { get; set; }
        public float Small { get; set; }
        public float Medium { get; set; }
        public float Large { get; set; }

        // ************************************************
        public void Sodium(float amount)
        {
            Salt = amount * 2.5f;
        }

        // ************************************************
        public override string ToString()
        {
            string output = " IngredientId: " + IngredientId.ToString()
                + " IngredientName: " + IngredientName
                + ", IngredientTypeEN: " + IngredientTypeEN.ToString()
                + ", Calories: " + Calories.ToString()
                + ", Energy: " + Energy.ToString()
                + ", Fat: " + Fat.ToString()
                + ", SaturatedFat: " + SaturatedFat.ToString()
                + ", Carbohydrate: " + Carbohydrate.ToString()
                + ", Sugars: " + Sugars.ToString()
                + ", Fibre: " + Fibre.ToString()
                + ", Protein: " + Protein.ToString()
                + ", Salt: " + Salt.ToString()
                + ", Liquid: " + Liquid.ToString()
                + ", Mass: " + Mass.ToString()
                + ", Small: " + Small.ToString()
                + ", Medium: " + Medium.ToString()
                + ", Large: " + Large.ToString();
            return output;
        }

        // ************************************************
        public bool NutientInfo()
        {
            double calc = Fat + SaturatedFat + Carbohydrate + Sugars + Fibre + Protein + Salt;

            return (calc > 0) ? true : false;
        }

        // ************************************************
        public override bool Equals(object obj)
        {
            if (!(obj is IngredientsModel))
                return false;

            var other = obj as IngredientsModel;

            if (IngredientName != other.IngredientName)
                return false;

            if (IngredientTypeEN != other.IngredientTypeEN)
                return false;

            return true;
        }

        // ************************************************
    }
}