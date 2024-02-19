using RecipeBooks.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RecipeBooks
{
    /// <summary>
    /// Interaction logic for IngredientsScr.xaml
    /// </summary>
    public partial class IngredientsScr : Window
    {
        // ************************************************
        // ***     G L O B A L E   V A R I A B L E S
        private Joblog errorlog = new Joblog("Books", 1);

        private List<IngredientsModel> Ingredients = new List<IngredientsModel>();
        private List<IngTypeModel> IngredientType = new List<IngTypeModel>();

        public IngredientsScr(Joblog el)
        {
            errorlog = el;

            InitializeComponent();

            LoadIngType();

            RefreshIngredients();

            WireUpIngredientTypeCB();
        }

        // ************************************************
        private void AddIngredientButton(object sender, RoutedEventArgs e)
        {
            if (ValidateIngredientsSrceen() == true)
            {
                IngredientsModel i = new IngredientsModel();

                i.IngredientName = tbIngredientName.Text.Trim();
                i.IngredientTypeEN = General.LookupIngredientType(cbIngredientType.Text.Trim());

                i.Liquid = (ckbLiquid.IsChecked == true) ? true : false;

                if (!i.Liquid)
                {
                    i.Mass = ConvertLiters2Grams();
                }

                i.Calories = (tbCalories.Text != "0") ? int.Parse(tbCalories.Text) : 0;

                i.Energy = (tbEnergy.Text != "0") ? int.Parse(tbEnergy.Text) : 0;

                i.Fat = (tbFat.Text != "0.0") ? float.Parse(tbFat.Text) : 0.0f;

                i.SaturatedFat = (tbSaturatedFat.Text != "0.0") ? float.Parse(tbSaturatedFat.Text) : 0.0f;

                i.Carbohydrate = (tbCarbohydrate.Text != "0.0") ? float.Parse(tbCarbohydrate.Text) : 0.0f;

                i.Sugars = (tbSugars.Text != "0.0") ? float.Parse(tbSugars.Text) : 0.0f;

                i.Fibre = (tbFibre.Text != "0.0") ? float.Parse(tbFibre.Text) : 0.0f;

                i.Protein = (tbProtein.Text != "0.0") ? float.Parse(tbProtein.Text) : 0.0f;

                i.Salt = (tbSalt.Text != "0.0") ? float.Parse(tbSalt.Text) : 0.0f;


                i.Small = (tbSmall.Text != "0.0") ? float.Parse(tbSmall.Text) : 0.0f;

                i.Medium = (tbMedium.Text != "0.0") ? float.Parse(tbMedium.Text) : 0.0f;

                i.Large = (tbLarge.Text != "0.0") ? float.Parse(tbLarge.Text) : 0.0f;

                SQLiteDataAccess.AddIngredient(i);

                // Refresh ListBox
                RefreshIngredients();

                // Find out the new Ingredient ID
                i.IngredientId = General.LookupIngredientType(i.IngredientName);

                errorlog.InformationMessage("Ingredient Added - ", i.ToString());
            }
            else
            {
                MessageBox.Show("One or more of the fields are blank!", "Ingredient Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ************************************************
        private void CheckBoxChangeState(object sender, RoutedEventArgs e)
        {
            if (ckbLiquid.IsChecked == true)
            {
                tbML.IsEnabled = false;
                tbWeight.IsEnabled = false;
            }
            else
            {
                tbML.IsEnabled = true;
                tbWeight.IsEnabled = true;
            }
        }

        // ************************************************
        private void ClearIngredientBoxes()
        {
            lblIngredientLabel.Content = (Ingredients.Count != 0) ? $"Ingredients ({Ingredients.Count})" : "Ingredients";

            lblIngredientIdValue.Content = "";
            tbIngredientName.Text = "";
            cbIngredientType.Text = "";

            ckbLiquid.IsChecked = false;

            tbCalories.Text = "0";
            tbEnergy.Text = "0";
            tbFat.Text = "0.0";
            tbSaturatedFat.Text = "0.0";
            tbCarbohydrate.Text = "0.0";
            tbSugars.Text = "0.0";
            tbFibre.Text = "0.0";
            tbProtein.Text = "0.0";
            tbSalt.Text = "0.0";

            tbSmall.Text = "0.0";
            tbMedium.Text = "0.0";
            tbLarge.Text = "0.0";

            tbWeight.Text = "0.0";
            tbML.Text = "0.0";
        }

        // ************************************************
        private void ClearIngredientButton(object sender, RoutedEventArgs e)
        {
            ClearIngredientBoxes();
        }

        // ************************************************
        private float ConvertLiters2Grams()
        {
            float weight = (tbWeight.Text != null) ? float.Parse(tbWeight.Text) : 0.0f;
            float ml = (tbML.Text != null) ? float.Parse(tbML.Text) : 0.0f;

            if (weight != 0.0f && ml != 0.0f)
            {
                if (ml != 100.0f)
                {
                    weight = (weight / ml) * 100.0f;
                }
            }
            else
            {
                weight = 0.0f;
            }

            return weight;
        }

        // ************************************************
        private void DeleteIngredientButton(object sender, RoutedEventArgs e)
        {
            int index = lbIngredientsList.SelectedIndex;

            errorlog.InformationMessage("Ingredient Deleted - ", Ingredients[index].ToString());

            SQLiteDataAccess.DeleteIngredient(Ingredients[index].IngredientId);

            RefreshIngredients();
        }

        // ************************************************
        private void ExportIngredientsButton(object sender, RoutedEventArgs e)
        {
            CSVFile.WriteIngredientsFile(errorlog, Ingredients);
        }

        // ************************************************
        private void FloatValidation(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            General.FloatValidation(sender, e);
        }

        // ************************************************
        private void ImportIngredientsButton(object sender, RoutedEventArgs e)
        {
            List<IngredientsModel> SearchResults = new List<IngredientsModel>();

            int count = 0;

            Ingredients.Clear();

            try
            {
                Ingredients = CSVFile.ReadIngredientsFile(errorlog);

                foreach (var ing in Ingredients)
                {
                    string q = $"select * from Ingredients where IngredientName = \"{ing.IngredientName}\"";
                    SearchResults = SQLiteDataAccess.SearchIngredients(q);

                    count++;

                    if (SearchResults.Count == 0 || SearchResults == null)
                    {
                        SQLiteDataAccess.AddIngredient(ing);

                        // Find out the new Ingredient ID
                        ing.IngredientId = General.LookupIngredientType(ing.IngredientName);

                        errorlog.InformationMessage("Ingredient Added - ", ing.ToString());
                    }
                }
            }
            catch
            {
                string mesTitle = "Import Ingredients";
                string message = $"Failed to Import file. An error occured at line {count} of the file. Please check you have the correct columns and the correct value types.";
                
                MessageBox.Show(message, mesTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(message, mesTitle);
                return;
            }

            RefreshIngredients();

            string mesBoxTitle = "Ingredients";
            string msgText = $"Finished Importing Ingredients.";

            MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            errorlog.InformationMessage(mesBoxTitle, msgText);
        }

        // ************************************************
        private void IngredientSelectedListBox(object sender, SelectionChangedEventArgs e)
        {
            int index = lbIngredientsList.SelectedIndex;

            if (index != -1)
            {
                // move data into the screen
                lblIngredientIdValue.Content = $"{Ingredients[index].IngredientId}/{index}";

                tbIngredientName.Text = Ingredients[index].IngredientName.Trim();
                cbIngredientType.SelectedValue = General.LookupIngredientTypeID(Ingredients[index].IngredientTypeEN);

                ckbLiquid.IsChecked = Ingredients[index].Liquid;

                tbCalories.Text = Ingredients[index].Calories.ToString();
                tbEnergy.Text = Ingredients[index].Energy.ToString();
                tbFat.Text = Ingredients[index].Fat.ToString();
                tbSaturatedFat.Text = Ingredients[index].SaturatedFat.ToString();
                tbCarbohydrate.Text = Ingredients[index].Carbohydrate.ToString();
                tbSugars.Text = Ingredients[index].Sugars.ToString();
                tbFibre.Text = Ingredients[index].Fibre.ToString();
                tbProtein.Text = Ingredients[index].Protein.ToString();
                tbSalt.Text = Ingredients[index].Salt.ToString();

                tbSmall.Text = Ingredients[index].Small.ToString();
                tbMedium.Text = Ingredients[index].Medium.ToString();
                tbLarge.Text = Ingredients[index].Large.ToString();

                tbWeight.Text = Ingredients[index].Mass.ToString("#,##0.##");
                tbML.Text = (Ingredients[index].Mass > 0) ? "100.0" : "0.0";
            }
        }

        // ************************************************
        private void IngredientTextChanged(object sender, TextChangedEventArgs e)
        {
            if (btnAddIngredient != null)
            {
                btnAddIngredient.IsEnabled = true;

                if (tbIngredientName.Text != "")
                {
                    btnUpdateIngredient.IsEnabled = true;
                    btnDeleteIngredient.IsEnabled = true;

                    btnImportIngredients.IsEnabled = false;
                    btnExportIngredients.IsEnabled = false;
                }
                else
                {
                    btnUpdateIngredient.IsEnabled = false;
                    btnDeleteIngredient.IsEnabled = false;

                    btnImportIngredients.IsEnabled = true;
                    btnExportIngredients.IsEnabled = true;
                }
            }
        }

        // ************************************************
        private void IntValidation(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            General.IntValidation(sender, e);
        }

        // ************************************************
        private void LoadIngType()
        {
            IngredientType = SQLiteDataAccess.LoadIngTypes();

            // Sort IngredientType
            IngredientType.Sort((x, y) => x.IngId.CompareTo(y.IngId));
        }

        // ************************************************
        private void RefreshIngredients()
        {
            // Clear out old Ingredients
            Ingredients.Clear();

            // Get the Ingredients
            Ingredients = SQLiteDataAccess.LoadIngredients();

            // Sort Ingredients
            Ingredients.Sort((x, y) => x.IngredientName.CompareTo(y.IngredientName));

            // reset the add field on the display
            ClearIngredientBoxes();

            // Display the Locations in the ListBox & DropDown
            // Must be Refreshed after EVERYTHING else
            WireUpIngredientsLB();

            lblIngredientIdValue.Content = string.Empty;
        }

        // ************************************************
        private void UpdateIngredientButton(object sender, RoutedEventArgs e)
        {
            if (ValidateIngredientsSrceen() == true)
            {
                int index = lbIngredientsList.SelectedIndex;

                errorlog.InformationMessage("Ingredient Updated", "From: " + Ingredients[index].ToString());

                Ingredients[index].IngredientName = tbIngredientName.Text.Trim();

                Ingredients[index].IngredientTypeEN = General.LookupIngredientType(cbIngredientType.Text.Trim());

                Ingredients[index].Liquid = (ckbLiquid.IsChecked == true) ? true : false;

                if (!Ingredients[index].Liquid)
                {
                    Ingredients[index].Mass = ConvertLiters2Grams();
                }

                Ingredients[index].Calories = (tbCalories.Text != "") ? int.Parse(tbCalories.Text) : 0;

                Ingredients[index].Energy = (tbEnergy.Text != "") ? int.Parse(tbEnergy.Text) : 0;

                Ingredients[index].Fat = (tbFat.Text != "") ? float.Parse(tbFat.Text) : 0.0f;

                Ingredients[index].SaturatedFat = (tbSaturatedFat.Text != "") ? float.Parse(tbSaturatedFat.Text) : 0.0f;

                Ingredients[index].Carbohydrate = (tbCarbohydrate.Text != "") ? float.Parse(tbCarbohydrate.Text) : 0.0f;

                Ingredients[index].Sugars = (tbSugars.Text != "") ? float.Parse(tbSugars.Text) : 0.0f;

                Ingredients[index].Fibre = (tbFibre.Text != "") ? float.Parse(tbFibre.Text) : 0.0f;

                Ingredients[index].Protein = (tbProtein.Text != "") ? float.Parse(tbProtein.Text) : 0.0f;

                Ingredients[index].Salt = (tbSalt.Text != "") ? float.Parse(tbSalt.Text) : 0.0f;

                Ingredients[index].Small = (tbSmall.Text != "") ? float.Parse(tbSmall.Text) : 0.0f;

                Ingredients[index].Medium = (tbMedium.Text != "") ? float.Parse(tbMedium.Text) : 0.0f;

                Ingredients[index].Large = (tbLarge.Text != "") ? float.Parse(tbLarge.Text) : 0.0f;


                errorlog.InformationMessage("", "To:" + Ingredients[index].ToString());

                SQLiteDataAccess.UpdateIngredient(Ingredients[index]);

                // Refresh Ingredients
                RefreshIngredients();
            }
        }

        // ************************************************
        private bool ValidateIngredientsSrceen()
        {
            bool output = true;
            string caption = "Ingredients";

            if (tbIngredientName.Text == "")
            {
                output = false;
                MessageBox.Show("Ingredient Name can not be blank!", caption, MessageBoxButton.OK);
            }

            if (cbIngredientType.Text == "")
            {
                output = false;
                MessageBox.Show("Ingredient Type can not be blank!", caption, MessageBoxButton.OK);
            }

            return output;
        }

        // ************************************************
        private void WireUpIngredientsLB()
        {
            lbIngredientsList.Items.Clear();

            foreach (var i in Ingredients)
            {
                lbIngredientsList.Items.Add($"{i.IngredientName} ({General.LookupIngredientTypeID(i.IngredientTypeEN)})");
            }
        }

        // ************************************************
        private void WireUpIngredientTypeCB()
        {
            cbIngredientType.Items.Clear();

            foreach (var c in IngredientType)
            {
                cbIngredientType.Items.Add(c.IngType);
            }
        }
        // ************************************************
    }
}