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
    public sealed partial class EditDialog : ContentDialog
    {
        public string Result { get; private set; }
        public string FieldToUpdate { get; private set; }
        public EditDialog(IEnumerable<string> comboboxSource, string editedFieldToTitle)
        {
            this.InitializeComponent();
            fieldToEdit.ItemsSource = comboboxSource;
            fieldToEdit.SelectedIndex = 0;
            Title = editedFieldToTitle;
        }
        public void AddNextField()
        {
            // TODO: Dodać możliwośc edycji wielu pól na raz
            /*
            grid.RowDefinitions.Add(new RowDefinition());
            ComboBox comboboxToAdd = new ComboBox();
            grid.Children.Add(comboboxToAdd);
            comboboxToAdd.ItemsSource = fieldToEdit.ItemsSource;
            comboboxToAdd.SetValue(Grid.RowProperty, 1);
            comboboxToAdd.SetValue(Grid.ColumnProperty, 0);
            comboboxToAdd.HorizontalAlignment = HorizontalAlignment.Stretch;
            comboboxToAdd.Margin = new Thickness(20);
            //grid.Children.Add(comboboxToAdd);*/

            
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = valueToUpdateTextBox.Text;
            FieldToUpdate = fieldToEdit.SelectedItem.ToString();
           // return Result;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddNextField();
        }
    }
}
