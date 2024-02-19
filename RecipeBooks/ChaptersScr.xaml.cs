using RecipeBooks.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RecipeBooks
{
    /// <summary>
    /// Interaction logic for ChaptersScr.xaml
    /// </summary>
    public partial class ChaptersScr : Window
    {
        // ************************************************
        // ***     G L O B A L E   V A R I A B L E S
        private Joblog errorlog = new Joblog("Books", 1);

        private List<ChaptersModel> Chapters = new List<ChaptersModel>();

        // ************************************************
        public ChaptersScr(Joblog el)
        {
            errorlog = el;

            InitializeComponent();

            RefreshChapters();
        }

        // ************************************************
        private void AddChaptersButton(object sender, RoutedEventArgs e)
        {
            if (ValidateChaptersSrceen() == true)
            {
                ChaptersModel c = new ChaptersModel();

                c.ChapterName = tbChapter.Text.Trim();

                SQLiteDataAccess.AddChapter(c);

                // Refresh ListBox
                RefreshChapters();

                // Fibd out the new Chapter ID
                c.ChapterId = LookupChapter(c.ChapterName);

                errorlog.InformationMessage("Chapter Added - ", c.ToString());
            }
            else
            {
                MessageBox.Show("One or more of the fields are blank!", "Chapter Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ************************************************
        private void ClearChapterBoxes()
        {
            if (Chapters.Count != 0)
            {
                lblChapterLabel.Content = $"Chapters ({Chapters.Count})";
            }
            else
            {
                lblChapterLabel.Content = "Chapters";
            }

            lblChapterIdValue.Content = "";
            tbChapter.Text = "";
        }

        // ************************************************
        private void ClearChapterButton(object sender, RoutedEventArgs e)
        {
            RefreshChapters();
        }

        // ************************************************
        private void DeleteChapter()
        {
            int index = lbChaptersList.SelectedIndex;

            errorlog.InformationMessage("Chapter Deleted - ", Chapters[index].ToString());

            SQLiteDataAccess.DeleteChapter(Chapters[index].ChapterId);

            RefreshChapters();
        }

        // ************************************************
        private void DeleteChapterButton(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Deleting Chapter", "Recipe Books",
                                   MessageBoxButton.YesNo, MessageBoxImage.Hand);

            if (res == MessageBoxResult.Yes)
            {
                // Delete the part
                DeleteChapter();
            }

            RefreshChapters();
        }

        // ************************************************
        private void ExportChaptersButton(object sender, RoutedEventArgs e)
        {
            CSVFile.WriteChaptersFile(errorlog, Chapters);
        }

        // ************************************************
        private void ImportChaptersButton(object sender, RoutedEventArgs e)
        {
            List<ChaptersModel> SearchResults = new List<ChaptersModel>();

            int count = 0;

            Chapters.Clear();

            try
            {
                Chapters = CSVFile.ReadChaptersFile(errorlog);

                foreach (var c in Chapters)
                {
                    string q = $"select * from Chapters where ChapterTitle = \"{c.ChapterName}\"";
                    SearchResults = SQLiteDataAccess.SearchChapters(q);

                    count++;

                    if (SearchResults.Count == 0 || SearchResults == null)
                    {
                        SQLiteDataAccess.AddChapter(c);
                    }
                }
            }
            catch
            {
                string message = $"Failed to Import file. An error occured at line {count} of the file. Please check you have the correct columns and the correct value types.";

                MessageBox.Show(message, "Import Chapters", MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(message, "Import Chapters");
                return;
            }

            RefreshChapters();
        }

        // *********************************************************
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

        // *********************************************************
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
        private void RefreshChapters()
        {
            // Clear out old Chapters
            Chapters.Clear();

            // Get the Location
            Chapters = SQLiteDataAccess.LoadChapters();

            // Sort Location
            Chapters.Sort((x, y) => x.ChapterName.CompareTo(y.ChapterName));

            // reset the add field on the display
            ClearChapterBoxes();

            // Display the Locations in the ListBox & DropDown
            // Must be Refreshed after EVERYTHING else
            WireUpChaptersLB();

            lblChapterIdValue.Content = "";
        }

        // ************************************************
        private void ChapterSelectedListBox(object sender, SelectionChangedEventArgs e)
        {
            int index = lbChaptersList.SelectedIndex;

            if (index != -1)
            {
                // move data into the screen
                lblChapterIdValue.Content = $"{Chapters[index].ChapterId}/{index}";

                tbChapter.Text = Chapters[index].ChapterName.Trim();
            }
        }

        // ************************************************
        private void ChapterTextChanged(object sender, TextChangedEventArgs e)
        {
            if (btnAddChapter != null)
            {
                btnAddChapter.IsEnabled = true;

                if (tbChapter.Text != "")
                {
                    btnUpdateChapter.IsEnabled = true;
                    btnDeleteChapter.IsEnabled = true;

                    btnImportChapters.IsEnabled = false;
                    btnExportChapters.IsEnabled = false;
                }
                else
                {
                    btnAddChapter.IsEnabled = false;
                    btnUpdateChapter.IsEnabled = false;
                    btnDeleteChapter.IsEnabled = false;

                    btnImportChapters.IsEnabled = true;
                    btnExportChapters.IsEnabled = true;
                }
            }
        }

        // ************************************************
        private void UpdateChapterButton(object sender, RoutedEventArgs e)
        {
            if (ValidateChaptersSrceen() == true)
            {
                int index = lbChaptersList.SelectedIndex;

                errorlog.InformationMessage("Chapter Updated", "From: " + Chapters[index].ToString());

                Chapters[index].ChapterName = tbChapter.Text.Trim();

                errorlog.InformationMessage("", "To:" + Chapters[index].ToString());

                SQLiteDataAccess.UpdateChapter(Chapters[index]);

                // Refresh Chapters
                RefreshChapters();
            }
        }

        // ************************************************
        private bool ValidateChaptersSrceen()
        {
            bool output = true;
            string caption = "Chapters";

            if (tbChapter.Text == "")
            {
                output = false;
                MessageBox.Show("Chapter Title can not be blank!", caption, MessageBoxButton.OK);
            }

            return output;
        }

        // ************************************************
        private void WireUpChaptersLB()
        {
            lbChaptersList.Items.Clear();

            foreach (var c in Chapters)
            {
                string text = c.ListBoxText();

                lbChaptersList.Items.Add(text);
            }
        }
    }
}