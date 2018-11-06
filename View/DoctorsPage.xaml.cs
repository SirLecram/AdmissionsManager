using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using AdmissionsManager.Controlers;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace AdmissionsManager.View
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class DoctorsPage : Page, IDBConnectionStateGettable, IPageNavigateable, IHasDefaultModelType
    {
        private Controler DatabaseController;
        public ObservableCollection<ISqlTableModelable> RecordsList { get => DatabaseController.RecordsList; }
        public IDBInfoProvider DatabaseInfoProvider { get; set; }
        // TODO: Usunąc tableOfPAge
        public Tabels TableOfPage { get; }
        private Type DefaultModelType { get; set; }
        private bool _IsConnectedToDb { get; set; }
        public bool IsConnectedToDb { get => _IsConnectedToDb; }
        private bool _IsDataLoaded { get; set; }
        public bool IsDataLoaded { get => _IsDataLoaded; }

        public DoctorsPage()
        {
            this.InitializeComponent();
            
            pageTitle.Text = "Lekarze (Connecting to database...)";
            _IsConnectedToDb = false;
            TableOfPage = Tabels.Doctors;
            DefaultModelType = typeof(Model.Doctor);
        }

        public async Task<bool> ConnectToDatabaseAsync()
        {
            using (SqlConnection connection = new SqlConnection((App.Current as App).ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                }
                catch (Exception e)
                {
                    await new MessageDialog(e.Message, "Błąd połączenia z bazą danych.").ShowAsync();

                }

                if (connection.State == ConnectionState.Open)
                {
                    pageTitle.Text = "Lekarze (Connected)";
                    List<string> comboBoxesList = await DatabaseController.GetColumnNamesFromTableAsync();
                    lookInComboBox.ItemsSource = sortComboBox.ItemsSource = comboBoxesList;
                    //lookInComboBox.SelectedIndex = sortComboBox.SelectedIndex = 0;

                    return true;
                }
                else
                {
                    pageTitle.Text = "Lekarze (Can't connect to database)";
                    _IsDataLoaded = true;
                    DatabaseController.OnPropertyChanged("IsDataLoaded");
                    
                    return false;
                }

            }
        }

        public string GetTableDescriptionToSql()
        {
            return TableOfPage.GetEnumDescription();
        }

        #region Database management

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
            ISqlTableModelable model = databaseView.SelectedItem as ISqlTableModelable;

            DatabaseController.DeleteRecordAsync(model, this);
        }
        private async void EditRecord()
        {
            Model.Doctor doctor = databaseView.SelectedItem as Model.Doctor;
            string textToTitle = "Edytowany lekarz: " + doctor.Name + " " + doctor.Surname;
            Dictionary<int, string> typesDictionary = await DatabaseController.GetColumnTypesAsync();
            EditDialog dialog = new EditDialog(await DatabaseController.GetColumnNamesFromTableAsync(), typesDictionary, textToTitle,
                DatabaseController.EnumTypes);
            ContentDialogResult dialogResult = await dialog.ShowAsync();
            if (dialogResult == ContentDialogResult.Primary && !string.IsNullOrEmpty(dialog.Result))
            {
                string result = dialog.Result;
                string fieldToEdit = dialog.FieldToUpdate;
                DatabaseController.EditRecordAsync(doctor, fieldToEdit, result);
            }
        }
        private async void NewRecord()
        {
            Dictionary<int, string> typesDictionary = await DatabaseController.GetColumnTypesAsync();
            NewDialog createDialog = new NewDialog(await DatabaseController.GetColumnNamesFromTableAsync(), typesDictionary,
                DatabaseController.EnumTypes);
            ContentDialogResult dialogResult = await createDialog.ShowAsync();
            if (dialogResult == ContentDialogResult.Primary && createDialog.ValuesOfNewObject.Any())
            {
                List<string> valuesList = createDialog.ValuesOfNewObject;
                DatabaseController.AddNewRecord(valuesList);
            }
        }

        #endregion

        #region Events

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_IsConnectedToDb)
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (databaseView.SelectedItem != null)
                DeleteRecord();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (databaseView.SelectedItem != null)
                EditRecord();
        }

        private void NewRecordButton_Click(object sender, RoutedEventArgs e)
        {
            NewRecord();
        }

        #endregion

        #region Navigation between pages

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DatabaseController = e.Parameter as Controler;
            _IsDataLoaded = false;
            DatabaseController.OnPropertyChanged("IsDataLoaded");
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bool IsConnectionAvailable;
            if (IsConnectionAvailable = await ConnectToDatabaseAsync())
            {
                databaseView.ItemsSource = RecordsList;
                _IsConnectedToDb = IsConnectionAvailable;
                lookInComboBox.SelectedIndex = 0;
                sortComboBox.SelectedIndex = 0;
                


            }
            _IsDataLoaded = true;
            DatabaseController.OnPropertyChanged("IsDataLoaded");
        }
        public void UnloadPage()
        {
            _IsConnectedToDb = false;
            databaseView.ItemsSource = null;
            lookInComboBox.ItemsSource = null;
            sortComboBox.ItemsSource = null;
            _IsDataLoaded = false;
            //DatabaseController.OnPropertyChanged("IsDataLoaded");
        }

        public Type GetDefaultModelType()
        {
            return DefaultModelType;
        }

        #endregion
        
    }
}
