using RecipeBooks.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

/*
* Title:    Recipe Book Program
* Author:   Kelvin Clements
* Date:     28-August-2023
* Purpose:  To catelogue Recipe Books
*
* LOG ------------------------------------------------------
* DD/MMM/YYYY   Comments ....................................
* ----------------------------------------------------------
* 28/Aug/2023 - Project started
* 28/Aug/2023 - Created Main Page
* 29/Aug/2023 - Added a Recipe Book Tab
* 30/Aug/2023 - Added a Book Import\Export Functions
* 31/Aug/2023 - Added a Chapter Tab
* 31/Aug/2023 - Added a Chapter Import\Export Functions
* 31/Aug/2023 - Added a Locations Tab
* 31/Aug/2023 - Added a Location Import\Export Functions
* 31/Aug/2023 - Added a Ingredient Tab
* 31/Aug/2023 - Added a Ingredient Import\Export Functions
* 31/Aug/2023 - Added Templates
* 30/Aug/2023 - Moved Chapters to a septerate Screen
* 30/Aug/2023 - Moved Ingredient to a septerate Screen
* 30/Aug/2023 - Moved Locatiuon to a septerate Screen
* 30/Aug/2023 - Added the General Modula
* 30/Aug/2023 - Added a Book Search Tab
* 18/Sep/2023 - Added Help.RTF
* 19/Sep/2023 - Added the Rcipe Contents Screen
* 19/Sep/2023 - Added a Recipe Search Tab
*             - Edit Recipe
*             - Added  Recipe Ingredient List
*             - Added  Recipe Directions\Method
*             - Added Cheifs Notes (Note1)
*             - Added My Notes (Note2)
*             - Added Converter Tab
* 19/Dec/2023 - Change all the Lookup methods to use .Find()
* 22/Dec/2023 - Added the Display FlowDocument
* 27/Dec/2023 - Change Search screens to have a search count
* 29/Dec/2023 - Added an Image to the FlowDocument
* 02/Jan/2024 - Added Options to the FlowDocument
* 03/Jan/2024 - Added in Recipe Quantity\QtyType (addition to Make & Serving)
* 11/Jan/2024 - Added a Nutrition Tabto the RecipeContents.
* 12/Jan/2024 - Allowed for convertion to metric in the Display of the recipe Quantity.
* 15/Jan/2024 - Added Links into the Recipe to point too connected recipes.
* 23/Jan/2024 - Added Purge To the Setting menu.
*/

namespace RecipeBooks
{
    public partial class MainWindow : Window
    {
        #region Gloable Varaiables

        // ************************************************
        // ************************************************
        // ***     G L O B A L E   V A R I A B L E S
        private Joblog errorlog = new Joblog("Books", 1);

        private List<BooksModel> Books = new List<BooksModel>();
        private List<LocationsModel> Locations = new List<LocationsModel>();
        private List<ChaptersModel> Chapters = new List<ChaptersModel>();
        private List<RecipesModel> Recipes = new List<RecipesModel>();

        private List<MediaTypeModel> MediaType = new List<MediaTypeModel>();
        private List<CountryModel> Cuisine = new List<CountryModel>();

        #endregion Gloable Varaiables

        // ************************************************
        // ************************************************
        // ***      M A I N   W I N D O W
        public MainWindow()
        {
            InitializeComponent();

            RefreshBooks();
            RefreshLocations();
            RefreshChapters();

            WireUpCuisineCB();
            WireUpMediaTypeCB();

            // Setup Book Search Screen
            lblSTitle.Content = $"Title Search: {Books.Count}";

            // Setup Recipe Search Screen
            int rcount = 0;

            foreach (var bk in Books)
            {
                rcount += SQLiteDataAccess.CountRecipes(bk.BookId);
            }

            lblSRecipe.Content = $"Recipe Search: {rcount}";
        }

        // ************************************************

        #region Book

        // ************************************************

        private void AddBookButton(object sender, RoutedEventArgs e)
        {
            if (ValidateBooksSrceen() == true)
            {
                BooksModel b = new BooksModel();

                b.BookTitle = tbTitle.Text.Trim();
                b.BookSubTitle = tbSubTitle.Text.Trim();
                b.Publisher = tbPublisher.Text.Trim();
                b.Author = tbAuthor.Text.Trim();
                b.Year = int.Parse(tbYear.Text.Trim());
                b.LocationFK = LookupLocation(cbLocation.Text);
                b.Copies = int.Parse(tbCopies.Text.Trim());
                b.Cuisine = LookupCuisine(cbCuisine.Text.Trim());
                b.MediaType = LookupMediaType(cbMediaType.Text.Trim());
                b.RecipeRef = 0;

                SQLiteDataAccess.AddBook(b);

                // Refresh ListBox
                RefreshBooks();

                errorlog.InformationMessage("Book Added - ", b.ToString());
            }
            else
            {
                MessageBox.Show("One or more of the fields are blank!", "Book Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ************************************************
        private void BookSelectedListBox(object sender, SelectionChangedEventArgs e)
        {
            int index = lbBooksList.SelectedIndex;

            if (index != -1)
            {
                // move data into the screen
                lblBookIdValue.Content = $"{Books[index].BookId}/{index}";

                tbTitle.Text = Books[index].BookTitle.Trim();
                tbSubTitle.Text = Books[index].BookSubTitle.Trim();
                tbPublisher.Text = Books[index].Publisher.Trim();
                tbAuthor.Text = Books[index].Author.Trim();
                tbYear.Text = Books[index].Year.ToString();
                cbLocation.Text = LookupLocationID(Books[index].LocationFK);
                tbCopies.Text = Books[index].Copies.ToString();
                cbCuisine.Text = LookupCuisineID(Books[index].Cuisine);
                cbMediaType.Text = LookupMediaTypeID(Books[index].MediaType);

                lblRCount.Content = SQLiteDataAccess.CountRecipes(Books[index].BookId).ToString();
                lblRRef.Content = Books[index].RecipeRef.ToString();
            }
        }

        // ************************************************
        private void BookTextChanged(object sender, TextChangedEventArgs e)
        {
            if (btnAddBook != null)
            {
                btnAddBook.IsEnabled = true;

                if (tbTitle.Text != "")
                {
                    btnClearBook.IsEnabled = true;
                    btnUpdateBook.IsEnabled = true;
                    btnDeleteBook.IsEnabled = true;
                    btnEditBook.IsEnabled = true;

                    btnImportBooks.IsEnabled = false;
                    btnExportBooks.IsEnabled = false;
                }
                else
                {
                    btnClearBook.IsEnabled = false;
                    btnUpdateBook.IsEnabled = false;
                    btnDeleteBook.IsEnabled = false;
                    btnEditBook.IsEnabled = false;

                    btnImportBooks.IsEnabled = true;
                    btnExportBooks.IsEnabled = true;
                }
            }
        }

        // ************************************************
        private void ClearBookBoxes()
        {
            lblBookLabel.Content = (Books.Count != 0) ? $"Books ({Books.Count})" : "Books";

            lblBookIdValue.Content = "";

            tbTitle.Text = "";
            tbSubTitle.Text = "";
            tbPublisher.Text = "";
            tbAuthor.Text = "";
            tbYear.Text = "";
            cbLocation.SelectedIndex = 0;
            lblRCount.Content = "";
            tbCopies.Text = "1";
            cbCuisine.SelectedIndex = LookupCuisine("United Kingdom");
            cbMediaType.SelectedIndex = 1;
            lblRRef.Content = "";
        }

        // ************************************************
        private void ClearBookButton(object sender, RoutedEventArgs e)
        {
            RefreshBooks();
        }

        // ************************************************
        private void DeleteBook()
        {
            int index = lbBooksList.SelectedIndex;
            int rCount = SQLiteDataAccess.CountRecipes(Books[index].BookId);

            MessageBoxResult res = MessageBox.Show($"Deleting this book with remove {rCount} of Recipes. Are you Really Sure?",
                                    "Recipe Books",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Hand);

            if (res == MessageBoxResult.Yes)
            {
                List<RecipesModel> BookRecipes = new List<RecipesModel>();

                // Build a List of Recipes
                Recipes.Clear();
                BookRecipes = SQLiteDataAccess.LoadRecipes(Books[index].BookId);

                // Remove each Recipe
                General.DeleteRecipes(errorlog, BookRecipes);

                // Remove the Book
                errorlog.InformationMessage("Book Deleted - ", Books[index].ToString());
                SQLiteDataAccess.DeleteBook(Books[index].BookId);

                RefreshBooks();
            }
        }

        // ************************************************
        private void DeleteBookButton(object sender, RoutedEventArgs e)
        {
            int index = lbBooksList.SelectedIndex;

            MessageBoxResult res = MessageBox.Show($"Deleting Book - {Books[index].FullTitle}. Are you Sure?", "Recipe Books",
                                    MessageBoxButton.YesNo, MessageBoxImage.Hand);

            if (res == MessageBoxResult.Yes)
            {
                DeleteBook();
            }
        }

        // ************************************************
        private void EditBookButton(object sender, RoutedEventArgs e)
        {
            int index = lbBooksList.SelectedIndex;

            if (index != -1)
            {
                new BookContents(errorlog, Books[index]).Show();

                int RCount = SQLiteDataAccess.CountRecipes(Books[index].BookId);

                lblRCount.Content = RCount.ToString();

                if (RCount != 0 && Books[index].RecipeRef == 0)
                {
                    // Get new ref
                    int maxValue = Books.Max(t => t.RecipeRef);
                    Books[index].RecipeRef = maxValue + 1;

                    SQLiteDataAccess.UpdateBook(Books[index]);
                }
            }
        }

        // ************************************************
        private void ExportBooksButton(object sender, RoutedEventArgs e)
        {
            string fileName = "";
            bool worked = CSVFile.GetExportFileName(out fileName);
            string[] contents = new string[Books.Count + 1];

            if (worked)
            {
                int i = 0;

                contents[i] = "BookId,BookTitle, BookSubTitle, Publisher, Author, Year, LocationFK, Copies, Cuisine, MediaType, RecipeRef";
                i++;

                foreach (var b in Books)
                {
                    contents[i] = $"{b.BookId}, {b.BookTitle}, {b.BookSubTitle} ," +
                        $" {b.Publisher}, {b.Author}, {b.Year}, {LookupLocationID(b.LocationFK)}," +
                        $" {b.Copies}, {b.Cuisine}, {b.MediaType}, {b.RecipeRef}";
                    i++;
                }

                System.IO.File.WriteAllLines(fileName, contents);

                string mesBoxTitle = "Books";
                string msgText = $"All Books Exported ({Books.Count})";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(msgText, mesBoxTitle);
            }
            else
            {
                string mesBoxTitle = "Books";
                string msgText = $"Export of Books Failed to open file";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                errorlog.WarningMessage(msgText, mesBoxTitle);
            }
        }

        // ************************************************
        private void ImportBooksButton(object sender, RoutedEventArgs e)
        {
            List<BooksModel> SearchResults = new List<BooksModel>();
            List<BookColumn> importBooks = new List<BookColumn>();

            Books = SQLiteDataAccess.LoadBooks();

            string fileName = "";
            bool worked = CSVFile.GetImportFileName(out fileName);

            if (worked)
            {
                importBooks = CSVFile.ReadBooksFile(fileName, "BookId");

                int count = 0;

                foreach (var ib in importBooks)
                {
                    int bfk = LookupBook(ib.c02, ib.c03);

                    if (bfk == -1) // -1 = not Found
                    {
                        try
                        {
                            count++;
                            BooksModel b = new BooksModel();

                            b.BookTitle = ib.c02.Trim();
                            b.BookSubTitle = ib.c03.Trim();
                            b.Publisher = ib.c04.Trim();
                            b.Author = ib.c05.Trim();

                            b.Year = (ib.c06.Trim() != null) ? int.Parse(ib.c06.Trim()) : b.Year = 0;

                            b.LocationFK = LookupLocation(ib.c07.Trim());

                            b.Copies = (ib.c08.Trim() != null) ? int.Parse(ib.c08.Trim()) : 0;

                            b.Cuisine = LookupCuisine(ib.c09.Trim());
                            b.MediaType = LookupMediaType(ib.c10.Trim());
                            b.RecipeRef = (ib.c11.Trim() != null) ? int.Parse(ib.c11.Trim()) : 0;

                            SQLiteDataAccess.AddBook(b);

                            errorlog.InformationMessage("Book Added - ", b.ToString());
                        }
                        catch
                        {
                            MessageBox.Show($"Failed to parse line {count} of import file.", "Books", MessageBoxButton.OK, MessageBoxImage.Information);
                            errorlog.InformationMessage($"Failed to parse line {count} of import file.", "Books");
                            break;
                        }
                    }
                }

                string mesBoxTitle = "Books";
                string msgText = $"Of {count} in the file, {importBooks.Count} where Imported.";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(msgText, mesBoxTitle);
            }
            else
            {
                string mesBoxTitle = "Books";
                string msgText = $"Imported of Books Failed to open file";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                errorlog.WarningMessage(msgText, mesBoxTitle);
            }

            RefreshBooks();
        }

        // ************************************************
        private int LookupBookRealID(int id)
        {
            for (int i = 0; i < Books.Count; i++)
            {
                if (Books[i].BookId == id)
                {
                    return i;
                }
            }

            return -1;
        }

        // ************************************************

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

        // ************************************************

        private int LookupBookFT(string ft)
        {
            var m = Books.Find(x => x.FullTitle == ft);

            if (m != null)
            {
                return m.BookId;
            }

            return -1;
        }

        // ************************************************
        private string LookupBookID(int id)
        {
            var m = Books.Find(x => x.BookId == id);

            if (m != null)
            {
                return m.FullTitle;
            }

            return "";
        }

        // ************************************************
        private string LookupBookYear(int id)
        {
            var m = Books.Find(x => x.BookId == id);

            if (m != null)
            {
                return m.Year.ToString();
            }

            return "";
        }

        // ************************************************
        private int LookupMediaType(string mt)
        {
            var m = MediaType.Find(x => x.MediaName == mt);

            if (m != null)
            {
                return m.MediaId;
            }

            return -1;
        }

        // ************************************************
        private string LookupMediaTypeID(int id)
        {
            var m = MediaType.Find(x => x.MediaId == id);

            if (m != null)
            {
                return m.MediaName;
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

        // ************************************************
        private void RefreshBooks()
        {
            // Clear out old Books
            Books.Clear();

            // Get the Books
            Books = SQLiteDataAccess.LoadBooks();

            // Sort Books
            Books.Sort((x, y) => x.FullTitle.CompareTo(y.FullTitle));

            // reset the add field on the display
            ClearBookBoxes();

            // Display the Books in the ListBox
            // Must be Refreshed after EVERYTHING else
            WireUpBooksLB();
        }

        // ************************************************
        private void RefreshLocations()
        {
            // Clear out old Locations
            Locations.Clear();

            // Get the Locations
            Locations = SQLiteDataAccess.LoadLocations();

            // Sort Locations
            Locations.Sort((x, y) => x.LocationTitle.CompareTo(y.LocationTitle));

            // Display the Locations in the ListBox
            // Must be Refreshed after EVERYTHING else
            WireUpLocationCB();
        }

        // ************************************************
        private void UpdateBookButton(object sender, RoutedEventArgs e)
        {
            if (ValidateBooksSrceen() == true)
            {
                int index = lbBooksList.SelectedIndex;

                if (index != -1)
                {
                    errorlog.InformationMessage("Book Updated", "From: " + Books[index].ToString());

                    Books[index].BookTitle = tbTitle.Text;
                    Books[index].BookSubTitle = tbSubTitle.Text;
                    Books[index].Publisher = tbPublisher.Text;
                    Books[index].Author = tbAuthor.Text;
                    Books[index].Year = int.Parse(tbYear.Text);
                    Books[index].LocationFK = LookupLocation(cbLocation.Text);
                    Books[index].Copies = int.Parse(tbCopies.Text.Trim());
                    Books[index].Cuisine = LookupCuisine(cbCuisine.Text.Trim());
                    Books[index].MediaType = LookupMediaType(cbMediaType.Text.Trim());

                    errorlog.InformationMessage("", "To:" + Books[index].ToString());

                    SQLiteDataAccess.UpdateBook(Books[index]);
                }

                // Refresh Books
                RefreshBooks();
            }
        }

        // ************************************************
        private bool ValidateBooksSrceen()
        {
            bool output = true;
            string caption = "Book";

            if (tbTitle.Text == "")
            {
                output = false;
                MessageBox.Show("Book Title can not be blank!", caption, MessageBoxButton.OK);
            }

            if (tbPublisher.Text == "")
            {
                output = false;
                MessageBox.Show("Publisher can not be blank!", caption, MessageBoxButton.OK);
            }

            if (tbAuthor.Text == "")
            {
                output = false;
                MessageBox.Show("Author can not be blank!", caption, MessageBoxButton.OK);
            }

            if (cbCuisine.Text == "")
            {
                output = false;
                MessageBox.Show("Cuisine can not be blank!", caption, MessageBoxButton.OK);
            }

            if (cbMediaType.Text == "")
            {
                output = false;
                MessageBox.Show("Media Type can not be blank!", caption, MessageBoxButton.OK);
            }

            return output;
        }

        // ************************************************
        private void WireUpBooksLB()
        {
            lbBooksList.Items.Clear();

            foreach (var b in Books)
            {
                string text = b.ListBoxText();

                lbBooksList.Items.Add(text);
            }
        }

        // ************************************************
        private void WireUpCuisineCB()
        {
            cbCuisine.Items.Clear();

            // Get the Cuisine's
            Cuisine = SQLiteDataAccess.LoadCusines();

            foreach (var c in Cuisine)
            {
                cbCuisine.Items.Add(c.CountryName);
            }
        }

        // ************************************************
        private void WireUpLocationCB()
        {
            cbLocation.Items.Clear();

            foreach (var l in Locations)
            {
                cbLocation.Items.Add(l.LocationTitle);
            }
        }

        // ************************************************
        private void WireUpMediaTypeCB()
        {
            cbMediaType.Items.Clear();

            // Get the MediaType's
            MediaType = SQLiteDataAccess.LoadMediaTypes();

            foreach (var c in MediaType)
            {
                cbMediaType.Items.Add(c.MediaName);
            }
        }

        // ************************************************

        private int LookupLocation(string loc)
        {
            var m = Locations.Find(x => x.LocationTitle == loc);

            if (m != null)
            {
                return m.LocationId;
            }

            return -1;
        }

        // ************************************************
        private string LookupLocationID(int id)
        {
            var m = Locations.Find(x => x.LocationId == id);

            if (m != null)
            {
                return m.LocationTitle;
            }

            return "";
        }

        #endregion Book

        // ************************************************

        #region BookSearch

        // ************************************************

        private void BookSearch_Changed()
        {
            List<BooksModel> QueryResults = new List<BooksModel>();
            List<BooksTable> query = new List<BooksTable>();

            string SearchQuery = "select * from Books where";

            // Re-Build the Query String
            if (tbBookSearch.Text != "")
            {
                SearchQuery += " BookTitle || BookSubTitle like \'%" + tbBookSearch.Text + "%\'";
            }

            if (SearchQuery != "select * from Books where")
            {
                // Dispolay the Querey
                lblBookSearchText.Content = SearchQuery;

                // Get the Search Results
                QueryResults = SQLiteDataAccess.SearchBooks(SearchQuery);

                if (QueryResults.Count > 0)
                {
                    foreach (var item in QueryResults)
                    {
                        BooksTable bdt = new BooksTable();

                        bdt.BId = item.BookId.ToString();
                        bdt.BTitle = item.FullTitle;
                        bdt.BPublisher = item.Publisher;
                        bdt.BAuthor = item.Author;
                        bdt.BYear = item.Year.ToString();
                        bdt.BLocation = LookupLocationID(item.LocationFK);

                        query.Add(bdt);
                    }
                }
            }

            lblSTitle.Content = (QueryResults.Count > 0) ? $"Title Search: {QueryResults.Count}/{Books.Count}" : $"Title Search: {Books.Count}";

            // Read the list and add to the query to display in the DataGrid
            dgBookSearch.ItemsSource = query;
        }

        // ************************************************
        private void DateGrid_BookSelected(object sender, SelectedCellsChangedEventArgs e)
        {
            int index = dgBookSearch.SelectedIndex;

            if (index >= -1)
            {
                DataGridRow row = (DataGridRow)dgBookSearch.ItemContainerGenerator.ContainerFromIndex(index);

                BooksTable drv = (BooksTable)dgBookSearch.SelectedItem;

                if (drv != null)
                {
                    tbMaster.SelectedItem = tiBooks;

                    lblBookIdValue.Content = drv.BId.Trim();
                    int ptr = LookupBookRealID(int.Parse(drv.BId.Trim()));

                    tbTitle.Text = Books[ptr].BookTitle.Trim();
                    tbSubTitle.Text = Books[ptr].BookSubTitle.Trim();
                    tbPublisher.Text = Books[ptr].Publisher.Trim();
                    tbAuthor.Text = Books[ptr].Author.Trim();
                    tbYear.Text = Books[ptr].Year.ToString();
                    cbLocation.SelectedValue = LookupLocationID(Books[ptr].LocationFK);
                    tbCopies.Text = Books[ptr].Copies.ToString();
                    cbCuisine.Text = LookupCuisineID(Books[ptr].Cuisine);
                    cbMediaType.Text = LookupMediaTypeID(Books[ptr].MediaType);

                    lblRCount.Content = SQLiteDataAccess.CountRecipes(Books[index].BookId).ToString();
                    lblRRef.Content = Books[ptr].RecipeRef.ToString();
                }
            }
        }

        // ************************************************
        private void tbSearchBook_Changed(object sender, TextChangedEventArgs e)
        {
            BookSearch_Changed();
        }

        #endregion BookSearch

        // ************************************************

        #region RecipeSearch

        // ************************************************

        private void RecipeSearch_Changed()
        {
            List<RecipesModel> QueryResults = new List<RecipesModel>();
            List<RecipeTable> query = new List<RecipeTable>();

            string SearchQuery = "select * from Recipes where";

            // Re-Build the Query String
            if (tbRecipeSearch.Text != "")
            {
                SearchQuery += " RecipeTitle like \'%" + tbRecipeSearch.Text + "%\'";
            }

            if (SearchQuery != "select * from Recipes where")
            {
                // Dispolay the Querey
                lblRSearchText.Content = SearchQuery;

                // Get the Search Results
                QueryResults = SQLiteDataAccess.SearchRecipes(SearchQuery);

                if (QueryResults.Count > 0)
                {
                    // convert Foreign Keys to Text
                    foreach (var item in QueryResults)
                    {
                        RecipeTable rdt = new RecipeTable();

                        rdt.RId = item.RecipeId.ToString();
                        rdt.RBookTitle = LookupBookID(item.BookFk);
                        rdt.RYear = LookupBookYear(item.BookFk);
                        rdt.RTitle = item.RecipeTitle;
                        rdt.RChapter = LookupChapterID(item.ChapterFk);

                        query.Add(rdt);
                    }
                }
            }

            int rcount = 0;
            foreach (var bk in Books)
            {
                rcount += SQLiteDataAccess.CountRecipes(bk.BookId);
            }

            lblSTitle.Content = $"Book Search: {rcount}";

            lblSRecipe.Content = (QueryResults.Count > 0) ? $"Recipe Search: {QueryResults.Count} of {rcount}" : $"Recipe Search: {rcount}";

            // Read the list and add to the query to display in the DataGrid
            dgRecipeSearch.ItemsSource = query;
        }

        // ************************************************
        public string LookupChapterID(int id)
        {
            for (int i = 0; i < Chapters.Count; i++)
            {
                if (Chapters[i].ChapterId == id)
                {
                    return Chapters[i].ChapterName;
                }
            }

            return "";
        }

        // ************************************************

        private void tbSearchRecipe_Changed(object sender, TextChangedEventArgs e)
        {
            RecipeSearch_Changed();
        }

        // ************************************************
        private void DateGrid_RecipeSelected(object sender, SelectedCellsChangedEventArgs e)
        {
            int index = dgRecipeSearch.SelectedIndex;

            if (index != -1)
            {
                RefreshBookRecipes();

                DataGridRow row = (DataGridRow)dgRecipeSearch.ItemContainerGenerator.ContainerFromIndex(index);
                RecipeTable drv = (RecipeTable)dgRecipeSearch.SelectedItem;

                if (drv != null)
                {
                    new RecipeContents(errorlog, Recipes[LookupRecipeID(drv.RTitle)]).Show();
                }
            }
        }

        // ************************************************
        private int LookupRecipeID(string rcp)
        {
            for (int i = 0; i < Recipes.Count; i++)
            {
                if (Recipes[i].RecipeTitle == rcp)
                {
                    return i;
                }
            }

            return -1;
        }

        // ************************************************

        private void RefreshChapters()
        {
            // Clear out old Chapters
            Chapters.Clear();

            // Get the Location
            Chapters = SQLiteDataAccess.LoadChapters();
        }

        // ************************************************
        private void ImportRecipesButton(object sender, RoutedEventArgs e)
        {
            List<RecipesModel> SearchResults = new List<RecipesModel>();
            List<RecipeColumn> importRecipes = new List<RecipeColumn>();

            RefreshBookRecipes();

            // Read the file and check if its a new Book/Recipe
            string fileName = "";
            bool worked = CSVFile.GetImportFileName(out fileName);

            if (worked)
            {
                importRecipes = CSVFile.ReadRecipesFile(fileName, "RecipeId");

                int count = 0;

                foreach (var ir in importRecipes)
                {
                    int bfk = LookupBookFT(ir.c02);

                    if (bfk != -1) // Book Found
                    {
                        int rfk = LookupRecipeName(bfk, ir.c03.Trim());

                        if (rfk == -1) // Recipe not Found
                        {
                            try
                            {
                                count++;
                                RecipesModel r = new RecipesModel();

                                r.BookFk = bfk;
                                r.RecipeTitle = ir.c03.Trim();
                                r.ChapterFk = LookupChapter(ir.c04.Trim());
                                r.Referance = int.Parse(ir.c05);
                                r.Makes = int.Parse(ir.c06);
                                r.Serving = int.Parse(ir.c07);
                                r.Quantity = float.Parse(ir.c08);
                                r.QtyType = int.Parse(ir.c09);
                                r.Preparation = ir.c10.Trim();
                                r.Cooking = ir.c11.Trim();
                                r.Cuisine = LookupCuisine(ir.c12);
                                r.Favourite = bool.Parse(ir.c13);
                                r.Image = ir.c14.Trim();

                                if (!RecipeExist(r))
                                {
                                    SQLiteDataAccess.AddRecipe(r);
                                    errorlog.InformationMessage("Recipe Added - ", r.ToString());

                                    // TODO: Test this
                                    // Look to see if there is a recipe file in the import directory
                                    // to import
                                    General.ImportRecipeDetails(errorlog, r, fileName);
                                }
                            }
                            catch
                            {
                                MessageBox.Show($"Failed to parse line {count} of import file.", "Import Recipes", MessageBoxButton.OK, MessageBoxImage.Information);
                                errorlog.InformationMessage($"Failed to parse line {count} of import file.", "Import Recipes");
                                break;
                            }
                        }
                        else
                        {
                            errorlog.InformationMessage($"Recipe Referance {rfk} already exists.", "Import Recipes");
                        }
                    }
                    else
                    {
                        errorlog.InformationMessage($"Book Referance {bfk} does not exist.", "Import Recipes");
                    }
                }

                string mesBoxTitle = "Recipes";
                string msgText = $"Of {importRecipes.Count} in file, {count} Recipes Imported.";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(msgText, mesBoxTitle);
            }
            else
            {
                string mesBoxTitle = "Recipes";
                string msgText = $"Imported of Recipe Failed to open file";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                errorlog.WarningMessage(msgText, mesBoxTitle);
            }

            // Update the Totel Number of Recipes in the Search Window
            int rcount = 0;
            // Update the Recipe Count in Each Book
            foreach (var bk in Books)
            {
                rcount += SQLiteDataAccess.CountRecipes(bk.BookId);
            }

            lblSRecipe.Content = (rcount > 0) ? $"Recipe Search: {rcount}" : $"Recipe Search:";

            // Re-Build the Book List
            RefreshBooks();
        }

        // ************************************************
        private void RefreshBookRecipes()
        {
            List<RecipesModel> BookRecipes = new List<RecipesModel>();

            Books = SQLiteDataAccess.LoadBooks();

            // Build a full List of Recipes
            Recipes.Clear();

            foreach (var bk in Books)
            {
                BookRecipes = SQLiteDataAccess.LoadRecipes(bk.BookId);
                Recipes.AddRange(BookRecipes);
            }
        }

        // ************************************************
        private void ExportRecipesButton(object sender, RoutedEventArgs e)
        {
            string fileName = "";
            bool worked = CSVFile.GetExportFileName(out fileName);

            RefreshBookRecipes();

            string[] contents = new string[Recipes.Count + 1];

            if (worked)
            {
                int i = 0;

                contents[i] = "RecipeId,FullTitle,RecipeTitle,ChapterFk,Referance,Makes,Serving,Quantity, QtyType,Preparation,Cooking,Type,Cuisine,Favourite, Image";
                i++;

                foreach (var r in Recipes)
                {
                    contents[i] = $"{r.RecipeId}, {LookupBookID(r.BookFk)}," +
                        $" {r.RecipeTitle}, {LookupChapterID(r.ChapterFk)}, {r.Referance}," +
                        $" {r.Makes}, {r.Serving}, {r.Quantity}, {General.LookupMTypeID(r.QtyType)}," +
                        $" {r.Preparation}, {r.Cooking}, {r.Cuisine}, {r.Favourite}, {r.Image}";
                    i++;

                    General.ExportRecipeDetails(errorlog, r, fileName);
                }

                System.IO.File.WriteAllLines(fileName, contents);

                string mesBoxTitle = "Recipes";
                string msgText = $"All Recipes Exported ({Recipes.Count})";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(msgText, mesBoxTitle);
            }
            else
            {
                string mesBoxTitle = "Recipes";
                string msgText = $"Export of Recipe Failed to open file";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                errorlog.WarningMessage(msgText, mesBoxTitle);
            }
        }

        // ************************************************
        private int LookupRecipeName(int bfk, string rt)
        {
            var m = Recipes.Find(x => x.RecipeTitle == rt);

            if (m != null)
            {
                return m.RecipeId;
            }

            return -1;
        }

        // ************************************************
        private int LookupChapter(string loc)
        {
            for (int i = 0; i < Chapters.Count; i++)
            {
                if (Chapters[i].ChapterName == loc)
                {
                    return Chapters[i].ChapterId;
                }
            }

            return -1;
        }

        // ************************************************
        private bool RecipeExist(RecipesModel rec)
        {
            foreach (var r in Recipes)
            {
                if (r == rec)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion RecipeSearch

        // ************************************************

        #region General

        // ************************************************
        private void ExitButton(object sender, RoutedEventArgs e)
        {
            errorlog.InformationMessage("Application Ending ...", "Someone pressed the Exit options.");

            Application.Current.Shutdown();
        }

        // ************************************************
        private void HelpButton(object sender, RoutedEventArgs e)
        {
            new AboutHelp().Show();
        }

        // ************************************************
        private void LocationsButton(object sender, RoutedEventArgs e)
        {
            new LocationsScr(errorlog).Show();
            RefreshLocations();
        }

        // ************************************************
        private void ChaptersButton(object sender, RoutedEventArgs e)
        {
            new ChaptersScr(errorlog).Show();
        }

        // ************************************************
        private void IngredientsButton(object sender, RoutedEventArgs e)
        {
            new IngredientsScr(errorlog).Show();
        }

        // ************************************************
        private void PurgeButton(object sender, RoutedEventArgs e)
        {
            new PurgeScr(errorlog).ShowDialog();

            RefreshBooks();
            RefreshLocations();
            RefreshChapters();
        }

        // ************************************************
        private void IntValidation(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            General.IntValidation(sender, e);
        }

        // ************************************************
        private void FloatValidation(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            General.FloatValidation(sender, e);
        }

        // ************************************************

        #endregion General

        // ************************************************
    }
}