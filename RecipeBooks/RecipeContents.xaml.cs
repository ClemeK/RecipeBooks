using Microsoft.Win32;
using RecipeBooks.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RecipeBooks
{
    public partial class RecipeContents : Window
    {
        private List<ChaptersModel> Chapters = new();
        private List<CountryModel> Cuisine = new List<CountryModel>();
        private RecipesModel CurrentRecipe = new();
        private Joblog errorlog = new Joblog("Books", 7);
        private List<IngListItemModel> IngList = new();
        private List<IngredientsModel> Ingredients = new();
        private List<LinksModel> Links = new();
        private List<DirectionsModel> Directions = new();
        private List<Note1sModel> Note1s = new();
        private List<Note2sModel> Note2s = new();
        private List<MTypeModel> MType = new List<MTypeModel>();

        private List<RecipesModel> Recipes = new();

        // ************************************************
        // ***   R e c i p e   C o n t e n t s   W I N D O W
        public RecipeContents(Joblog el, RecipesModel recipe)
        {
            InitializeComponent();

            errorlog = el;
            CurrentRecipe = recipe;

            lblRecipeTitle.Content = $"{recipe.RecipeTitle}";

            lblRecipeType.Content = String.Empty;

            RefreshChapter();
            RefreshIngredients();

            WireUpLinkCB();
            WireUpCuisineCB();
            WireUpMTypeCB();

            WireUpBasics();

            if (CurrentRecipe.Image != "")
            {
                tbImage.Text = Path.GetFileName(CurrentRecipe.Image);
                DisplayImage(CurrentRecipe.Image);
            }

            ClearIngredientInputField();
            RefreshIngItemListDG();

            ClearLinkField();
            RefreshLinkDG();

            ClearDirectionField();
            RefreshDirectionDG();

            ClearNote1Field();
            RefreshNote1sDG();

            ClearNote2Field();
            RefreshNote2sDG();

            UpdateNutrition();

            UpdateRTFBox();
        }

        // ************************************************

        #region Basic

        // ************************************************
        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            string fileName = CurrentRecipe.Image;

            bool worked = GetImageFileName(ref fileName);

            if (worked)
            {
                tbImage.Text = fileName;
                DisplayImage(fileName);
                SQLiteDataAccess.UpdateRecipe(CurrentRecipe);
            }
        }

        // ************************************************
        private void ClearRecipeBoxes()
        {
            lblRecipeIdValue.Content = "";

            cbChapter2.SelectedIndex = 0;
            cbCuisine3.SelectedIndex = CurrentRecipe.Cuisine;
            tbReferance.Text = string.Empty;
            ckbFavourite.IsChecked = false;

            tbMakes.Text = "1";
            tbServing.Text = "1";
            tbQuantity.Text = "0";
            cbQtyType.Text = string.Empty;

            tbPreparation.Text = "0";
            tbCooking.Text = "0";

            tbImage.Text = string.Empty;
        }

        // ************************************************
        private void ClearRecipeButton(object sender, RoutedEventArgs e)
        {
            ClearRecipeBoxes();
        }

        // ************************************************
        private int LookupChapter(string loc)
        {
            var m = Chapters.Find(x => x.ChapterName == loc);

            if (m != null)
            {
                return m.ChapterId;
            }

            return -1;
        }

        // ************************************************
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

        // ************************************************
        private void RefreshChapter()
        {
            // Get the Chapters
            Chapters = SQLiteDataAccess.LoadChapters();

            // Sort the Chapters
            Chapters.Sort((x, y) => x.ChapterName.CompareTo(y.ChapterName));

            // Display the Chapters in the ComboBox
            WireUpChaptersCB();
        }

        // ************************************************
        private void WireUpChaptersCB()
        {
            cbChapter2.Items.Clear();

            foreach (var c in Chapters)
            {
                cbChapter2.Items.Add(c.ChapterName);
            }

            cbChapter2.SelectedIndex = 0;
        }

        // ************************************************
        private void WireUpCuisineCB()
        {
            cbCuisine3.Items.Clear();

            // Get the Cuisine's
            Cuisine = SQLiteDataAccess.LoadCusines();

            foreach (var c in Cuisine)
            {
                cbCuisine3.Items.Add(c.CountryName);
            }
        }

        // ************************************************

        #endregion Basic

        // ************************************************

        #region Ingredients

        // ************************************************
        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (tbQyt.Text != "")
            {
                IngListItemModel il = new();

                il.RecipeFK = CurrentRecipe.RecipeId;

                il.Quantity = (float.TryParse(tbQyt.Text.Trim(), out float value)) ? value : 0.0f;

                il.MType = General.LookupMType(cbMType.Text);

                il.IngredientFK = General.LookupIngredientName(cbIng.Text, Ingredients);

                SQLiteDataAccess.AddIngListItem(il);

                errorlog.InformationMessage("Recipe Ingredient Added - ", il.ToString());
            }

            RefreshIngItemListDG();

            // Clear input Fields
            ClearIngredientInputField();
        }

        // ************************************************
        private void ClearIngredientInputField()
        {
            tbQyt.Text = string.Empty;
            cbMType.SelectedIndex = 0;
            cbIng.Text = string.Empty;
        }

        // ************************************************
        private void DeleteIngredient_Click(object sender, RoutedEventArgs e)
        {
            Int32 selectedRowCount = dgIngredientSearch.SelectedIndex;

            if (selectedRowCount > -1 && IngList.Count > 0)
            {
                errorlog.InformationMessage("Recipe Ingredient Deleted - ", IngList[selectedRowCount].IngredientFK.ToString());

                SQLiteDataAccess.DeleteIngListItem(CurrentRecipe.RecipeId, IngList[selectedRowCount].IngredientFK);
            }

            RefreshIngItemListDG();
        }

        // ************************************************
        private float LookupIngredientMass(int id)
        {
            var i = Ingredients.Find(x => x.IngredientId == id);

            return (i != null) ? i.Mass : 0.0f;
        }

        // ************************************************
        private void RefreshIngItemListDG()
        {
            List<IngListItemModel> QueryResults = new List<IngListItemModel>();
            List<IngListItemTable> tableQuery = new List<IngListItemTable>();

            // Get the Search Results
            QueryResults = SQLiteDataAccess.LoadIngListItem(CurrentRecipe.RecipeId);

            if (QueryResults.Count > 0)
            {
                // convert Foreign Keys to Text
                foreach (var item in QueryResults)
                {
                    IngListItemTable ing = new IngListItemTable();

                    ing.IQty = item.Quantity.ToString();
                    ing.IMType = General.LookupMTypeID(item.MType);
                    ing.IIngredient = General.LookupIngredientID(item.IngredientFK, Ingredients);
                    tableQuery.Add(ing);
                }
            }

            lblRecipeType.Content = (QueryResults.Count > 0) ? RecipeType(QueryResults) + " Dish" : "";

            // Read the list and add to the query to display in the DataGrid
            dgIngredientSearch.ItemsSource = tableQuery;

            // Update the Converter Tab
            RefreshConvertListDG();

            // Update the Display tab
            UpdateRTFBox();
        }

        // ************************************************
        private void RefreshIngredients()
        {
            // Clear out old Ingredients
            Ingredients.Clear();

            // Get the Ingredients
            Ingredients = SQLiteDataAccess.LoadIngredients();

            // Sort Location
            Ingredients.Sort((x, y) => x.IngredientName.CompareTo(y.IngredientName));

            // Display the Locations in the ListBox & DropDown
            // Must be Refreshed after EVERYTHING else
            WireUpIngredientsCB();
        }

        // ************************************************
        private void WireUpIngredientsCB()
        {
            cbIng.Items.Clear();

            foreach (var i in Ingredients)
            {
                cbIng.Items.Add($"{i.IngredientName}");
            }
        }

        // ************************************************
        private void WireUpMTypeCB()
        {
            cbMType.Items.Clear();
            cbQtyType.Items.Clear();

            MType = SQLiteDataAccess.LoadMTypes();

            foreach (var m in MType)
            {
                cbMType.Items.Add(m.MTypeName);
                cbQtyType.Items.Add(m.MTypeName);
            }
        }

        // ************************************************

        #endregion Ingredients

        // ************************************************

        #region Convert

        // *********************************************************
        private void RefreshConvertListDG()
        {
            List<IngListItemModel> IngList = new List<IngListItemModel>();
            List<ConvertTable> query = new List<ConvertTable>();

            // Populate the List (Incase its changed)
            IngList = SQLiteDataAccess.LoadIngListItem(CurrentRecipe.RecipeId);

            if (IngList.Count > 0)
            {
                // convert Foreign Keys to Text
                foreach (var item in IngList)
                {
                    ConvertTable con = new ConvertTable();

                    if (MTypeConvertable(item.MType))
                    {
                        // Is Convertable
                        float mass = LookupIngredientMass(item.IngredientFK);

                        if (mass > 0.0f)
                        {
                            float temp = ((item.Quantity * ConvertWith(item.MType) / 100) * mass);

                            con.CQty = (temp).ToString("#,##0.##");
                            con.CMType = ConvertTo(CurrentRecipe.QtyType);
                            con.CIngredient = General.LookupIngredientID(item.IngredientFK, Ingredients);
                        }
                        else
                        {
                            con.CQty = (item.Quantity * ConvertWith(item.MType)).ToString("#,##0.##");
                            con.CMType = ConvertTo(item.MType);

                            var ing = General.LookupIngredient(item.IngredientFK, Ingredients);
                            string star = (ing.NutientInfo()) ? "" : " (*)";

                            con.CIngredient = General.LookupIngredientID(item.IngredientFK, Ingredients) + star;
                        }
                    }
                    else
                    {
                        // Not Convertable
                        // don't change (-, Small, Medium, Large)
                        con.CQty = (item.Quantity).ToString("#,##0.##");
                        con.CMType = General.LookupMTypeID(item.MType);
                        con.CIngredient = General.LookupIngredientID(item.IngredientFK, Ingredients);
                    }

                    query.Add(con);
                }
            }

            // Read the list and add to the query to display in the DataGrid
            dgConvertSearch.ItemsSource = query;
        }

        // *********************************************************

        #endregion Convert

        // ************************************************

        #region Directions

        // *********************************************************
        private void AddDirection_Click(object sender, RoutedEventArgs e)
        {
            if (tbDirection.Text != "")
            {
                DirectionsModel dir = new();

                dir.RecipeFK = CurrentRecipe.RecipeId;

                dir.Position = (int.TryParse(tbPosition.Text.Trim(), out int value)) ? value : 0;

                dir.Direction = tbDirection.Text.Trim();

                SQLiteDataAccess.AddDirection(dir);

                errorlog.InformationMessage("Recipe Direction Added - ", dir.ToString());
            }

            RefreshDirectionDG();

            ClearDirectionField();
        }

        // *********************************************************
        private void ClearDirectionField()
        {
            tbPosition.Text = (Directions.Count + 1).ToString();
            tbDirection.Text = string.Empty;
        }

        // *********************************************************
        private void DeleteDirection_Click(object sender, RoutedEventArgs e)
        {
            Int32 selectedRowCount = dgDirectionSearch.SelectedIndex;

            if (selectedRowCount > -1 && Directions.Count > 0)
            {
                SQLiteDataAccess.DeleteDirections(CurrentRecipe.RecipeId, Directions[selectedRowCount].DirectId);
                Directions.RemoveAt(selectedRowCount);
            }

            RePositionDirections();
            RefreshDirectionDG();
            ClearDirectionField();
        }

        // *********************************************************
        private void DownDirection_Click(object sender, RoutedEventArgs e)
        {
            DirectionsModel temp = new();

            Int32 selectedRowCount = dgDirectionSearch.SelectedIndex;

            if (selectedRowCount < Directions.Count && Directions.Count > 1)
            {
                if (selectedRowCount != (Directions.Count - 1))
                {
                    temp = Directions[selectedRowCount];
                    Directions.RemoveAt(selectedRowCount);

                    selectedRowCount++;

                    Directions.Insert(selectedRowCount, temp);
                }
            }

            RePositionDirections();
            RefreshDirectionDG();
            ClearDirectionField();
        }

        // *********************************************************
        private void UpDirection_Click(object sender, RoutedEventArgs e)
        {
            DirectionsModel temp = new();

            Int32 selectedRowCount = dgDirectionSearch.SelectedIndex;

            if (selectedRowCount > 0 && Directions.Count > 0)
            {
                temp = Directions[selectedRowCount];
                Directions.RemoveAt(selectedRowCount);

                selectedRowCount--;

                Directions.Insert(selectedRowCount, temp);
            }

            RePositionDirections();
            RefreshDirectionDG();
            ClearDirectionField();
        }

        // *********************************************************
        private void RePositionDirections()
        {
            int p = 0;

            foreach (var d in Directions)
            {
                p++;

                d.Position = p;
                SQLiteDataAccess.UpdateDirection(d);
            }
        }

        // *********************************************************
        private void RefreshDirectionDG()
        {
            List<DirectionsTable> query = new List<DirectionsTable>();

            // Get the Updated Directions
            Directions = SQLiteDataAccess.LoadDirections(CurrentRecipe.RecipeId);

            // Sort the Directions
            Directions.Sort((x, y) => x.Position.CompareTo(y.Position));

            if (Directions.Count > 0)
            {
                // convert Foreign Keys to Text
                foreach (var item in Directions)
                {
                    DirectionsTable dir = new DirectionsTable();

                    dir.DPos = item.Position.ToString();
                    dir.DDirection = item.Direction;

                    query.Add(dir);
                }
            }

            // Read the list and add to the query to display in the DataGrid
            dgDirectionSearch.ItemsSource = query;

            // Update the Display Tab
            UpdateRTFBox();
        }

        // *********************************************************

        #endregion Directions

        // ************************************************

        #region Note1

        // *********************************************************
        private void AddNote1_Click(object sender, RoutedEventArgs e)
        {
            if (tbNote1.Text != "")
            {
                Note1sModel note = new();

                note.RecipeFK = CurrentRecipe.RecipeId;

                note.Note1Nbr = (int.TryParse(tbNote1Nbr.Text.Trim(), out int value)) ? value : 0;

                note.Note1 = tbNote1.Text.Trim();

                SQLiteDataAccess.AddNote1(note);

                errorlog.InformationMessage("Recipe Note1 Added - ", note.ToString());
            }

            RefreshNote1sDG();

            ClearNote1Field();
        }

        // *********************************************************

        private void ClearNote1Field()
        {
            tbNote1Nbr.Text = (Note1s.Count + 1).ToString();
            tbNote1.Text = string.Empty;
        }

        // *********************************************************

        private void DeleteNote1_Click(object sender, RoutedEventArgs e)
        {
            Int32 selectedRowCount = dgNote1Search.SelectedIndex;

            if (selectedRowCount > -1 && Note1s.Count > 0)
            {
                SQLiteDataAccess.DeleteNote1s(CurrentRecipe.RecipeId, Note1s[selectedRowCount].Note1Id);
                Note1s.RemoveAt(selectedRowCount);
            }

            RePositionNote1s();
            RefreshNote1sDG();
            ClearNote1Field();
        }

        // *********************************************************

        private void UpNote1_Click(object sender, RoutedEventArgs e)
        {
            Note1sModel temp = new();

            Int32 selectedRowCount = dgNote1Search.SelectedIndex;

            if (selectedRowCount > 0 && Note1s.Count > 0)
            {
                temp = Note1s[selectedRowCount];
                Note1s.RemoveAt(selectedRowCount);

                selectedRowCount--;

                Note1s.Insert(selectedRowCount, temp);
            }

            RePositionNote1s();
            RefreshNote1sDG();
            ClearNote1Field();
        }

        // *********************************************************

        private void DownNote1_Click(object sender, RoutedEventArgs e)
        {
            Note1sModel temp = new();

            Int32 selectedRowCount = dgNote1Search.SelectedIndex;

            if (selectedRowCount < Note1s.Count && Note1s.Count > 1)
            {
                if (selectedRowCount != (Note1s.Count - 1))
                {
                    temp = Note1s[selectedRowCount];
                    Note1s.RemoveAt(selectedRowCount);

                    selectedRowCount++;

                    Note1s.Insert(selectedRowCount, temp);
                }
            }

            RePositionNote1s();
            RefreshNote1sDG();
            ClearNote1Field();
        }

        // *********************************************************
        private void RePositionNote1s()
        {
            int p = 0;

            foreach (var n1 in Note1s)
            {
                p++;

                n1.Note1Nbr = p;
                SQLiteDataAccess.UpdateNote1(n1);
            }
        }

        // *********************************************************
        private void RefreshNote1sDG()
        {
            List<NotesTable> query = new List<NotesTable>();

            // Get the Updated Note1s
            Note1s.Clear();
            Note1s = SQLiteDataAccess.LoadNote1s(CurrentRecipe.RecipeId);

            // Sort the Note1s
            Note1s.Sort((x, y) => x.Note1Nbr.CompareTo(y.Note1Nbr));

            if (Note1s.Count > 0)
            {
                // convert Foreign Keys to Text
                foreach (var item in Note1s)
                {
                    NotesTable N = new NotesTable();

                    N.NNbr = item.Note1Nbr.ToString();
                    N.NNote = item.Note1;

                    query.Add(N);
                }
            }

            // Read the list and add to the query to display in the DataGrid
            dgNote1Search.ItemsSource = query;

            // Update the Display Tab
            UpdateRTFBox();
        }

        // *********************************************************

        #endregion Note1

        // ************************************************

        #region Note2

        // *********************************************************
        private void AddNote2_Click(object sender, RoutedEventArgs e)
        {
            if (tbNote2.Text != "")
            {
                Note2sModel note = new();

                note.RecipeFK = CurrentRecipe.RecipeId;

                note.Note2Nbr = (int.TryParse(tbNote2Nbr.Text.Trim(), out int value)) ? value : 0;

                note.Note2 = tbNote2.Text.Trim();

                SQLiteDataAccess.AddNote2(note);

                errorlog.InformationMessage("Recipe Note2 Added - ", note.ToString());
            }

            RefreshNote2sDG();

            ClearNote2Field();
        }

        // *********************************************************

        private void ClearNote2Field()
        {
            tbNote2Nbr.Text = (Note2s.Count + 1).ToString();
            tbNote2.Text = string.Empty;
        }

        // *********************************************************

        private void DeleteNote2_Click(object sender, RoutedEventArgs e)
        {
            Int32 selectedRowCount = dgNote2Search.SelectedIndex;

            if (selectedRowCount > -1 && Note2s.Count > 0)
            {
                SQLiteDataAccess.DeleteNote2s(CurrentRecipe.RecipeId, Note2s[selectedRowCount].Note2Id);
                Note2s.RemoveAt(selectedRowCount);
            }

            RePositionNote2s();
            RefreshNote2sDG();
            ClearNote2Field();
        }

        // *********************************************************

        private void UpNote2_Click(object sender, RoutedEventArgs e)
        {
            Note2sModel temp = new();

            Int32 selectedRowCount = dgNote2Search.SelectedIndex;

            if (selectedRowCount > 0 && Note2s.Count > 0)
            {
                temp = Note2s[selectedRowCount];
                Note2s.RemoveAt(selectedRowCount);

                selectedRowCount--;

                Note2s.Insert(selectedRowCount, temp);
            }

            RePositionNote2s();
            RefreshNote2sDG();
            ClearNote2Field();
        }

        // *********************************************************

        private void DownNote2_Click(object sender, RoutedEventArgs e)
        {
            Note2sModel temp = new();

            Int32 selectedRowCount = dgNote2Search.SelectedIndex;

            if (selectedRowCount < Note2s.Count && Note2s.Count > 1)
            {
                if (selectedRowCount != (Note2s.Count - 1))
                {
                    temp = Note2s[selectedRowCount];
                    Note2s.RemoveAt(selectedRowCount);

                    selectedRowCount++;

                    Note2s.Insert(selectedRowCount, temp);
                }
            }

            RePositionNote2s();
            RefreshNote2sDG();
            ClearNote2Field();
        }

        // *********************************************************
        private void RePositionNote2s()
        {
            int p = 0;

            foreach (var n2 in Note2s)
            {
                p++;

                n2.Note2Nbr = p;
                SQLiteDataAccess.UpdateNote2(n2);
            }
        }

        // *********************************************************
        private void RefreshNote2sDG()
        {
            List<NotesTable> query = new List<NotesTable>();

            // Get the Updated Note2s
            Note2s.Clear();
            Note2s = SQLiteDataAccess.LoadNote2s(CurrentRecipe.RecipeId);

            // Sort the Note2s
            Note2s.Sort((x, y) => x.Note2Nbr.CompareTo(y.Note2Nbr));

            if (Note2s.Count > 0)
            {
                // convert Foreign Keys to Text
                foreach (var item in Note2s)
                {
                    NotesTable N = new NotesTable();

                    N.NNbr = item.Note2Nbr.ToString();
                    N.NNote = item.Note2;

                    query.Add(N);
                }
            }

            // Read the list and add to the query to display in the DataGrid
            dgNote2Search.ItemsSource = query;

            // Update the Display Tab
            UpdateRTFBox();
        }

        // *********************************************************

        #endregion Note2

        // ************************************************

        #region Display

        // *********************************************************
        private void Display_Checked(object sender, RoutedEventArgs e)
        {
            UpdateRTFBox();
        }

        // *********************************************************
        private void UpdateRTFBox()
        {
            if (rtfDetail != null)
            {
                rtfDetail.Document.Blocks.Clear();

                FlowDocument myFlowDoc = new FlowDocument();

                // Load Heading
                Paragraph HeadingText = new Paragraph();
                Bold heading = new Bold(new Run($"{CurrentRecipe.RecipeTitle}"));
                heading.FontSize = 20;
                HeadingText.Inlines.Add(heading);
                HeadingText.TextAlignment = TextAlignment.Center;
                myFlowDoc.Blocks.Add(HeadingText);

                // Load Picture
                if (CurrentRecipe.Image != null)
                {
                    if (CurrentRecipe.Image != "")
                    {
                        BlockUIContainer BUIC = new BlockUIContainer();
                        Image myImage = new Image();
                        BitmapImage bitmapImg = new BitmapImage();
                        bitmapImg.BeginInit();
                        bitmapImg.UriSource = new Uri(CurrentRecipe.Image);
                        bitmapImg.EndInit();
                        myImage.Width = 250;
                        myImage.Height = 250;
                        myImage.Source = bitmapImg;
                        BUIC.TextAlignment = TextAlignment.Center;
                        BUIC.Child = myImage;
                        myFlowDoc.Blocks.Add(BUIC);
                    }
                }

                // Load Basic Info *****
                {
                    Paragraph BasicInfo = new Paragraph();
                    Bold subheading = new Bold(new Run($"Basic Information\n"));
                    subheading.FontSize = 14;
                    BasicInfo.Inlines.Add(subheading);

                    Bold make = new Bold(new Run($"Makes:"));
                    Bold prep = new Bold(new Run($"Prep Time:"));
                    Bold cook = new Bold(new Run($"Cooking Time:"));

                    BasicInfo.Inlines.Add("\t");
                    BasicInfo.Inlines.Add(make);

                    if (CurrentRecipe.Quantity > 0)
                    {
                        // Quantity & QtyType
                        if (Btn_Default.IsChecked == true)
                        {
                            BasicInfo.Inlines.Add($" {CurrentRecipe.Quantity} {General.LookupMTypeID(CurrentRecipe.QtyType)}\n");
                        }
                        else
                        {
                            BasicInfo.Inlines.Add($" {CurrentRecipe.Quantity * ConvertWith(CurrentRecipe.QtyType)} {ConvertTo(CurrentRecipe.QtyType)}\n");
                        }
                    }
                    else
                    {
                        // or
                        // Makes & Serving
                        BasicInfo.Inlines.Add($" {CurrentRecipe.Makes} with {CurrentRecipe.Serving} portions\n");
                    }

                    // Preperation Time
                    // Cooking Time
                    BasicInfo.Inlines.Add("\t");
                    BasicInfo.Inlines.Add(prep);
                    BasicInfo.Inlines.Add($" {CurrentRecipe.Preparation} \t");
                    BasicInfo.Inlines.Add(cook);
                    BasicInfo.Inlines.Add($" {CurrentRecipe.Cooking}\n");

                    myFlowDoc.Blocks.Add(BasicInfo);
                }

                // Load Ingredients
                if (Btn_Default.IsChecked == true)
                {
                    Paragraph IngredientsText = new Paragraph();
                    List<IngListItemModel> IngItemList = new List<IngListItemModel>();
                    IngItemList = SQLiteDataAccess.LoadIngListItem(CurrentRecipe.RecipeId);

                    if (IngItemList.Count > 0)
                    {
                        Bold subheading = new Bold(new Run($"Ingredients\n"));
                        subheading.FontSize = 14;
                        IngredientsText.Inlines.Add(subheading);
                        IngredientsText.FontSize = 12;

                        foreach (var item in IngItemList)
                        {
                            string mtype = General.LookupMTypeID(item.MType);

                            if (mtype != "-")
                            {
                                mtype = mtype + " -";
                            }

                            IngredientsText.Inlines.Add($"\t{item.Quantity} {mtype} {General.LookupIngredientID(item.IngredientFK, Ingredients)}\n");
                        }
                    }
                    myFlowDoc.Blocks.Add(IngredientsText);
                }

                // Load Converted
                if (Btn_Metric.IsChecked == true)
                {
                    Paragraph ConvertedsText = new Paragraph();
                    List<IngListItemModel> IngItemList = new List<IngListItemModel>();
                    IngItemList = SQLiteDataAccess.LoadIngListItem(CurrentRecipe.RecipeId);

                    if (IngItemList.Count > 0)
                    {
                        Bold subheading = new Bold(new Run($"Ingredients\n"));
                        subheading.FontSize = 14;
                        ConvertedsText.Inlines.Add(subheading);
                        ConvertedsText.FontSize = 12;

                        // convert to Text
                        foreach (var item in IngItemList)
                        {
                            string mtype = General.LookupMTypeID(item.MType);
                            string CQty = "";
                            string CMType = "";

                            if (MTypeConvertable(item.MType))
                            {
                                // Is Convertable
                                float mass = LookupIngredientMass(item.IngredientFK);

                                if (mass > 0.0f)
                                {
                                    float temp = ((item.Quantity * ConvertWith(item.MType) / 100) * mass);

                                    CQty = (temp).ToString("#,##0.##");
                                    CMType = General.LookupMTypeID(2); // Gram's
                                }
                                else
                                {
                                    CQty = (item.Quantity * ConvertWith(item.MType)).ToString("#,##0.##");
                                    CMType = ConvertTo(item.MType);
                                }
                            }
                            else
                            {
                                // Not Convertable
                                // don't change (-, Small, Medium, Large)
                                CQty = (item.Quantity).ToString("#,##0.##");
                                CMType = General.LookupMTypeID(item.MType);
                            }

                            ConvertedsText.Inlines.Add($"\t{CQty} {CMType} {General.LookupIngredientID(item.IngredientFK, Ingredients)}\n");
                        }
                    }
                    myFlowDoc.Blocks.Add(ConvertedsText);
                }

                // Load Direction
                if (Directions.Count > 0)
                {
                    Paragraph DirectionsText = new Paragraph();
                    Bold subheading = new Bold(new Run($"Direction\n"));
                    subheading.FontSize = 14;
                    DirectionsText.Inlines.Add(subheading);
                    DirectionsText.FontSize = 12;

                    foreach (var item in Directions)
                    {
                        DirectionsText.Inlines.Add($"\t{item.Position}) {item.Direction}\n");
                    }
                    myFlowDoc.Blocks.Add(DirectionsText);
                }

                // Load Notes1 (Chef Notes)
                if (Btn_Note1s.IsChecked == true)
                {
                    Paragraph Notes1sText = new Paragraph();
                    if (Note1s.Count > 0)
                    {
                        Bold subheading = new Bold(new Run($"Chef Notes\n"));
                        subheading.FontSize = 12;
                        Notes1sText.Inlines.Add(subheading);
                        Notes1sText.FontSize = 12;

                        foreach (var item in Note1s)
                        {
                            Notes1sText.Inlines.Add($"\t{item.Note1Nbr}) {item.Note1}\n");
                        }
                    }
                    myFlowDoc.Blocks.Add(Notes1sText);
                }

                // Load Notes2 (Personal Notes)
                if (Btn_Note2s.IsChecked == true)
                {
                    Paragraph Notes2sText = new Paragraph();
                    if (Note2s.Count > 0)
                    {
                        Bold subheading = new Bold(new Run($"Personal Notes\n"));
                        subheading.FontSize = 12;
                        Notes2sText.Inlines.Add(subheading);
                        Notes2sText.FontSize = 12;

                        foreach (var item in Note2s)
                        {
                            Notes2sText.Inlines.Add($"\t{item.Note2Nbr}) {item.Note2}\n");
                        }
                    }
                    myFlowDoc.Blocks.Add(Notes2sText);
                }

                // Show the Document
                rtfDetail.Document = myFlowDoc;
            }
        }

        // *********************************************************

        #endregion Display

        // ************************************************

        #region Nutrition

        // ************************************************
        private void UpdateNutrition()
        {
            /// Food Labeling
            /// https://www.nutrition.org.uk/putting-it-into-practice/food-labelling/looking-at-labels/

            // Populate the List (Incase its changed)
            IngList = SQLiteDataAccess.LoadIngListItem(CurrentRecipe.RecipeId);

            int TCalories = 0;
            int TEnergy = 0;
            float TFat = 0.0f;
            float TSaturatedFat = 0.0f;
            float TCarbohydrate = 0.0f;
            float TSugars = 0.0f;
            float TFibre = 0.0f;
            float TProtein = 0.0f;
            float TSalt = 0.0f;

            bool bigError = false;

            if (IngList.Count > 0)
            {
                foreach (var IngItem in IngList)
                {
                    IngredientsModel ing = General.LookupIngredient(IngItem.IngredientFK, Ingredients);

                    if (ing.NutientInfo())
                    {
                        if (MTypeConvertable(IngItem.MType))
                        {
                            // Is Convertable
                            float mass = LookupIngredientMass(IngItem.IngredientFK);

                            if (mass > 0.0f)
                            {
                                float temp = ((IngItem.Quantity * ConvertWith(IngItem.MType) / 100) * mass);

                                TCalories += (int)(temp * ing.Calories);
                                TEnergy += (int)(temp * ing.Energy);
                                TFat += temp * ing.Fat;
                                TSaturatedFat += temp * ing.SaturatedFat;
                                TCarbohydrate += temp * ing.Carbohydrate;
                                TSugars += temp * ing.Sugars;
                                TFibre += temp * ing.Fibre;
                                TProtein += temp * ing.Protein;
                                TSalt += temp * ing.Salt;
                            }
                            else if (IngItem.Quantity > 0)
                            {
                                float temp = (IngItem.Quantity / 100);

                                TCalories += (int)(temp * ing.Calories);
                                TEnergy += (int)(temp * ing.Energy);
                                TFat += temp * ing.Fat;
                                TSaturatedFat += temp * ing.SaturatedFat;
                                TCarbohydrate += temp * ing.Carbohydrate;
                                TSugars += temp * ing.Sugars;
                                TFibre += temp * ing.Fibre;
                                TProtein += temp * ing.Protein;
                                TSalt += temp * ing.Salt;
                            }
                            else
                            {
                                bigError = true;
                                errorlog.WarningMessage("Nutrition Info", $"{ing.IngredientName} has No Mass!");
                            }
                        }
                        else
                        {
                            string MT = General.LookupMTypeID(IngItem.MType);

                            float temp = 0.0f;

                            if (MT == "-")
                            {
                                temp = (IngItem.Quantity * ing.Medium / 100);
                            }
                            else if (MT == "small")
                            {
                                temp = (IngItem.Quantity * ing.Small / 100);
                            }
                            else if (MT == "medium")
                            {
                                temp = (IngItem.Quantity * ing.Medium / 100);
                            }
                            else if (MT == "large")
                            {
                                temp = (IngItem.Quantity * ing.Large / 100);
                            }

                            TCalories += (int)(temp * ing.Calories);
                            TEnergy += (int)(temp * ing.Energy);
                            TFat += temp * ing.Fat;
                            TSaturatedFat += temp * ing.SaturatedFat;
                            TCarbohydrate += temp * ing.Carbohydrate;
                            TSugars += temp * ing.Sugars;
                            TFibre += temp * ing.Fibre;
                            TProtein += temp * ing.Protein;
                            TSalt += temp * ing.Salt;
                        }
                    }
                    else
                    {
                        bigError = true;
                        errorlog.WarningMessage("Nutrition Info", $"{ing.IngredientName} has No Nutrition Info!");
                    }
                }
            }

            lbTCalories.Content = TCalories.ToString("#,##0");
            lbPCalories.Content = "0";

            lbTEnergy.Content = TEnergy.ToString("#,##0");
            lbPEnergy.Content = "0";

            lbTFat.Content = TFat.ToString("#,##0.00");
            lbTFat.Background = TrafficLight(TFat, 3.0f, 17.5f, 21.0f);
            lbPFat.Content = "0.00";

            lbTSaturatedFat.Content = TSaturatedFat.ToString("#,##0.00");
            lbTSaturatedFat.Background = TrafficLight(TSaturatedFat, 1.50f, 5.0f, 6.0f);
            lbPSaturatedFat.Content = "0.00";

            lbTCarbohydrate.Content = TCarbohydrate.ToString("#,##0.00");
            lbPCarbohydrate.Content = "0.00";

            lbTSugars.Content = TSugars.ToString("#,##0.00");
            lbTSugars.Background = TrafficLight(TSugars, 5.0f, 22.5f, 27.0f);
            lbPSugars.Content = "0.00";

            lbTFibre.Content = TFibre.ToString("#,##0.00");
            lbPFibre.Content = "0.00";

            lbTProtein.Content = TProtein.ToString("#,##0.00");
            lbPProtein.Content = "0.00";

            lbTSalt.Content = TSalt.ToString("#,##0.00");
            lbTSalt.Background = TrafficLight(TSalt / 1000, 0.30f, 1.5f, 1.8f);
            lbPSalt.Content = "0.00";

            int portionSize = CurrentRecipe.Makes * CurrentRecipe.Serving;
            if (portionSize > 0)
            {
                lbPCalories.Content = (TCalories / portionSize).ToString("#,##0");
                lbPEnergy.Content = (TEnergy / portionSize).ToString("#,##0");
                lbPFat.Content = (TFat / portionSize).ToString("#,##0.00");
                lbPFat.Background = TrafficLight((TFat / portionSize), 3.0f, 17.5f, 21.0f);

                lbPSaturatedFat.Content = (TSaturatedFat / portionSize).ToString("#,##0.00");
                lbPSaturatedFat.Background = TrafficLight((TSaturatedFat / portionSize), 1.50f, 5.0f, 6.0f);

                lbPCarbohydrate.Content = (TCarbohydrate / portionSize).ToString("#,##0.00");
                lbPSugars.Content = (TSugars / portionSize).ToString("#,##0.00");
                lbPSugars.Background = TrafficLight((TSugars / portionSize), 5.0f, 22.5f, 27.0f);

                lbPFibre.Content = (TFibre / portionSize).ToString("#,##0.00");
                lbPProtein.Content = (TProtein / portionSize).ToString("#,##0.00");
                lbPSalt.Content = (TSalt / portionSize).ToString("#,##0.00");
                lbPSalt.Background = TrafficLight((TSalt / portionSize) / 1000, 0.30f, 1.5f, 1.8f);
            }

            lbError.Content = (bigError) ? "Not all Ingredients had Values!" : "";
        }

        // ************************************************
        private Brush TrafficLight(float value, float low, float medium, float high)
        {
            if (value < low)
            {
                return (Brush)Brushes.LightPink;
            }

            if (value.Between(low, medium))
            {
                return (Brush)Brushes.LightGreen;
            }

            if (value.Between(medium, high))
            {
                return (Brush)Brushes.Yellow;
            }

            // Must therefore be High
            return (Brush)Brushes.Red;
        }

        // ************************************************

        #endregion Nutrition

        // ************************************************

        #region Link

        // ************************************************

        private void AddLink_Click(object sender, RoutedEventArgs e)
        {
            if (cbLink.Text != "")
            {
                LinksModel lk = new();

                lk.RecipeFK = CurrentRecipe.RecipeId;

                lk.LinkFK = LookupRecipe(cbLink.Text.Trim());

                SQLiteDataAccess.AddLink(lk);

                errorlog.InformationMessage("Recipe Link Added - ", lk.ToString());
            }

            RefreshLinkDG();

            ClearLinkField();
        }

        // *********************************************************
        private void ClearLinkField()
        {
            cbLink.Text = string.Empty;
        }

        // ************************************************
        private void DeleteLink_Click(object sender, RoutedEventArgs e)
        {
            Int32 selectedRowCount = dgLinkSearch.SelectedIndex;

            if (selectedRowCount > -1 && Links.Count > 0)
            {
                SQLiteDataAccess.DeleteLinks(CurrentRecipe.RecipeId, Links[selectedRowCount].LinkFK);
                Links.RemoveAt(selectedRowCount);
            }

            RefreshLinkDG();
            ClearLinkField();
        }

        // ************************************************

        private int LookupRecipe(string rcp)
        {
            var m = Recipes.Find(x => x.RecipeTitle == rcp);

            if (m != null)
            {
                return m.RecipeId;
            }

            return -1;
        }

        // ************************************************
        private string LookupRecipeID(int id)
        {
            var m = Recipes.Find(x => x.RecipeId == id);

            if (m != null)
            {
                return m.RecipeTitle;
            }

            return "";
        }

        // *********************************************************
        private void RefreshLinkDG()
        {
            List<LinksTable> query = new List<LinksTable>();

            // Get the Updated Link
            Links = SQLiteDataAccess.LoadLinks(CurrentRecipe.RecipeId);

            if (Links.Count > 0)
            {
                // convert Foreign Keys to Text
                foreach (var item in Links)
                {
                    LinksTable lk = new LinksTable();

                    lk.DLink = LookupRecipeID(item.LinkFK);

                    query.Add(lk);
                }
            }

            // Read the list and add to the query to display in the DataGrid
            dgLinkSearch.ItemsSource = query;
        }

        // ************************************************
        private void WireUpLinkCB()
        {
            cbLink.Items.Clear();

            // Get the Recipe's
            Recipes = SQLiteDataAccess.LoadRecipes(CurrentRecipe.BookFk);

            Recipes.Sort((x, y) => x.RecipeTitle.CompareTo(y.RecipeTitle));

            foreach (var r in Recipes)
            {
                cbLink.Items.Add(r.RecipeTitle);
            }
        }

        // ************************************************

        #endregion Link

        // ************************************************

        #region General

        // ************************************************
        public void DisplayImage(string filename)
        {
            if (filename != null)
            {
                BitmapImage bitmap1 = new BitmapImage();
                bitmap1.BeginInit();
                bitmap1.UriSource = new Uri(filename);
                bitmap1.EndInit();
                RecipeImage.Source = bitmap1;
            }
        }

        // ************************************************
        public static bool GetImageFileName(ref string filename)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            string Filter = "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff"
                          + "BMP|*.bmp|"
                          + "GIF|*.gif|"
                          + "JPG|*.jpg;*.jpeg|"
                          + "PNG|*.png|"
                          + "TIFF|*.tif;*.tiff|";
            openFileDialog.Filter = Filter;

            if (filename == null)
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
            else
            {
                openFileDialog.InitialDirectory = Path.GetDirectoryName(filename);
            }

            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.FileName;
                return true;
            }
            else
            {
                filename = "";
                return false;
            }
        }

        // ************************************************
        private void UpdateRecipeButton(object sender, RoutedEventArgs e)
        {
            if (ValidateContentsSrceen() == true)
            {
                errorlog.InformationMessage("Recipe Updated", "From: " + CurrentRecipe.ToString());

                CurrentRecipe.ChapterFk = LookupChapter(cbChapter2.Text.Trim());
                CurrentRecipe.Cuisine = LookupCuisine(cbCuisine3.Text.Trim());
                CurrentRecipe.Referance = int.Parse(tbReferance.Text);
                CurrentRecipe.Makes = int.Parse(tbMakes.Text);
                CurrentRecipe.Serving = int.Parse(tbServing.Text);
                CurrentRecipe.Quantity = float.Parse(tbQuantity.Text);
                CurrentRecipe.QtyType = General.LookupMType(cbQtyType.Text.Trim());
                CurrentRecipe.Preparation = tbPreparation.Text;
                CurrentRecipe.Cooking = tbCooking.Text;
                CurrentRecipe.Favourite = (ckbFavourite.IsChecked == true) ? true : false;
                CurrentRecipe.Image = tbImage.Text;

                errorlog.InformationMessage("", "To:" + CurrentRecipe.ToString());

                SQLiteDataAccess.UpdateRecipe(CurrentRecipe);
            }
        }

        // ************************************************
        private bool ValidateContentsSrceen()
        {
            bool output = true;
            string caption = "Book Contents";
            int intValue;
            float floatValue;

            if (cbChapter2.Text == "")
            {
                output = false;
                MessageBox.Show("A chapter needs to be selected!", caption, MessageBoxButton.OK);
            }

            if (tbReferance.Text == "")
            {
                output = false;
                MessageBox.Show("Referance can not be blank!", caption, MessageBoxButton.OK);
            }

            if (!int.TryParse(tbReferance.Text, out intValue))
            {
                output = false;
                MessageBox.Show("Referance needs to be a number!", caption, MessageBoxButton.OK);
            }

            if (!float.TryParse(tbQuantity.Text, out floatValue))
            {
                output = false;
                MessageBox.Show("Quantity needs to be a number!", caption, MessageBoxButton.OK);
            }

            // QtyType

            if (!int.TryParse(tbMakes.Text, out intValue))
            {
                output = false;
                MessageBox.Show("Make needs to be a number!", caption, MessageBoxButton.OK);
            }

            if (!int.TryParse(tbServing.Text, out intValue))
            {
                output = false;
                MessageBox.Show("Serving needs to be a number!", caption, MessageBoxButton.OK);
            }

            // Preparation Time - String
            // Cooking Time - String

            return output;
        }

        // ************************************************
        private void WireUpBasics()
        {
            lblRecipeIdValue.Content = CurrentRecipe.RecipeId;

            cbChapter2.SelectedValue = LookupChapterID(CurrentRecipe.ChapterFk);
            tbReferance.Text = CurrentRecipe.Referance.ToString();
            cbCuisine3.SelectedValue = LookupCuisineID(CurrentRecipe.Cuisine);
            ckbFavourite.IsChecked = CurrentRecipe.Favourite;

            tbImage.Text = CurrentRecipe.Image;

            tbQuantity.Text = CurrentRecipe.Quantity.ToString();
            cbQtyType.SelectedValue = General.LookupMTypeID(CurrentRecipe.QtyType);
            tbMakes.Text = CurrentRecipe.Makes.ToString();
            tbServing.Text = CurrentRecipe.Serving.ToString();

            if (CurrentRecipe.Preparation == null)
            {
                CurrentRecipe.Preparation = "0";
            }
            tbPreparation.Text = CurrentRecipe.Preparation;

            if (CurrentRecipe.Cooking == null)
            {
                CurrentRecipe.Cooking = "0";
            }
            tbCooking.Text = CurrentRecipe.Cooking;
        }

        // ************************************************
        private bool MTypeConvertable(int id)
        {
            var m = MType.Find(x => x.MTypeId == id);

            if (m != null)
            {
                return (m.ToUnit == "*") ? false : true;
            }

            return false;
        }

        // ************************************************
        private float ConvertWith(int id)
        {
            for (int i = 0; i < MType.Count; i++)
            {
                if (MType[i].MTypeId == id)
                {
                    return MType[i].ConvertWith;
                }
            }

            return 0.0f;
        }

        // ************************************************
        private string ConvertTo(int id)
        {
            for (int i = 0; i < MType.Count; i++)
            {
                if (MType[i].MTypeId == id)
                {
                    return MType[i].ToUnit;
                }
            }

            return "";
        }

        // ************************************************
        private string RecipeType(List<IngListItemModel> ing)
        {
            List<IngTypeModel> IngredientType = new();
            IngredientType = SQLiteDataAccess.LoadIngTypes();

            List<IngredientsModel> myIng = new List<IngredientsModel>();

            IngredientsModel i;

            foreach (var item in ing)
            {
                try
                {
                    i = SQLiteDataAccess.GetIngredient(item.IngredientFK)[0];
                }
                catch
                {
                    // Ingredient Not Found
                    i = new() { IngredientName = "** Error **" };
                }

                myIng.Add(i);
            }

            int max = myIng.Max(t => t.IngredientTypeEN);

            return General.LookupIngredientTypeID(max);
        }

        // ************************************************

        #endregion General

        // ************************************************
    }
}