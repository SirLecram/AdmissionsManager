﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using AdmissionsManager.View;
using AdmissionsManager.Model;
using Windows.UI.Popups;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace AdmissionsManager.Controlers
{
    public class Controler: INotifyPropertyChanged
    {
        private Frame MainFrame { get; set; }
        //public readonly string ConnectionString = @"Data Source=MARCEL\SQLEXPRESS;Initial Catalog = DB_s439397; Integrated Security = true;";
        
        

        //private ObservableCollection<object> _FullRecordsList { get; set; }
        private ObservableCollection<ISqlTableModelable> _RecordsList { get; set; }
        public ObservableCollection<ISqlTableModelable> RecordsList { get => _RecordsList; }
        public string CommandText { get; protected set; }
        private IDatabaseConnectable _ActualPage { get; set; }
        public Dictionary<string, Type> EnumTypes { get; }
        public bool IsDataLoaded => _ActualPage.IsConnectedToDb;
        public Controler(Frame mainFrame)
        {
            MainFrame = mainFrame;
            _RecordsList = new ObservableCollection<ISqlTableModelable>();
            MainFrame.Content = new AdmissionsPage(this);
            
            _ActualPage = MainFrame.Content as IDatabaseConnectable;
            EnumTypes = CreateEnumTypesDictionary();
        }

      /*  internal void ChangeFrame(Tabels changeTo)
        {
            
            CommandText = SqlCommandFilterCreator.ResetCommand(changeTo);
            _RecordsList.Clear();
            // TODO: UZUPELNIC W PRZYPADKU DODAWANIA KOLEJNYCH MODELI I WIDOKOW + 
            //Do usuniecia stare zmienianie strony
            // TODO: Zablokować możliwość ciagłego ładowania jednej strony
            IDatabaseConnectable page;
            _ActualPage.UnloadPage();
            

            switch (changeTo)
            {
                case Tabels.Admissions:
                    page = new AdmissionsPage(this);
                    MainFrame.Content = page;

                    break;
                case Tabels.Patients:
                    
                    this.MainFrame.Navigate(typeof(PatientsPage), this);
                    _ActualPage = MainFrame.Content as IDatabaseConnectable;
                    
                   /* if ((page as IDatabaseConnectable).IsConnectedToDb)
                        ReadDataFromDatabase();
                    break;
                case Tabels.Doctors:
                    
                    this.MainFrame.Navigate(typeof(DoctorsPage), this);
                    _ActualPage = MainFrame.Content as IDatabaseConnectable;

                    break;
                case Tabels.Diagnoses:
                    page = new AdmissionsPage(this);
                    MainFrame.Content = page;

                    break;
                case Tabels.Surgeries:
                    page = new AdmissionsPage(this);
                    MainFrame.Content = page;
                    break;
                case Tabels.Rooms:
                    page = new AdmissionsPage(this);
                    MainFrame.Content = page;
                    break;
                default:
                    //page = new PatientsPage(this);
                    page = new PatientsPage() as IDatabaseConnectable;
                    break;
            }
            CommandText = SqlCommandFilterCreator.CreateCommand(_ActualPage);

        } */

        #region Connection DB methods
        // TODO: Dodac opis metody aby uzywac ja tylko gdy baza jest podlaczona
        public async void ReadDataFromDatabase()
        {

            using (SqlConnection connection = new SqlConnection((App.Current as App).ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                }
                catch (Exception e)
                {
                    throw new Exception("Nie ma możliwości połączyć się z DB.");
                }
                if (connection.State == ConnectionState.Open)
                {

                    using (SqlCommand cmd = connection.CreateCommand())
                    {

                        cmd.CommandText = CommandText;
                        // TODO: Dodac try (W razie jakby zapytanie bylo transact sql)
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {

                            int fieldCount = reader.FieldCount;
                            _RecordsList.Clear();
                            // TODO: Dodac try w razie blednej daty w wyszukiwaniu
                            while (await reader.ReadAsync())
                            {
                                List<string> valueList = new List<string>();
                                for (int i = 0; i < fieldCount; i++)
                                {
                                    if (String.IsNullOrEmpty(reader[i].ToString()))
                                        valueList.Add("NULL");
                                    else
                                        valueList.Add(reader[i].ToString());
                                }
                                ISqlTableModelable model = null;
                                switch (_ActualPage.GetModelType())
                                {
                                    case Tabels.Admissions:
                                        model = new Patient(valueList);
                                        break;
                                    case Tabels.Patients:
                                        model = new Patient(valueList);
                                        break;
                                    case Tabels.Doctors:
                                        model = new Doctor(valueList);
                                        break;
                                    case Tabels.Diagnoses:
                                        model = new Doctor(valueList);
                                        break;
                                    case Tabels.Surgeries:
                                        model = new Doctor(valueList);

                                        break;
                                    case Tabels.Rooms:
                                        model = new Doctor(valueList);
                                        break;
                                }

                                //var patient = new Patient(pesel,surname,name,date,state,patientSex);
                                _RecordsList.Add(model);
                            }
                        }
                    }
                }
            }
        }
        // TODO: Dodac opis metody aby uzywac ja tylko gdy baza jest podlaczona
        public async Task<int> ExecuteTransactCommandOnDatabaseAsync(string commandText)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection((App.Current as App).ConnectionString))
            {
                await connection.OpenAsync();
                if (connection.State == ConnectionState.Open)
                {
                    // TODO: Dodac try w raiez jakby zapytanie bylo select itp;
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = commandText;

                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            return rowsAffected;
        }
        #endregion

        #region DB mangement methods

        public async void DeleteRecordAsync(object objectToDelete, Tabels actualTable)
        {
            string primaryKey = null;
            string primaryKeyName = null;
            GetPrimaryKeyAndPrimaryKeyName(objectToDelete as SqlTable, out primaryKey, out primaryKeyName);
            string commandText = null;
            bool isCommandToExecute = false;

            if (!await CheckCenterTableContainsElement(primaryKey))
            {
                commandText = SqlCommandFilterCreator.CreateDeleteCommand(actualTable, primaryKey, primaryKeyName);
                isCommandToExecute = true;
            }

            if (isCommandToExecute)
            {
                int rowsAffected = await ExecuteTransactCommandOnDatabaseAsync(commandText);
                await new MessageDialog("Usunięto " + rowsAffected.ToString() +
                    " rekordów z tabeli " + actualTable.GetEnumDescription() + ".").ShowAsync();
                ReadDataFromDatabase();
            }
            else
            {
                await new MessageDialog("Nie usunięto rekordów.").ShowAsync();
            }

        }

        public async void EditRecordAsync(object objectToUpdate, string fieldToUpdate, string valueToUpdate)
        {
            GetPrimaryKeyAndPrimaryKeyName(objectToUpdate as SqlTable, out string primaryKey, out string primaryKeyName);
            string command = SqlCommandFilterCreator.CreateUpdateCommand(_ActualPage.GetModelType(), primaryKey, primaryKeyName,
                new List<string> { fieldToUpdate }, new List<string> { valueToUpdate });

            int rowsAffected = 0;
            try
            {
                rowsAffected = await ExecuteTransactCommandOnDatabaseAsync(command);
            }
            catch (SqlException e)
            {
                await new MessageDialog(e.Message, "Błędny format danych").ShowAsync();
            }
            ReadDataFromDatabase();
            await new MessageDialog("Zaktualizowano " + rowsAffected.ToString() +
                " rekordów z tabeli " + _ActualPage.GetModelType().GetEnumDescription() + ".").ShowAsync();

        }
        public async void AddNewRecord(List<string> valuesList)
        {

            string command = SqlCommandFilterCreator.CreateNewRecordCommand(_ActualPage.GetModelType(),
                valuesList, await GetColumnNamesFromTableAsync());
            int rowsAffected = 0;
            try
            {
                rowsAffected = await ExecuteTransactCommandOnDatabaseAsync(command);
            }
            catch (SqlException e)
            {
                await new MessageDialog(e.Message, "Błędny format danych!").ShowAsync();
            }
            ReadDataFromDatabase();
            await new MessageDialog("Dodano " + rowsAffected.ToString() +
                " rekordów do tabeli " + _ActualPage.GetModelType().GetEnumDescription() + ".").ShowAsync();
        }

        #endregion

        #region Filters and searching

        public void SortBy(string orderBy, SortCriteria sortCriterium)
        {
            /*var newCollection = from model in _FullRecordsList
                                orderby (model as Patient).PeselNumber ascending
                                select model;
            newCollection.ToList();
            _RecordsList = newCollection as ObservableCollection<object>;*/
            CommandText = SqlCommandFilterCreator.CreateCommand(CommandText, orderBy, sortCriterium);
            ReadDataFromDatabase();
        }

        public void SearchExpression(string searchIn, string searchedExpression,
            string sortBy = null, SortCriteria sortCriterium = SortCriteria.Ascending)
        {
            CommandText = SqlCommandFilterCreator.CreateCommand(CommandText, searchIn, searchedExpression, sortBy, sortCriterium);
            ReadDataFromDatabase();
        }

        #endregion

        #region DB info methods

        public async Task<List<string>> GetColumnNamesFromTableAsync()
        {
            using (SqlConnection connection = new SqlConnection((App.Current as App).ConnectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = CommandText;
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        int columnAmount = reader.FieldCount;
                        List<string> columnNames = new List<string>();
                        for (int i = 0; i < columnAmount; i++)
                        {
                            columnNames.Add(reader.GetName(i));

                        }
                        return columnNames;
                    }
                }
            }
        }
        public async Task<Dictionary<int, string>> GetColumnTypesAsync()
        {
            using (SqlConnection connection = new SqlConnection((App.Current as App).ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = CommandText;
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        int columnAmount = reader.FieldCount;
                        Dictionary<int, string> typesDictionary = new Dictionary<int, string>();
                        for (int i = 0; i < columnAmount; i++)
                        {
                            typesDictionary.Add(i, reader.GetDataTypeName(i));
                        }
                        return typesDictionary;
                    }
                }

            }
        }
        private string GetForeignKeyNameFromAdmissionsTable()
        {
            Tabels table = _ActualPage.GetModelType();
            string stringToReturn = null;
            switch (table)
            {
                case Tabels.Admissions:
                    stringToReturn = null;
                    break;
                case Tabels.Patients:
                    stringToReturn = "PESEL_pacjenta";
                    break;
                case Tabels.Doctors:
                    stringToReturn = "Lekarz_prowadzacy";
                    break;
                case Tabels.Diagnoses:
                    stringToReturn = "Symbol_diagnozy";
                    break;
                case Tabels.Surgeries:
                    stringToReturn = "Planowana_operacja";

                    break;
                case Tabels.Rooms:
                    stringToReturn = "Nr_sali";
                    break;
            }

            return stringToReturn;
        }

        #endregion

        #region Help methods

        private async Task<bool> CheckCenterTableContainsElement(string primaryKey)
        {
            string sqlCommand = SqlCommandFilterCreator.CreateCommand(new AdmissionsPage(this), GetForeignKeyNameFromAdmissionsTable(),
                primaryKey);
            using (SqlConnection connection = new SqlConnection((App.Current as App).ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sqlCommand;
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            IUICommand response = null;
                            MessageDialog mDialog = new MessageDialog("W tabeli Przyjęcia znajduje się odwołanie do usuwanego rekordu." +
                                    "Jeśli usuniesz ten rekord, wpis z tabeli Przyjęcia zostanie również usunięty. Czy nadal chcesz usunąć rekord?");
                            mDialog.Commands.Add(new UICommand("Tak, usuń"));
                            mDialog.Commands.Add(new UICommand("Nie, pozostaw rekord"));
                            response = await mDialog.ShowAsync();

                            if (response == mDialog.Commands.First())
                            {
                                string deleteCommand = SqlCommandFilterCreator.CreateDeleteCommand(Tabels.Admissions, primaryKey,
                                    GetForeignKeyNameFromAdmissionsTable());
                                int rowsAffected = await ExecuteTransactCommandOnDatabaseAsync(deleteCommand);
                                if (rowsAffected == 0)
                                {
                                    throw new Exception("NIE USUNIETO REKORDU!");
                                }
                                return false;
                            }
                            return true;
                        }

                    }
                }

            }
            return false;
        }
        private async Task<bool> CheckCenterTableContainsElement(int primaryKey)
        {
            string stringToSearch = primaryKey.ToString();
            bool response = await CheckCenterTableContainsElement(stringToSearch);
            return response;
        }

        public void ResetCommand(string orderBy, SortCriteria sortCriterium)
        {
            CommandText = SqlCommandFilterCreator.ResetCommand(_ActualPage);
            SortBy(orderBy, sortCriterium);
        }

        private void GetPrimaryKeyAndPrimaryKeyName(SqlTable objectToGetValues, out string primaryKey, out string primaryKeyName)
        {
            Tabels actualTable = _ActualPage.GetModelType();
            primaryKey = null;
            primaryKeyName = null;
            primaryKey = (objectToGetValues as ISqlTableModelable).GetPrimaryKey();
            primaryKeyName = (objectToGetValues as ISqlTableModelable).GetPrimaryKeyName();
        }
        private Dictionary<string, Type> CreateEnumTypesDictionary()
        {
            // TODO: Uzupełniać w miare dodawania tabel!
            Dictionary<string, Type> newDictionary = new Dictionary<string, Type>();
            newDictionary.Add("Plec", typeof(Sex));
            newDictionary.Add("Stan", typeof(PatientState));
            newDictionary.Add("Stopien_naukowy", typeof(AcademicDegrees));
            newDictionary.Add("Specjalizacja", typeof(MedicalSpecializations));
            newDictionary.Add("Stanowisko", typeof(JobPositions));
            return newDictionary;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

    }
}