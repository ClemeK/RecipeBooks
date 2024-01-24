using RecipeBooks.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RecipeBooks
{
    public static class General
    {
        // ************************************************
        public static void DeleteRecipe(Joblog errorlog, RecipesModel br)
        {
            List<IngListItemModel> IngList = new();
            List<LinksModel> Links = new();
            List<DirectionsModel> Directions = new();
            List<Note1sModel> Note1s = new();
            List<Note2sModel> Note2s = new();

            // Ingredient List Item
            IngList = SQLiteDataAccess.LoadIngListItem(br.RecipeId);

            foreach (var i in IngList)
            {
                errorlog.InformationMessage($"Recipe {br.RecipeTitle}, Ingredient Deleted - {i.IngredientFK.ToString()}");

                SQLiteDataAccess.DeleteIngListItem(br.RecipeId, i.IngredientFK);
            }

            // Links
            Links = SQLiteDataAccess.LoadLinks(br.RecipeId);

            foreach (var l in Links)
            {
                errorlog.InformationMessage($"Recipe {br.RecipeTitle}, Link Deleted - {l.LinkFK}");

                SQLiteDataAccess.DeleteLinks(br.RecipeId, l.LinkFK);
            }

            // Directions
            Directions = SQLiteDataAccess.LoadDirections(br.RecipeId);

            foreach (var d in Directions)
            {
                errorlog.InformationMessage($"Recipe {br.RecipeTitle}, Direction Deleted - {d.Direction}");

                SQLiteDataAccess.DeleteDirections(br.RecipeId, d.DirectId);
            }

            // Notes1
            Note1s = SQLiteDataAccess.LoadNote1s(br.RecipeId);

            foreach (var n in Note1s)
            {
                errorlog.InformationMessage($"Recipe {br.RecipeTitle}, Note1 Deleted - {n.Note1}");

                SQLiteDataAccess.DeleteNote1s(br.RecipeId, n.Note1Id);
            }

            // Notes2
            Note2s = SQLiteDataAccess.LoadNote2s(br.RecipeId);

            foreach (var n in Note2s)
            {
                errorlog.InformationMessage($"Recipe {br.RecipeTitle},Note2 Deleted - {n.Note2}");

                SQLiteDataAccess.DeleteNote2s(br.RecipeId, n.Note2Id);
            }

            errorlog.InformationMessage($"Recipe Deleted - {br.RecipeTitle}");
            SQLiteDataAccess.DeleteRecipe(br.RecipeId);
        }

        // ************************************************
        public static void DeleteRecipes(Joblog errorlog, List<RecipesModel> BookRecipes)
        {
            if (BookRecipes.Count > 0)
            {
                foreach (var br in BookRecipes)
                {
                    DeleteRecipe(errorlog, br);
                }
            }
        }

        // ************************************************
        public static void ExportRecipeDetails(Joblog errorlog, RecipesModel br, string filename)
        {
            List<IngredientsModel> Ingredients = new List<IngredientsModel>();
            Ingredients = SQLiteDataAccess.LoadIngredients();

            List<BooksModel> Books = new();
            Books = SQLiteDataAccess.LoadBooks();

            List<IngListItemModel> IngList = new();
            List<DirectionsModel> Directions = new();
            List<Note1sModel> Note1s = new();
            List<Note2sModel> Note2s = new();

            // Get the path to the csv file
            string path = Path.GetDirectoryName(filename);

            // Add in a recipe directory as a Hex Number
            int RecRef = LookupBookRef(br.BookFk, Books);
            path = path + "\\" + RecRef.ToString("X4");

            if (!File.Exists($"{path}\\{br.RecipeTitle}.rcp"))
            {
                IngList = SQLiteDataAccess.LoadIngListItem(br.RecipeId);
                Directions = SQLiteDataAccess.LoadDirections(br.RecipeId);
                Note1s = SQLiteDataAccess.LoadNote1s(br.RecipeId);
                Note2s = SQLiteDataAccess.LoadNote2s(br.RecipeId);

                // Check if there is any Ingredients
                if (IngList.Count > 0)
                {
                    // Check the Directory exist and if not Create it
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    // Create file
                    var thisRecipe = new IniFile($"{path}\\{br.RecipeTitle}.rcp");

                    // Add in Book Title
                    thisRecipe.Write("FullTitle", $"{LookupBook(br.BookFk, Books)}");

                    // Export Ingredients List
                    string Section = "IngList";
                    thisRecipe.Write(Section, $"{IngList.Count}");

                    {
                        int j = 1;

                        foreach (var i in IngList)
                        {
                            string SectionName = $"{Section}{j}";

                            thisRecipe.Write($"Ingredient", LookupIngredientID(i.IngredientFK, Ingredients), SectionName);
                            thisRecipe.Write($"Quantity", i.Quantity.ToString(), SectionName);
                            thisRecipe.Write($"MType", i.MType.ToString(), SectionName);

                            j++;
                        }
                    }

                    // Export Directions
                    Section = "Directions";
                    thisRecipe.Write(Section, $"{Directions.Count}");

                    if (Directions.Count > 0)
                    {
                        int j = 1;

                        foreach (var i in Directions)
                        {
                            string SectionName = $"{Section}{j}";

                            thisRecipe.Write($"Position", i.Position.ToString(), SectionName);
                            thisRecipe.Write($"Direction", i.Direction, SectionName);

                            j++;
                        }
                    }

                    // Export Notes1
                    Section = "Note1s";
                    thisRecipe.Write(Section, $"{Note1s.Count}");

                    if (Note1s.Count > 0)
                    {
                        foreach (var i in Note1s)
                        {
                            thisRecipe.Write($"Note1", i.Note1, $"{Section}{i.Note1Nbr}");
                        }
                    }

                    // Export Notes2
                    Section = "Note2s";
                    thisRecipe.Write(Section, $"{Note2s.Count}");

                    if (Note2s.Count > 0)
                    {
                        foreach (var i in Note2s)
                        {
                            thisRecipe.Write($"Note2", i.Note2, $"{Section}{i.Note2Nbr}");
                        }
                    }
                }
            }

            string mesBoxTitle = "Export Recipe";
            string msgText = $"{br.RecipeTitle} details has been Exported to file {br.RecipeTitle}.rcp.";

            errorlog.InformationMessage(msgText, mesBoxTitle);
        }

        // ************************************************
        public static void FloatValidation(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Only allow numbers in the textbox
            Regex regex = new Regex("[^0-9].");
            e.Handled = regex.IsMatch(e.Text);
        }

        // ************************************************
        public static void ImportRecipeDetails(Joblog errorlog, RecipesModel br, string filename)
        {
            List<IngredientsModel> Ingredients = new List<IngredientsModel>();
            Ingredients = SQLiteDataAccess.LoadIngredients();

            List<BooksModel> Books = new List<BooksModel>();
            Books = SQLiteDataAccess.LoadBooks();

            // Get the path to the csv file
            string path = Path.GetDirectoryName(filename);

            // Add in a recipe directory as a Hex Number
            int RecRef = LookupBookRef(br.BookFk, Books);
            path = path + "\\" + RecRef.ToString("X4");

            // Add the Recipe File name to the path
            string file = $"{path}\\{br.RecipeTitle}.rcp";

            if (File.Exists(file))
            {
                var thisRecipe = new IniFile(file);

                // Read Book Title
                string BookTitle = thisRecipe.Read("FullTitle");
                if (LookupBook(br.BookFk, Books) == BookTitle)
                {
                    // Import Ingredients List
                    string Section = "IngList";
                    int Count = int.Parse(thisRecipe.Read(Section));

                    if (Count > 0)
                    {
                        for (int i = 0; i < Count; i++)
                        {
                            IngListItemModel Ing = new();

                            string SectionName = $"{Section}{i + 1}";

                            Ing.RecipeFK = br.RecipeId;
                            Ing.IngredientFK = LookupIngredientName(thisRecipe.Read($"Ingredient", SectionName).Trim(), Ingredients);
                            Ing.Quantity = float.Parse(thisRecipe.Read($"Quantity", SectionName));
                            Ing.MType = int.Parse(thisRecipe.Read($"MType", SectionName));

                            SQLiteDataAccess.AddIngListItem(Ing);
                        }
                    }

                    // Import Directions
                    Section = "Directions";
                    Count = int.Parse(thisRecipe.Read(Section));

                    if (Count > 0)
                    {
                        for (int i = 0; i < Count; i++)
                        {
                            DirectionsModel Dir = new();

                            string SectionName = $"{Section}{i + 1}";

                            Dir.RecipeFK = br.RecipeId;
                            Dir.Position = int.Parse(thisRecipe.Read($"Position", SectionName));
                            Dir.Direction = thisRecipe.Read($"Direction", SectionName);

                            SQLiteDataAccess.AddDirection(Dir);
                        }
                    }

                    // Import Notes1
                    Section = "Note1s";
                    Count = int.Parse(thisRecipe.Read(Section));

                    if (Count > 0)
                    {
                        for (int i = 0; i < Count; i++)
                        {
                            Note1sModel Note = new();

                            Note.RecipeFK = br.RecipeId;
                            Note.Note1Nbr = i + 1;
                            Note.Note1 = thisRecipe.Read($"Note1", $"{Section}{i + 1}");

                            SQLiteDataAccess.AddNote1(Note);
                        }
                    }

                    // Import Notes2
                    Section = "Note2s";
                    Count = int.Parse(thisRecipe.Read(Section));

                    if (Count > 0)
                    {
                        for (int i = 0; i < Count; i++)
                        {
                            Note2sModel Note = new();

                            Note.RecipeFK = br.RecipeId;
                            Note.Note2Nbr = i + 1;
                            Note.Note2 = thisRecipe.Read($"Note2", $"{Section}{i + 1}");

                            SQLiteDataAccess.AddNote2(Note);
                        }
                    }
                }

                string mesBoxTitle = "Import Recipe";
                string msgText = $"{br.RecipeTitle} details have been Imported from file {br.RecipeTitle}.rcp.";

                errorlog.InformationMessage(msgText, mesBoxTitle);
            }
        }

        // ************************************************
        public static void IntValidation(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Only allow numbers in the textbox
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        // ************************************************
        public static int LookupIngredientType(string ing)
        {
            List<IngTypeModel> IngredientType = new();
            IngredientType = SQLiteDataAccess.LoadIngTypes();

            var m = IngredientType.Find(x => x.IngType == ing);

            if (m != null)
            {
                return m.IngId;
            }

            return -1;
        }

        // ************************************************
        public static string LookupIngredientTypeID(int id)
        {
            List<IngTypeModel> IngredientType = new();
            IngredientType = SQLiteDataAccess.LoadIngTypes();

            var m = IngredientType.Find(x => x.IngId == id);

            if (m != null)
            {
                return m.IngType;
            }

            return "";
        }

        // ************************************************
        public static int LookupMType(string mtype)
        {
            List<MTypeModel> MtType = new();
            MtType = SQLiteDataAccess.LoadMTypes();

            var m = MtType.Find(x => x.MTypeName == mtype);

            if (m != null)
            {
                return m.MTypeId;
            }

            return -1;
        }

        // ************************************************
        public static string LookupMTypeID(int id)
        {
            List<MTypeModel> MType = new();
            MType = SQLiteDataAccess.LoadMTypes();

            var m = MType.Find(x => x.MTypeId == id);

            if (m != null)
            {
                return m.MTypeName;
            }

            return "";
        }

        // ************************************************
        private static string LookupBook(int id, List<BooksModel> books)
        {
            var m = books.Find(x => x.BookId == id);

            if (m != null)
            {
                return m.FullTitle;
            }

            return "";
        }

        // ************************************************
        private static int LookupBookRef(int id, List<BooksModel> books)
        {
            var m = books.Find(x => x.BookId == id);

            if (m != null)
            {
                return m.RecipeRef;
            }

            return -1;
        }

        // ************************************************
        public static string LookupIngredientID(int id, List<IngredientsModel> Ingredients)
        {
            var m = Ingredients.Find(x => x.IngredientId == id);

            if (m != null)
            {
                return m.IngredientName;
            }

            return "";
        }

        // ************************************************
        public static int LookupIngredientName(string name, List<IngredientsModel> Ingredients)
        {
            var m = Ingredients.Find(x => x.IngredientName == name);

            if (m != null)
            {
                return m.IngredientId;
            }

            return -1;
        }

        // ************************************************
        public static IngredientsModel LookupIngredient(int id, List<IngredientsModel> Ingredients)
        {
            var m = Ingredients.Find(x => x.IngredientId == id);

            if (m != null)
            {
                return m;
            }

            return null;
        }

        // ************************************************
    }
}