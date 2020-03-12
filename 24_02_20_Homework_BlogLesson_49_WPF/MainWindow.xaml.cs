using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _24_02_20_Homework_BlogLesson_49_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }
        private async void Initialize()
        {
            


                string strn = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName;
                string strn2 = strn + "\\Resources";


                var flagImages = Directory.GetFiles(System.IO.Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName, "Resources"));

                string pathToFlags = System.IO.Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName, "Resources");
                string pathToJsonCountriesFile = System.IO.Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName, "Resources", "countries.json");
                var parser = new CountriesParser();                

                List<Country> country = await parser.Deserialize(pathToJsonCountriesFile, pathToFlags);

                cmbMyComboBox.ItemsSource = country;


            





            txtNumber1.Text = "Type a number here";
            txtNumber2.Text = "Type a number here";
            txtNumber1.TextChanged += TextBox_TextChanged;
            txtNumber2.TextChanged += TextBox_TextChanged;
            txtNumber1.GotFocus += TextBox_GotFocus;
            txtNumber2.GotFocus += TextBox_GotFocus;
            tblResult.Text = string.Empty;
            btnSum.Click += ((object sender, RoutedEventArgs e) =>
            {
                int n1 = 0; int n2 = 0;
                Int32.TryParse(txtNumber1.Text, out n1);
                Int32.TryParse(txtNumber2.Text, out n2);

                tblResult.Text = (n1 + n2).ToString();
            });


        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!Int32.TryParse((e.Source as TextBox).Text, out int num)) (e.Source as TextBox).Text = string.Empty;
        }
        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if ((!Int32.TryParse((e.Source as TextBox).Text, out int num) || String.IsNullOrWhiteSpace((sender as TextBox).Text)) && !String.IsNullOrEmpty((e.Source as TextBox).Text))
            {
                string text = string.Empty;
                foreach (var s in (sender as TextBox).Text)
                {
                    if (Int32.TryParse(s.ToString(), out int charNum)) text += charNum;
                }
                (sender as TextBox).Text = text;
                (sender as TextBox).CaretIndex = (sender as TextBox).Text.Length;
                MessageBox.Show("Please type only numbers!");
            }

        }

    }
}
