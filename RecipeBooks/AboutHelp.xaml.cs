using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace RecipeBooks
{
    /// <summary>
    /// Interaction logic for AboutHelp.xaml
    /// </summary>
    public partial class AboutHelp : Window
    {
        private Dictionary<string, string> ScreenText = new Dictionary<string, string>();

        public AboutHelp()
        {
            InitializeComponent();

            string fileName = @"Resources\Help.rtf";

            TextRange range;
            FileStream fStream;

            if (File.Exists(fileName))
            {
                range = new TextRange(HelpText.Document.ContentStart, HelpText.Document.ContentEnd);
                fStream = new FileStream(fileName, FileMode.Open);
                range.Load(fStream, DataFormats.Rtf);
                fStream.Close();
            }
        }
    }
}