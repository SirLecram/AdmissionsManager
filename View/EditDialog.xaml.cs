﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
        private Dictionary<int, ComboBox> comboBoxes = new Dictionary<int, ComboBox>();
        private Dictionary<int, Control> controlsDictionary = new Dictionary<int, Control>();
        private Dictionary<int, string> _TypesDictionary { get; set; }
        private Dictionary<string, Type> enumTypes;
        public EditDialog(IEnumerable<string> comboboxSource, IDictionary<int, string> typesDictionary, string editedFieldToTitle,
            IDictionary<string, Type> enumTypesDictionary)
        {
            this.InitializeComponent();
            (comboboxSource as List<string>).RemoveAt(0);
            (typesDictionary as Dictionary<int, string>).Remove(0);
            enumTypes = enumTypesDictionary as Dictionary<string, Type>;
            CreateBasicInterface(comboboxSource, editedFieldToTitle);
            _TypesDictionary = typesDictionary as Dictionary<int, string>;
            fieldToEdit.SelectedIndex = 0;
            
        }
        private void CreateBasicInterface(IEnumerable<string> comboboxSource, string editedFieldToTitle)
        {
            Title = editedFieldToTitle;
            
            TextBox textBox = new TextBox();
            textBox.VerticalAlignment = VerticalAlignment.Center;
            textBox.Width = 170;
            textBox.Margin = new Thickness(20D);
            firstValueStackPanel.Children.Add(textBox);
            Button btn = new Button();
            btn.Content = "+";
            firstValueStackPanel.Children.Add(btn);
            comboBoxes.Add(0, fieldToEdit);
            controlsDictionary.Add(0, textBox);
            fieldToEdit.ItemsSource = comboboxSource;
            
            
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
        /*private void StoreData() // TODO: Dodac tę funkcję
        {
            foreach (Control control in controlsDictionary.Values)
            {
                if (control is TextBox)
                    ValuesOfNewObject.Add((control as TextBox).Text);
                else
                    ValuesOfNewObject.Add((control as ComboBox).SelectedItem as string);
            }

        }*/
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (controlsDictionary[0] is ComboBox)
                Result = (controlsDictionary[0] as ComboBox).SelectedItem.ToString();
            else
                Result = (controlsDictionary[0] as TextBox).Text;
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

        private void FieldToEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = fieldToEdit.SelectedIndex;
            string selectedText = fieldToEdit.SelectedItem.ToString();
            string typeOfField = _TypesDictionary[selectedIndex+1];
            int changeInRow = Grid.GetRow(sender as ComboBox);

            // TODO: Dodać warunki co do pozostałych tabel, COMBOBOX JUZ JEST GENEROWANY!
            if (typeOfField == "date")
                additionalFormatInfo.Text = "Format: RRRR-MM-DD";
            else
                additionalFormatInfo.Text = "Format: brak wymagań";

            if (enumTypes.Keys.ToList().Contains(selectedText))
            {
                // TODO: Dodać moze datapicker dla wartosci datowych ?
                ComboBox cBox = new ComboBox();
                Type type = enumTypes[selectedText];
                List<string> list = new List<string>(Enum.GetNames(enumTypes[selectedText]).ToList());
                List<string> descriptionsList = new List<string>();
                foreach (string x in list)
                {
                    var enumX = Enum.Parse(type, x);
                    descriptionsList.Add((enumX as Enum).GetEnumDescription());

                }
                cBox.VerticalAlignment = VerticalAlignment.Center;
                cBox.Width = 170;
                cBox.Margin = new Thickness(20D);
                cBox.ItemsSource = descriptionsList;
                cBox.SelectedIndex = 0;
                firstValueStackPanel.Children.Remove(controlsDictionary[changeInRow]);
                controlsDictionary[changeInRow] = cBox;
                firstValueStackPanel.Children.Insert(0, controlsDictionary[changeInRow]);
                additionalFilterInfo.Text = "Wybierz z dostępnych wartości";
                //firstValueStackPanel.Children.Add(controlsDictionary[changeInRow]);
                //additionalFilterInfo.Text = "Możliwości: 'KRYTYCZNY', 'STABILNY', 'ZAGROŻONY', 'NULL'";
            }
            else
            {
                TextBox tBox = new TextBox();
                tBox.VerticalAlignment = VerticalAlignment.Center;
                tBox.Width = 170;
                tBox.Margin = new Thickness(20D);
                firstValueStackPanel.Children.Remove(controlsDictionary[changeInRow]);
                controlsDictionary[changeInRow] = tBox;
                firstValueStackPanel.Children.Insert(0, controlsDictionary[changeInRow]);
                additionalFilterInfo.Text = "Brak dodatkowych wymagań";
            }
                

            if (typeOfField == "varchar")
                additionalTypeInfo.Text = "Typ: " + "text";
            else
                additionalTypeInfo.Text = "Typ: " + typeOfField;
        }
    }
}
