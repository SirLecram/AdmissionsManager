using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Okno dialogowe zawartości jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace AdmissionsManager.View
{
    public sealed partial class NewDialog : ContentDialog
    {
        private Dictionary<int, TextBlock> textBlocksDictionary = new Dictionary<int, TextBlock>();
        private Dictionary<int, TextBox> textBoxesDictionary = new Dictionary<int, TextBox>();
        private TextBlock AlertText = new TextBlock();
        private IEnumerable<string> neededValues;
        public List<object> ValuesOfNewObject { get; private set; }

        public NewDialog(IEnumerable<string> namesOfColumn, IDictionary<int, string> typesOfColumn)
        {
            this.InitializeComponent();
            this.neededValues = namesOfColumn;
            ValuesOfNewObject = new List<object>();
            CreateInterface(namesOfColumn, typesOfColumn);

        }
        private void CreateInterface(IEnumerable<string> namesOfColumn, IDictionary<int, string> typesOfColumn)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowSpacing = 10D;
            grid.ColumnSpacing = 10D;
            List<string> namesList = namesOfColumn.ToList();
           // List<string> typesList = typesOfColumn.ToList();
            foreach(string value in namesList)
            {
                int rowIndex = namesList.FindIndex(x => x == value);
                grid.RowDefinitions.Add(new RowDefinition());
                TextBlock newTextBlock = new TextBlock();
                TextBox newTextBox = new TextBox();
                grid.Children.Add(newTextBlock);
                grid.Children.Add(newTextBox);
                Grid.SetColumn(newTextBox, 1);
                newTextBlock.Width = newTextBox.Width = 120;
                newTextBlock.Text = value;
                Grid.SetRow(newTextBox, rowIndex);
                Grid.SetRow(newTextBlock, rowIndex);
                textBlocksDictionary.Add(rowIndex, newTextBlock);
                textBoxesDictionary.Add(rowIndex, newTextBox);

                TextBlock descriptionTextBlock = new TextBlock();
                descriptionTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                descriptionTextBlock.VerticalAlignment = VerticalAlignment.Center;
                descriptionTextBlock.HorizontalTextAlignment = TextAlignment.Center;
                Grid.SetColumn(descriptionTextBlock, 2);
                descriptionTextBlock.TextWrapping = TextWrapping.Wrap;
                descriptionTextBlock.FontSize = 12;
                grid.Children.Add(descriptionTextBlock);
                Grid.SetRow(descriptionTextBlock, rowIndex);

                // TODO: Dopisac pozostałe wyjatki dla pozostalych widokow
                if (typesOfColumn[rowIndex] == "date")
                    descriptionTextBlock.Text = "Format: RRRR-MM-DD";
                else if (value == "PESEL")
                    descriptionTextBlock.Text = "Format: 11 cyfr";
                else if (value == "Stan")
                    descriptionTextBlock.Text = "Możliwości: 'KRYTYCZNY', 'STABILNY', 'ZAGROŻONY', 'NULL'";
                else if (value == "Plec")
                    descriptionTextBlock.Text = "Możliwości: 'M', 'K'";
                else
                    descriptionTextBlock.Text = "Brak dodatkowych warunków";

                
            }
            grid.Children.Add(AlertText);
            grid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(AlertText, grid.RowDefinitions.Count - 1);
            Grid.SetColumnSpan(AlertText, 3);
        }
        private bool CheckDialogIsFilled()
        {
            bool response = true;
            foreach(TextBox box in textBoxesDictionary.Values)
            {
                if (string.IsNullOrEmpty(box.Text))
                {
                    response = false;
                    break;
                }
                    
            }
            if(textBlocksDictionary[0].Text == "PESEL")
            {
                int peselLength = textBoxesDictionary[0].Text.Length;
                if (peselLength != 11)
                {
                    response = false;
                    textBoxesDictionary[0].Text = string.Empty;
                }
                    
            }
            
            return response;
        }
        private void StoreData()
        {
            foreach (TextBox box in textBoxesDictionary.Values)
            {
                ValuesOfNewObject.Add(box.Text);
            }
            
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            bool isFilled = CheckDialogIsFilled();
            if (isFilled)
                StoreData();
            else
                AlertText.Text = "Wypełnij brakujące pola lub sprawdź poprawność danych!";
            args.Cancel = !isFilled;
                
            
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
