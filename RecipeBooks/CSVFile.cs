using CsvHelper;
using Microsoft.Win32;
using RecipeBooks.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace RecipeBooks
{
    internal class CSVFile
    {
        // *********************************************************
        // *** Filenames
        // *********************************************************
        public static bool GetImportFileName(out string filename)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Text files (*.csv)|*.csv|All files (*.*)|*.*";

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

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

        // *********************************************************
        public static bool GetExportFileName(out string filename)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Text file (*.csv)|*.csv|All file (*.*)|*.*";

            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (saveFileDialog.ShowDialog() == true)
            {
                filename = saveFileDialog.FileName;
                return true;
            }
            else
            {
                filename = "";
                return false;
            }
        }

        // *********************************************************
        // *** Books
        // *********************************************************
        public static List<BookColumn> ReadBooksFile(string path, string header)
        {
            string[] lines = System.IO.File.ReadAllLines(path);

            int freq = lines[0].Count(f => (f == ','));

            List<BookColumn> data = new List<BookColumn> { };

            // read through the Lines of the file
            for (int j = 0; j < lines.Length; j++)
            {
                if (lines[j] != "")
                {
                    string[] columns = lines[j].Split(',');

                    if (columns[0] != header)
                    {
                        BookColumn b = new BookColumn();
                        b.c01 = columns[0].Trim();
                        b.c02 = columns[1].Trim();
                        b.c03 = columns[2].Trim();
                        b.c04 = columns[3].Trim();
                        b.c05 = columns[4].Trim();
                        b.c06 = columns[5].Trim();
                        b.c07 = columns[6].Trim();
                        b.c08 = columns[7].Trim();
                        b.c09 = columns[8].Trim();
                        b.c10 = columns[9].Trim();
                        data.Add(b);
                    }
                }
            }
            return data;
        }

        // *********************************************************
        // *** Locations
        // *********************************************************
        public static List<LocationsModel> ReadLocationsFile(Joblog errorlog)
        {
            string fileName = "";
            bool worked = GetImportFileName(out fileName);

            List<LocationsModel> records = new();

            if (worked)
            {
                using var reader = new StreamReader(fileName);

                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                records = csv.GetRecords<LocationsModel>().ToList();

                string mesBoxTitle = "Locations";
                string msgText = $"All Locations Imported ({records.Count()})";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(mesBoxTitle, msgText);
            }
            else
            {
                string mesBoxTitle = "Locations";
                string msgText = $"Locations Failed to import";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(mesBoxTitle, msgText);
            }

            return records;
        }

        // *********************************************************
        public static void WriteLocationsFile(Joblog errorlog, List<LocationsModel> Locations)
        {
            string fileName = "";
            bool worked = GetExportFileName(out fileName);

            if (worked)
            {
                using var writer = new StreamWriter(fileName);
                using var csvOut = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csvOut.WriteRecords(Locations);

                string mesBoxTitle = "Locations";
                string msgText = $"All Locations Export ({Locations.Count()})";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(msgText, mesBoxTitle);
            }
            else
            {
                string mesBoxTitle = "Locations";
                string msgText = $"Export of Location Failed to open file";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                errorlog.WarningMessage(msgText, mesBoxTitle);
            }
        }

        // *********************************************************
        // *** Chapters
        // *********************************************************
        public static List<ChaptersModel> ReadChaptersFile(Joblog errorlog)
        {
            string fileName = "";
            bool worked = GetImportFileName(out fileName);

            List<ChaptersModel> records = new();

            if (worked)
            {
                using var reader = new StreamReader(fileName);

                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                records = csv.GetRecords<ChaptersModel>().ToList();

                string mesBoxTitle = "Chapters";
                string msgText = $"All Chapters Imported ({records.Count()})";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(mesBoxTitle, msgText);
            }
            else
            {
                string mesBoxTitle = "Chapters";
                string msgText = $"Chapters Failed to import";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(mesBoxTitle, msgText);
            }

            return records;
        }

        // *********************************************************
        public static void WriteChaptersFile(Joblog errorlog, List<ChaptersModel> Chapters)
        {
            string fileName = "";
            bool worked = GetExportFileName(out fileName);

            if (worked)
            {
                using var writer = new StreamWriter(fileName);
                using var csvOut = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csvOut.WriteRecords(Chapters);

                string mesBoxTitle = "Chapters";
                string msgText = $"All Chapters Export ({Chapters.Count()})";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(msgText, mesBoxTitle);
            }
            else
            {
                string mesBoxTitle = "Chapterns";
                string msgText = $"Export of Chapters Failed!";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                errorlog.WarningMessage(msgText, mesBoxTitle);
            }
        }

        // *********************************************************
        public static List<RecipeColumn> ReadRecipesFile(string path, string header)
        {
            string[] lines = System.IO.File.ReadAllLines(path);

            int freq = lines[0].Count(f => (f == ','));

            List<RecipeColumn> data = new List<RecipeColumn> { };

            // read through the Lines of the file
            for (int j = 0; j < lines.Length; j++)
            {
                if (lines[j] != "")
                {
                    string[] columns = lines[j].Split(',');

                    if (columns[0] != header)
                    {
                        RecipeColumn c = new RecipeColumn();
                        c.c01 = columns[0].Trim();
                        c.c02 = columns[1].Trim();
                        c.c03 = columns[2].Trim();
                        c.c04 = columns[3].Trim();
                        c.c05 = columns[4].Trim();
                        c.c06 = columns[5].Trim();
                        c.c07 = columns[6].Trim();
                        c.c08 = columns[7].Trim();
                        c.c09 = columns[8].Trim();
                        c.c10 = columns[9].Trim();
                        c.c11 = columns[10].Trim();
                        c.c12 = columns[11].Trim();
                        c.c13 = columns[12].Trim();

                        c.c14 = (columns[13] != null) ? columns[13].Trim() : "";

                        data.Add(c);
                    }
                }
            }
            return data;
        }
        // *********************************************************
        // *** Ingredients
        // *********************************************************
        public static List<IngredientsModel> ReadIngredientsFile(Joblog errorlog)
        {
            string fileName = "";
            bool worked = GetImportFileName(out fileName);

            List<IngredientsModel> records = new();

            if (worked)
            {
                using var reader = new StreamReader(fileName);

                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                records = csv.GetRecords<IngredientsModel>().ToList();

                string mesBoxTitle = "Ingredients";
                string msgText = $"All Ingredients Imported ({records.Count()})";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(mesBoxTitle, msgText);
            }
            else
            {
                string mesBoxTitle = "Ingredients";
                string msgText = $"Ingredients Failed to import";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(mesBoxTitle, msgText);
            }

            return records;
        }

        // *********************************************************
        public static void WriteIngredientsFile(Joblog errorlog, List<IngredientsModel> Ingredients)
        {
            string fileName = "";
            bool worked = GetExportFileName(out fileName);

            if (worked)
            {
                using var writer = new StreamWriter(fileName);
                using var csvOut = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csvOut.WriteRecords(Ingredients);

                string mesBoxTitle = "Ingredients";
                string msgText = $"All Ingredients Export ({Ingredients.Count()})";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                errorlog.InformationMessage(msgText, mesBoxTitle);
            }
            else
            {
                string mesBoxTitle = "Ingredients";
                string msgText = $"Export of Ingredients Failed to open file";

                MessageBox.Show(msgText, mesBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                errorlog.WarningMessage(msgText, mesBoxTitle);
            }
        }
        // *********************************************************

    }
}