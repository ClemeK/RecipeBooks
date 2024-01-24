using RecipeBooks.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RecipeBooks
{
    /// <summary>
    /// Interaction logic for LocationsScr.xaml
    /// </summary>
    public partial class LocationsScr : Window
    {
        // ************************************************
        // ***     G L O B A L E   V A R I A B L E S
        private Joblog errorlog = new Joblog("Books", 1);

        private List<LocationsModel> Locations = new List<LocationsModel>();

        public LocationsScr(Joblog el)
        {
            errorlog = el;

            InitializeComponent();

            RefreshLocations();
        }

        #region Location

        private void AddLocationsButton(object sender, RoutedEventArgs e)
        {
            if (ValidateLocationsSrceen() == true)
            {
                LocationsModel l = new LocationsModel();

                l.LocationTitle = tbLocation2.Text.Trim();

                SQLiteDataAccess.AddLocation(l);

                // Refresh ListBox
                RefreshLocations();

                // Find out the new Location ID
                l.LocationId = LookupLocation(l.LocationTitle);

                errorlog.InformationMessage("Location Added - ", l.ToString());
            }
            else
            {
                MessageBox.Show("One or more of the fields are blank!", "Location Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ************************************************
        private void ClearLocationBoxes()
        {
            lblLocationLabel.Content = (Locations.Count != 0) ? $"Locations ({Locations.Count})" : "Locations";

            lblLocationIdValue.Content = "";
            tbLocation2.Text = "";
        }

        // ************************************************
        private void ClearLocationButton(object sender, RoutedEventArgs e)
        {
            RefreshLocations();
        }

        // ************************************************
        private void DeleteLocation()
        {
            int index = lbLocationsList.SelectedIndex;

            errorlog.InformationMessage("Location Deleted - ", Locations[index].ToString());

            SQLiteDataAccess.DeleteLocation(Locations[index].LocationId);

            RefreshLocations();
        }

        // ************************************************
        private void DeleteLocationButton(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Deleting Location", "Recipe Books",
                                    MessageBoxButton.YesNo, MessageBoxImage.Hand);

            if (res == MessageBoxResult.Yes)
            {
                // Delete the Location
                DeleteLocation();
            }

            RefreshLocations();
        }

        // ************************************************
        private void ExportLocationsButton(object sender, RoutedEventArgs e)
        {
            CSVFile.WriteLocationsFile(errorlog, Locations);
        }

        // ************************************************
        private void ImportLocationsButton(object sender, RoutedEventArgs e)
        {
            List<LocationsModel> SearchResults = new List<LocationsModel>();

            Locations.Clear();
            Locations = CSVFile.ReadLocationsFile(errorlog);

            foreach (var loc in Locations)
            {
                string q = $"select * from Locations where LocationTitle = \"{loc.LocationTitle}\"";
                SearchResults = SQLiteDataAccess.SearchLocations(q);

                if (SearchResults.Count == 0 || SearchResults == null)
                {
                    SQLiteDataAccess.AddLocation(loc);
                }
            }

            RefreshLocationLB();
        }

        // ************************************************
        private void LocationTextChanged(object sender, TextChangedEventArgs e)
        {
            if (btnAddLocation != null)
            {
                btnAddLocation.IsEnabled = true;

                if (tbLocation2.Text != "")
                {
                    btnUpdateLocation.IsEnabled = true;
                    btnDeleteLocation.IsEnabled = true;

                    btnImportLocations.IsEnabled = false;
                    btnExportLocations.IsEnabled = false;
                }
                else
                {
                    btnUpdateLocation.IsEnabled = false;
                    btnDeleteLocation.IsEnabled = false;

                    btnImportLocations.IsEnabled = true;
                    btnExportLocations.IsEnabled = true;
                }
            }
        }

        // ************************************************
        private int LookupLocation(string loc)
        {
            for (int i = 0; i < Locations.Count; i++)
            {
                if (Locations[i].LocationTitle == loc)
                {
                    return Locations[i].LocationId;
                }
            }

            return -1;
        }

        // ************************************************
        private void LocationSelectedListBox(object sender, SelectionChangedEventArgs e)
        {
            int index = lbLocationsList.SelectedIndex;

            if (index != -1)
            {
                // move data into the screen
                lblLocationIdValue.Content = $"{Locations[index].LocationId}/{index}";

                tbLocation2.Text = Locations[index].LocationTitle.Trim();
            }
        }

        // ************************************************
        private void RefreshLocationLB()
        {
            // Clear out old Locations
            Locations.Clear();

            // Get the Locations
            Locations = SQLiteDataAccess.LoadLocations();

            // Sort
            Locations.Sort((x, y) => x.LocationTitle.CompareTo(y.LocationTitle));

            // reset the add field on the display
            ClearLocationBoxes();

            // Display the books in the ListBox
            // Must be Refreshed after EVERYTHING else
            WireUpLocationsLB();

            lblLocationIdValue.Content = "";
        }

        // ************************************************
        private void RefreshLocations()
        {
            // Clear out old Locations
            Locations.Clear();

            // Get the Location
            Locations = SQLiteDataAccess.LoadLocations();

            // Sort Location
            Locations.Sort((x, y) => x.LocationTitle.CompareTo(y.LocationTitle));

            // reset the add field on the display
            ClearLocationBoxes();

            // Display the Locations in the ListBox & DropDown
            // Must be Refreshed after EVERYTHING else
            WireUpLocationsLB();
        }

        // ************************************************
        private void UpdateLocationButton(object sender, RoutedEventArgs e)
        {
            if (ValidateLocationsSrceen() == true)
            {
                int index = lbLocationsList.SelectedIndex;

                errorlog.InformationMessage("Location Updated", "From: " + Locations[index].ToString());

                Locations[index].LocationTitle = tbLocation2.Text.Trim();

                errorlog.InformationMessage("", "To:" + Locations[index].ToString());

                SQLiteDataAccess.UpdateLocation(Locations[index]);

                // Refresh Locations
                RefreshLocations();
            }
        }

        // ************************************************
        private bool ValidateLocationsSrceen()
        {
            bool output = true;
            string caption = "Locations";

            if (tbLocation2.Text == "")
            {
                output = false;
                MessageBox.Show("Location can not be blank!", caption, MessageBoxButton.OK);
            }

            return output;
        }

        // ************************************************
        private void WireUpLocationsLB()
        {
            lbLocationsList.Items.Clear();

            foreach (var l in Locations)
            {
                string text = l.ListBoxText();

                lbLocationsList.Items.Add(text);
            }
        }

        #endregion Location
    }
}