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
        private IEnumerable<string> neededValues;

        public NewDialog(IEnumerable<string> neededValues, IEnumerable<string> typesOfColumn)
        {
            this.InitializeComponent();
            this.neededValues = neededValues;
            CreateInterface(neededValues, typesOfColumn);

        }
        private void CreateInterface(IEnumerable<string> neededValues, IEnumerable<string> typesOfColumn)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowSpacing = 10D;
            grid.ColumnSpacing = 10D;
            List<string> valuesList = neededValues.ToList();
            List<string> typesList = typesOfColumn.ToList();
            foreach(string value in valuesList)
            {
                int rowIndex = valuesList.FindIndex(x => x == value);
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
                if (typesList[rowIndex] == "date")
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
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
