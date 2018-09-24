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
using System.Data;
using System.Data.SqlClient;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace AdmissionsManager.View
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class PatientsPage : Page, IDatabaseConnectable
    {
        private Controller DatabaseController;
        public ObservableCollection<object> PatientList { get => DatabaseController.PatientList; }
        private Tabels TableOfPage { get; }
        

        public PatientsPage(Controller dbController)
        {
            this.InitializeComponent();
            pageTitle.Text = "Pacjenci (Connecting to database...)";
            DatabaseController = dbController;
            ConnectToDatabase();
            TableOfPage = Tabels.Patients;
            
            databaseView.ItemsSource = PatientList;
        }
        private async void ConnectToDatabase()
        {
            using (SqlConnection connection = new SqlConnection(DatabaseController.ConnectionString))
            {
                await connection.OpenAsync();
                if (connection.State == ConnectionState.Open)
                {
                    pageTitle.Text = "Pacjenci (Connected)";
                    List<string> comboBoxesList = await DatabaseController.GetColumnNamesFromTable();
                    lookInComboBox.ItemsSource = sortComboBox.ItemsSource = comboBoxesList;
                    lookInComboBox.SelectedIndex = sortComboBox.SelectedIndex = 0;
                }
                else
                {
                    pageTitle.Text = "Pacjenci (Can't connect to database)";
                }
            }
        }
        
        void IDatabaseConnectable.ConnectToDatabase()
        {
            ConnectToDatabase();
        }

        public Tabels GetModelType()
        {
            return TableOfPage;
        }

        private void Sort()
        {
            SortCriteria criterium;
            if ((bool)radioBtn1.IsChecked)
                criterium = SortCriteria.Ascending;
            else
                criterium = SortCriteria.Descending;
            DatabaseController.SortBy(sortComboBox.SelectedItem.ToString(), criterium);
        }
        private void Search()
        {
            SortCriteria criterium;
            if ((bool)radioBtn1.IsChecked)
                criterium = SortCriteria.Ascending;
            else
                criterium = SortCriteria.Descending;
            string searchIn = lookInComboBox.SelectedItem.ToString();
            string searchedExpression = searchBox.Text;
            string sortBy = sortComboBox.SelectedItem.ToString();
            DatabaseController.SearchExpression(searchIn, searchedExpression, sortBy, criterium);
        }
        private void DeleteRecord()
        {
            object patient = databaseView.SelectedItem;
            
            DatabaseController.DeleteRecord(patient, GetModelType());
        }


        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sort();
        }

        private void RadionBtn_Click(object sender, RoutedEventArgs e)
        {
            Sort();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            lookInComboBox.SelectedIndex = 0;
            searchBox.Text = string.Empty;
            SortCriteria criterium;
            if ((bool)radioBtn1.IsChecked)
                criterium = SortCriteria.Ascending;
            else
                criterium = SortCriteria.Descending;
            DatabaseController.ResetCommand(sortComboBox.SelectedItem.ToString(), criterium);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(databaseView.SelectedItem != null)
                DeleteRecord();
        }
        /*private async void ReadValuesFromDatabase()
{

}*/
    }
}
