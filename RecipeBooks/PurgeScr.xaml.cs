using RecipeBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RecipeBooks
{
    /// <summary>
    /// Interaction logic for Purge.xaml
    /// </summary>
    public partial class PurgeScr : Window
    {
        // ************************************************
        // ***     G L O B A L E   V A R I A B L E S
        private Joblog errorlog = new Joblog("Books", 1);

        private List<BooksModel> Books = new List<BooksModel>();
        private List<RecipesModel> Recipes = new List<RecipesModel>();

        private List<LocationsModel> Locations = new List<LocationsModel>();
        private List<ChaptersModel> Chapters = new List<ChaptersModel>();

        private List<IngredientsModel> Ingredients = new List<IngredientsModel>();

        public PurgeScr(Joblog el)
        {
            InitializeComponent();

            errorlog = el;

            UpdateLabels();
        }

        private void UpdateLabels()
        {
            lblBooks.Content = $"({SQLiteDataAccess.CountBook()})";
            lblRecipes.Content = $"({SQLiteDataAccess.CountAllRecipes()})";
            lblChapters.Content = $"({SQLiteDataAccess.CountChapters()})";
            lblLocations.Content = $"({SQLiteDataAccess.CountLocations()})";
            lblIngredients.Content = $"({SQLiteDataAccess.CountIngredients()})";
        }

        private void PurgeButton(object sender, RoutedEventArgs e)
        {
            MessageBoxResult response = MessageBox.Show($"Do you really want to do this?",
                                   "Recipe Books",
                                   MessageBoxButton.YesNo,
                                   MessageBoxImage.Warning);

            if (response == MessageBoxResult.Yes)
            {
                // Delete Books
                if (cbBooks.IsChecked == true)
                {
                    Books = SQLiteDataAccess.LoadBooks();

                    foreach (var b in Books)
                    {
                        Recipes = SQLiteDataAccess.LoadRecipes(b.BookId);
                        General.DeleteRecipes(errorlog, Recipes);

                        SQLiteDataAccess.DeleteBook(b.BookId);
                    }
                }

                UpdateLabels();

                // Delete Recipes
                if (cbRecipes.IsChecked == true)
                {
                    Books = SQLiteDataAccess.LoadBooks();
                    Recipes.Clear();

                    List<RecipesModel> TempRcp = new List<RecipesModel>();

                    foreach (var b in Books)
                    {
                        TempRcp = SQLiteDataAccess.LoadRecipes(b.BookId);

                        Recipes.AddRange(TempRcp);
                    }

                    General.DeleteRecipes(errorlog, Recipes);
                }

                UpdateLabels();

                // Delete Chapters
                if (cbChapters.IsChecked == true)
                {
                    Chapters = SQLiteDataAccess.LoadChapters();

                    foreach (var c in Chapters)
                    {
                        SQLiteDataAccess.DeleteChapter(c.ChapterId);
                    }
                }

                UpdateLabels();

                // Delete Locations
                if (cbLocations.IsChecked == true)
                {
                    Locations = SQLiteDataAccess.LoadLocations();

                    foreach (var l in Locations)
                    {
                        SQLiteDataAccess.DeleteLocation(l.LocationId);
                    }
                }

                UpdateLabels();

                // Delete Ingredients
                if (cbIngredients.IsChecked == true)
                {
                    Ingredients = SQLiteDataAccess.LoadIngredients();

                    foreach (var i in Ingredients)
                    {
                        SQLiteDataAccess.DeleteIngredient(i.IngredientId);
                    }
                }

                UpdateLabels();
            }
        }
    }
}