using Dapper;
using RecipeBooks.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace RecipeBooks
{
    internal class SQLiteDataAccess
    {
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        #region Books

        // ************************************************
        // *** B o o k
        // ************************************************
        public static void DeleteBook(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BooksModel>($"delete from Books where BookId = {id}", new DynamicParameters());
            }
        }

        // ************************************************
        public static int CountBook()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BooksModel>($"select * from Books", new DynamicParameters());

                return output.ToList().Count;
            }
        }

        // ************************************************

        public static List<BooksModel> GetBook(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BooksModel>($"select * from Books where BooksId = {id}", new DynamicParameters());

                return output.ToList();
            }
        }

        // ************************************************

        public static List<BooksModel> LoadBooks()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BooksModel>("select * from Books", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************
        public static void AddBook(BooksModel book)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Books (BookTitle, BookSubTitle, Publisher, Author, Year, LocationFK, Copies, Cuisine, MediaType, RecipeRef) " +
                    "values (@BookTitle, @BookSubTitle,@Publisher, @Author, @Year, @LocationFK, @Copies, @Cuisine, @MediaType, @RecipeRef)", book);
            }
        }

        // ************************************************
        public static List<BooksModel> SearchBooks(string SearchQurey)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<BooksModel>(SearchQurey, new DynamicParameters());

                return output.ToList();
            }
        }

        // ************************************************

        public static void UpdateBook(BooksModel book)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string UpdateText = $"update Books set" +
                    $"  BookTitle = \"{book.BookTitle}\" " +
                    $", BookSubTitle = \"{book.BookSubTitle}\" " +
                    $", Publisher = \"{book.Publisher}\" " +
                    $", Author = \"{book.Author}\"" +
                    $", Year = {book.Year}" +
                    $", LocationFK = {book.LocationFK}" +
                    $", Copies = {book.Copies}" +
                    $", Cuisine = {book.Cuisine}" +
                    $", MediaType = {book.MediaType}" +
                    $", RecipeRef = {book.RecipeRef}" +
                    $"  where BookId = {book.BookId}";
                cnn.Execute(UpdateText, new DynamicParameters());
            }
        }

        // ************************************************

        #endregion Books

        // ************************************************

        #region Locations

        // ************************************************
        // *** L o c a t i o n s
        // ************************************************
        public static int CountLocations()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LocationsModel>("select * from Locations", new DynamicParameters());

                return output.ToList().Count;
            }
        }

        // ************************************************

        public static void DeleteLocation(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LocationsModel>($"delete from Locations where LocationId = {id}", new DynamicParameters());
            }
        }

        // ************************************************
        public static List<LocationsModel> GetLocation(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LocationsModel>($"select * from Locations where LocationsId = {id}", new DynamicParameters());

                return output.ToList();
            }
        }

        // ************************************************

        public static List<LocationsModel> LoadLocations()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LocationsModel>("select * from Locations", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************
        public static void AddLocation(LocationsModel loc)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Locations (LocationTitle) " +
                    "values (@LocationTitle)", loc);
            }
        }

        // ************************************************
        public static List<LocationsModel> SearchLocations(string SearchQurey)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LocationsModel>(SearchQurey, new DynamicParameters());

                return output.ToList();
            }
        }

        // ************************************************
        public static void UpdateLocation(LocationsModel loc)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string UpdateText = $"update Locations set" +
                    $" LocationTitle = \"{loc.LocationTitle}\"" +
                    $" where LocationId = {loc.LocationId}";
                cnn.Execute(UpdateText, new DynamicParameters());
            }
        }

        // ************************************************

        #endregion Locations

        // ************************************************

        #region Chapters

        // ************************************************
        // *** C h a p t e r s
        // ************************************************
        public static int CountChapters()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ChaptersModel>("select * from Chapters", new DynamicParameters());

                return output.ToList().Count;
            }
        }

        // ************************************************

        public static void DeleteChapter(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ChaptersModel>($"delete from Chapters where ChapterId = {id}", new DynamicParameters());
            }
        }

        // ************************************************
        public static List<ChaptersModel> GetChapter(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ChaptersModel>($"select * from Chapters where ChapterId = {id}", new DynamicParameters());

                return output.ToList();
            }
        }

        // ************************************************

        public static List<ChaptersModel> LoadChapters()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ChaptersModel>("select * from Chapters", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************
        public static void AddChapter(ChaptersModel chapter)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Chapters (ChapterName) " +
                    "values (@ChapterName)", chapter);
            }
        }

        // ************************************************
        public static List<ChaptersModel> SearchChapters(string SearchQurey)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ChaptersModel>(SearchQurey, new DynamicParameters());

                return output.ToList();
            }
        }

        // ************************************************
        public static void UpdateChapter(ChaptersModel chapter)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string UpdateText = $"update Chapters set" +
                    $" ChapterName = \"{chapter.ChapterName}\"" +
                    $"  where ChapterId = {chapter.ChapterId}";
                cnn.Execute(UpdateText, new DynamicParameters());
            }
        }

        // ************************************************

        #endregion Chapters

        // ************************************************

        #region Recipes

        // ************************************************
        // *** R e c i p e s
        // ************************************************
        public static int CountRecipes(int bookId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<RecipesModel>($"select * from Recipes where BookFk = {bookId}", new DynamicParameters());

                return output.ToList().Count;
            }
        }

        // ************************************************
        public static int CountAllRecipes()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<RecipesModel>($"select * from Recipes", new DynamicParameters());

                return output.ToList().Count;
            }
        }

        // ************************************************


        public static void DeleteRecipe(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<RecipesModel>($"delete from Recipes where RecipeId = {id}", new DynamicParameters());
            }
        }

        // ************************************************
        public static List<RecipesModel> GetRecipe(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<RecipesModel>($"select * from Recipes where RecipeId = {id}", new DynamicParameters());

                return output.ToList();
            }
        }

        // ************************************************

        public static List<RecipesModel> LoadRecipes(int bookId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<RecipesModel>($"select * from Recipes where BookFk = {bookId}", new DynamicParameters());

                return output.ToList();
            }
        }

        // ************************************************
        public static void AddRecipe(RecipesModel recp)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Recipes (BookFk, RecipeTitle, ChapterFk, Referance, Makes, Serving, Quantity, QtyType, Preparation, Cooking, Cuisine, Favourite, Image) " +
                    "values (@BookFk, @RecipeTitle, @ChapterFk, @Referance, @Makes, @Serving, @Quantity, @QtyType,@Preparation, @Cooking, @Cuisine, @Favourite, @Image)", recp);
            }
        }

        // ************************************************
        public static List<RecipesModel> SearchRecipes(string SearchQurey)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<RecipesModel>(SearchQurey, new DynamicParameters());

                return output.ToList();
            }
        }

        // ************************************************
        public static void UpdateRecipe(RecipesModel r)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string UpdateText = $"update Recipes set" +
                    $"  BookFk = {r.BookFk}" +
                    $", RecipeTitle = \"{r.RecipeTitle}\"" +
                    $", ChapterFk = {r.ChapterFk}" +
                    $", Referance = {r.Referance}" +
                    $", Makes = {r.Makes}" +
                    $", Serving = {r.Serving}" +
                    $", Quantity = {r.Quantity}" +
                    $", QtyType = {r.QtyType}" +
                    $", Preparation = \"{r.Preparation}\"" +
                    $", Cooking = \"{r.Cooking}\"" +
                    $", Cuisine = {r.Cuisine}" +
                    $", Favourite = {r.Favourite}" +
                    $", Image = \"{r.Image}\"" +
                    $"  where RecipeId = {r.RecipeId}";

                cnn.Execute(UpdateText, new DynamicParameters());
            }
        }

        // ************************************************

        #endregion Recipes

        // ************************************************

        #region Ingredients

        // ************************************************
        // *** I n g r e d i e n t
        // ************************************************

        public static int CountIngredients()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<IngredientsModel>($"select * from Ingredients", new DynamicParameters());

                return output.ToList().Count;
            }
        }
        // ************************************************
        public static void DeleteIngredient(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<IngredientsModel>($"delete from Ingredients where IngredientId = {id}", new DynamicParameters());
            }
        }

        // ************************************************

        public static List<IngredientsModel> GetIngredient(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<IngredientsModel>($"select * from Ingredients where IngredientId = {id}", new DynamicParameters());

                return output.ToList();
            }
        }

        // ************************************************

        public static List<IngredientsModel> LoadIngredients()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<IngredientsModel>("select * from Ingredients", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************
        public static void AddIngredient(IngredientsModel ingredient)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Ingredients (IngredientName, IngredientTypeEN, Calories, Energy, Fat, SaturatedFat, Carbohydrate," +
                    " Sugars, Fibre, Protein, Salt, Liquid, Mass, Small, Medium, Large)" +
                    " values (@IngredientName, @IngredientTypeEN, @Calories, @Energy, @Fat, @SaturatedFat, @Carbohydrate," +
                    " @Sugars, @Fibre, @Protein, @Salt, @Liquid, @Mass, @Small, @Medium, @Large)", ingredient);
            }
        }

        // ************************************************
        public static List<IngredientsModel> SearchIngredients(string SearchQurey)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<IngredientsModel>(SearchQurey, new DynamicParameters());

                return output.ToList();
            }
        }

        // ************************************************

        public static void UpdateIngredient(IngredientsModel ingredient)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string UpdateText = $"update Ingredients set" +
                    $"  IngredientName = \"{ingredient.IngredientName}\" " +
                    $", IngredientTypeEN = \"{ingredient.IngredientTypeEN}\" " +
                    $", Calories = \"{ingredient.Calories}\" " +
                    $", Energy = \"{ingredient.Energy}\"" +
                    $", Fat = {ingredient.Fat}" +
                    $", SaturatedFat = {ingredient.SaturatedFat}" +
                    $", Carbohydrate = {ingredient.Carbohydrate}" +
                    $", Sugars = {ingredient.Sugars}" +
                    $", Fibre = {ingredient.Fibre}" +
                    $", Protein = {ingredient.Protein}" +
                    $", Salt = {ingredient.Salt}" +
                    $", Liquid = {ingredient.Liquid}" +
                    $", Mass = {ingredient.Mass}" +
                    $", Small = {ingredient.Small}" +
                    $", Medium = {ingredient.Medium}" +
                    $", Large = {ingredient.Large}" +
                    $"  where IngredientId = {ingredient.IngredientId}";
                cnn.Execute(UpdateText, new DynamicParameters());
            }
        }

        // ************************************************

        #endregion Ingredients

        // ************************************************

        #region IngList

        // ************************************************
        // *** Ingredient List
        // ************************************************
        public static void AddIngListItem(IngListItemModel ili)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into IngList (RecipeFK, IngredientFK, Quantity, MType) " +
                    "values (@RecipeFK, @IngredientFK, @Quantity, @MType)", ili);
            }
        }

        // ************************************************
        public static void DeleteIngListItem(int rcpID, int ingID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<IngListItemModel>(
                    $"delete from IngList where RecipeFK = {rcpID} and IngredientFK = {ingID}",
                    new DynamicParameters());
            }
        }

        // ************************************************

        public static List<IngListItemModel> LoadIngListItem(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<IngListItemModel>($"select * from IngList where RecipeFK = {id}", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************
        public static int CountIngListItem(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<IngListItemModel>($"select * from IngList where RecipeFK = {id}", new DynamicParameters());

                return output.ToList().Count;
            }
        }

        // ************************************************

        #endregion IngList

        // ************************************************

        #region Link

        // ************************************************
        // *** Links
        // ************************************************
        public static void AddLink(LinksModel lk)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Link (RecipeFK, LinkFK) values (@RecipeFK, @LinkFK)", lk);
            }
        }

        // ************************************************
        public static void DeleteLinks(int rcpID, int lkID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LinksModel>(
                    $"delete from Link where RecipeFK = {rcpID} and LinkFK = {lkID}",
                    new DynamicParameters());
            }
        }

        // ************************************************

        public static List<LinksModel> LoadLinks(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LinksModel>($"select * from Link where RecipeFK = {id}", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************

        #endregion Link

        // ************************************************

        #region Directions

        // ************************************************
        // *** Directions
        // ************************************************
        public static void AddDirection(DirectionsModel dir)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Direction (RecipeFK, Position, Direction) " +
                    "values (@RecipeFK, @Position, @Direction)", dir);
            }
        }

        // ************************************************
        public static void DeleteDirections(int rcpID, int dirID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<DirectionsModel>(
                    $"delete from Direction where RecipeFK = {rcpID} and DirectId = {dirID}",
                    new DynamicParameters());
            }
        }

        // ************************************************

        public static List<DirectionsModel> LoadDirections(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<DirectionsModel>($"select * from Direction where RecipeFK = {id}", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************

        public static void UpdateDirection(DirectionsModel dir)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string UpdateText = $"update Direction set" +
                    $" Position = {dir.Position} " +
                    $", Direction = \"{dir.Direction}\" " +
                    $"  where RecipeFK = {dir.RecipeFK} and DirectId = {dir.DirectId}";

                cnn.Execute(UpdateText, new DynamicParameters());
            }
        }

        // ************************************************

        #endregion Directions

        // ************************************************

        #region Note1

        // ************************************************
        // *** Note1
        // ************************************************
        public static void AddNote1(Note1sModel note)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Note1 (RecipeFK, Note1Nbr, Note1) " +
                    "values (@RecipeFK, @Note1Nbr, @Note1)", note);
            }
        }

        // ************************************************
        public static void DeleteNote1s(int rcpID, int noteID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Note1sModel>(
                    $"delete from Note1 where RecipeFK = {rcpID} and Note1Id = {noteID}",
                    new DynamicParameters());
            }
        }

        // ************************************************

        public static List<Note1sModel> LoadNote1s(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Note1sModel>($"select * from Note1 where RecipeFK = {id}", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************

        public static void UpdateNote1(Note1sModel note)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string UpdateText = $"update Note1 set" +
                    $" Note1Nbr = {note.Note1Nbr} " +
                    $", Note1 = \"{note.Note1}\" " +
                    $"  where RecipeFK = {note.RecipeFK} and Note1Id = {note.Note1Id}";

                cnn.Execute(UpdateText, new DynamicParameters());
            }
        }

        // ************************************************

        #endregion Note1

        // ************************************************

        #region Note2

        // ************************************************
        // *** Note2
        // ************************************************
        public static void AddNote2(Note2sModel note)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Note2 (RecipeFK, Note2Nbr, Note2) " +
                    "values (@RecipeFK, @Note2Nbr, @Note2)", note);
            }
        }

        // ************************************************
        public static void DeleteNote2s(int rcpID, int noteID)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Note2sModel>(
                    $"delete from Note2 where RecipeFK = {rcpID} and Note2Id = {noteID}",
                    new DynamicParameters());
            }
        }

        // ************************************************

        public static List<Note2sModel> LoadNote2s(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Note2sModel>($"select * from Note2 where RecipeFK = {id}", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************

        public static void UpdateNote2(Note2sModel note)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                string UpdateText = $"update Note2 set" +
                    $" Note2Nbr = {note.Note2Nbr} " +
                    $", Note2 = \"{note.Note2}\" " +
                    $"  where RecipeFK = {note.RecipeFK} and Note2Id = {note.Note2Id}";

                cnn.Execute(UpdateText, new DynamicParameters());
            }
        }

        // ************************************************

        #endregion Note2

        // ************************************************

        #region MediaType

        // ************************************************
        // *** M e d i a T y p e
        // ************************************************

        public static List<MediaTypeModel> LoadMediaTypes()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<MediaTypeModel>("select * from MediaType", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************

        #endregion MediaType

        // ************************************************

        #region Cusine

        // ************************************************
        // *** C u s i n e
        // ************************************************

        public static List<CountryModel> LoadCusines()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<CountryModel>("select * from Country", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************

        #endregion Cusine

        // ************************************************

        #region IngType

        // ************************************************
        // *** I n g T y p e
        // ************************************************

        public static List<IngTypeModel> LoadIngTypes()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<IngTypeModel>("select * from IngType", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************

        #endregion IngType

        // ************************************************

        #region MType

        // ************************************************
        // *** M T y p e
        // ************************************************

        public static List<MTypeModel> LoadMTypes()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<MTypeModel>("select * from MType", new DynamicParameters());
                return output.ToList();
            }
        }

        // ************************************************

        #endregion MType

        // ************************************************
        // ************************************************
        // ************************************************
    }
}