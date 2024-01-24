using RecipeBooks.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RecipeBooks
{
    public partial class BookContents : Window
    {
        private List<BooksModel> Books = new();
        private List<ChaptersModel> Chapters = new();
        private List<CountryModel> Cuisine = new List<CountryModel>();
        private BooksModel CurrentBook = new();
        private Joblog errorlog = new Joblog("Books", 7);
        private List<RecipesModel> Recipes = new();

        // ************************************************
        // ***   B o o k C o n t e n t s   W I N D O W
        public BookContents(Joblog el, BooksModel book)
        {
            InitializeComponent();

            errorlog = el;
            CurrentBook = book;

            lblBookTitle.Content = $"{book.FullTitle}";

            RefreshChapter();
            RefreshRecipeLB();

            WireUpCuisineCB();

            ClearRecipeBoxes();
        }

        // *********************************************************
        private void AddRecipeButton(object sender, RoutedEventArgs e)
        {
            if (ValidateRecipesSrceen() == true)
            {
                RecipesModel r = new RecipesModel();

                r.RecipeTitle = tbRecipeTitle.Text.Trim();
                r.BookFk = CurrentBook.BookId;
                r.ChapterFk = LookupChapter(cbChapter.Text);
                r.Cuisine = LookupCuisine(cbCuisine2.Text);
                r.Referance = int.Parse(tbReferance.Text.Trim());

                if (ckbFavourite.IsChecked == true)
                {
                    r.Favourite = true;
                }
                else
                {
                    r.Favourite = true;
                }

                r.Image = string.Empty;

                SQLiteDataAccess.AddRecipe(r);

                // Refresh ListBox
                RefreshRecipeLB();

                ClearRecipeBoxes();

                errorlog.InformationMessage("Recipe Added - ", r.ToString());
            }
            else
            {
                MessageBox.Show("One or more of the fields are blank!", "Recipe Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // *********************************************************
        private void ClearRecipeBoxes()
        {
            lblRecipeLabel.Content = (Recipes.Count != 0) ? $"Recipes ({Recipes.Count})" : "Recipes";

            lblRecipeIdValue.Content = "";

            tbRecipeTitle.Text = "";
            cbChapter.SelectedIndex = 0;
            cbCuisine2.SelectedIndex = CurrentBook.Cuisine;
            tbReferance.Text = "";
            ckbFavourite.IsChecked = false;
        }

        // *********************************************************
        private void ClearRecipeButton(object sender, RoutedEventArgs e)
        {
            ClearRecipeBoxes();
        }

        // *********************************************************
        private void DeleteRecipeButton(object sender, RoutedEventArgs e)
        {
            int index = lbRecipesList.SelectedIndex;

            General.DeleteRecipe(errorlog, Recipes[index]);

            RefreshRecipeLB();
        }

        // *********************************************************
        private void EditRecipeButton(object sender, RoutedEventArgs e)
        {
            int index = lbRecipesList.SelectedIndex;

            if (index != -1)
            {
                new RecipeContents(errorlog, Recipes[index]).Show();
            }
        }

        // *********************************************************
        private int LookupBook(string bt, string bst)
        {
            string ft = $"{bt.Trim()} {bst.Trim()}";

            var m = Books.Find(x => x.FullTitle == ft);

            if (m != null)
            {
                return m.BookId;
            }

            return -1;
        }

        // *********************************************************
        private int LookupChapter(string loc)
        {
            var m = Chapters.Find(x => x.ChapterName == loc);

            if (m != null)
            {
                return m.ChapterId;
            }

            return -1;
        }

        // *********************************************************
        private string LookupChapterID(int id)
        {
            var m = Chapters.Find(x => x.ChapterId == id);

            if (m != null)
            {
                return m.ChapterName;
            }

            return "";
        }

        // ************************************************
        private int LookupCuisine(string mt)
        {
            var m = Cuisine.Find(x => x.CountryName == mt);

            if (m != null)
            {
                return m.CountryId;
            }

            return -1;
        }

        // ************************************************
        private string LookupCuisineID(int id)
        {
            var m = Cuisine.Find(x => x.CountryId == id);

            if (m != null)
            {
                return m.CountryName;
            }

            return "";
        }

        // *********************************************************
        private void RecipeSelectedListBox(object sender, SelectionChangedEventArgs e)
        {
            int index = lbRecipesList.SelectedIndex;

            if (index != -1)
            {
                // move data into the screen
                lblRecipeIdValue.Content = $"{Recipes[index].RecipeId}/{index}";

                tbRecipeTitle.Text = Recipes[index].RecipeTitle.Trim();
                cbChapter.Text = LookupChapterID(Recipes[index].ChapterFk);
                cbCuisine2.Text = LookupCuisineID(Recipes[index].Cuisine);
                tbReferance.Text = Recipes[index].Referance.ToString();
            }
        }

        // *********************************************************
        private void RefreshChapter()
        {
            // Get the Chapters
            Chapters = SQLiteDataAccess.LoadChapters();

            // Sort the Chapters
            Chapters.Sort((x, y) => x.ChapterName.CompareTo(y.ChapterName));

            // Display the Chapters in the ComboBox
            WireUpChaptersCB();
        }

        // *********************************************************
        private void RefreshRecipeLB()
        {
            // Get the Section
            Recipes.Clear();
            Recipes = SQLiteDataAccess.LoadRecipes(CurrentBook.BookId);

            // Sort
            Recipes.Sort((x, y) => x.RecipeTitle.CompareTo(y.RecipeTitle));
            Recipes.Sort((x, y) => x.ChapterFk.CompareTo(y.ChapterFk));

            // Display the books in the ListBox
            // Must be Refreshed after EVERYTHING else
            WireUpRecipesLB();

            lblRecipeIdValue.Content = "";
        }

        // *********************************************************
        private void RTitleChanged(object sender, TextChangedEventArgs e)
        {
            if (btnAddRecipe != null)
            {
                btnAddRecipe.IsEnabled = true;

                if (tbRecipeTitle.Text != "")
                {
                    btnUpdateRecipe.IsEnabled = true;
                    btnDeleteRecipe.IsEnabled = true;
                    btnEditRecipe.IsEnabled = true;
                }
                else
                {
                    btnAddRecipe.IsEnabled = false;
                    btnUpdateRecipe.IsEnabled = false;
                    btnDeleteRecipe.IsEnabled = false;
                    btnEditRecipe.IsEnabled = false;
                }
            }
        }

        // *********************************************************
        private void UpdateRecipeButton(object sender, RoutedEventArgs e)
        {
            if (ValidateRecipesSrceen() == true)
            {
                int index = lbRecipesList.SelectedIndex;

                errorlog.InformationMessage("Recipe Updated", "From: " + Recipes[index].ToString());

                Recipes[index].RecipeTitle = tbRecipeTitle.Text;
                Recipes[index].ChapterFk = LookupChapter(cbChapter.Text);
                Recipes[index].Cuisine = LookupCuisine(cbCuisine2.Text.Trim());
                Recipes[index].Referance = int.Parse(tbReferance.Text);

                Recipes[index].Favourite = (ckbFavourite.IsChecked == true) ? true : false;

                errorlog.InformationMessage("", "To:" + Recipes[index].ToString());

                SQLiteDataAccess.UpdateRecipe(Recipes[index]);

                // Refresh ListBox
                RefreshRecipeLB();

                ClearRecipeBoxes();
            }
        }

        // *********************************************************
        private bool ValidateRecipesSrceen()
        {
            bool output = true;
            string caption = "Book Contents";

            if (tbRecipeTitle.Text == "")
            {
                output = false;
                MessageBox.Show("Recipe Title can not be blank!", caption, MessageBoxButton.OK);
            }

            if (cbChapter.Text == "")
            {
                output = false;
                MessageBox.Show("A chapter needs to be selected!", caption, MessageBoxButton.OK);
            }

            if (tbReferance.Text == "")
            {
                output = false;
                MessageBox.Show("Referance can not be blank!", caption, MessageBoxButton.OK);
            }

            if (!int.TryParse(tbReferance.Text, out int value))
            {
                output = false;
                MessageBox.Show("Referance needs to be a number!", caption, MessageBoxButton.OK);
            }

            return output;
        }

        // *********************************************************
        private void WireUpChaptersCB()
        {
            cbChapter.Items.Clear();

            foreach (var c in Chapters)
            {
                cbChapter.Items.Add(c.ChapterName);
            }

            cbChapter.SelectedIndex = 0;
        }

        // ************************************************
        private void WireUpCuisineCB()
        {
            cbCuisine2.Items.Clear();

            // Get the Cuisine's
            Cuisine = SQLiteDataAccess.LoadCusines();

            foreach (var c in Cuisine)
            {
                cbCuisine2.Items.Add(c.CountryName);
            }
        }

        // *********************************************************
        private void WireUpRecipesLB()
        {
            lbRecipesList.Items.Clear();

            foreach (var r in Recipes)
            {
                string mark = (SQLiteDataAccess.CountIngListItem(r.RecipeId) == 0) ? "" : "(*)";
                lbRecipesList.Items.Add($"{LookupChapterID(r.ChapterFk)} - {r.RecipeTitle} {mark}");
            }

            lbRecipesList.SelectedIndex = 0;
        }
    }
}