using System;
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
using AdmissionsManager.ExtendedTypes;

namespace AdmissionsManager.Controlers
{
    public class Controler: INotifyPropertyChanged
    {
       // private Frame MainFrame { get; set; }
        //public readonly string ConnectionString = @"Data Source=MARCEL\SQLEXPRESS;Initial Catalog = DB_s439397; Integrated Security = true;";
        
        
        //TEST
        DatabaseReader DBReader { get; set; }
        private IDBInfoProvider DBInfoProvider { get; }
        //private ObservableCollection<object> _FullRecordsList { get; set; }
        private ObservableRangeCollection<ISqlTableModelable> _RecordsList { get; set; }
        public ObservableRangeCollection<ISqlTableModelable> RecordsList { get => _RecordsList; }
        public string CommandText { get; set; }
        private IPageNavigateable ActualPage { get; set; }
        public Dictionary<string, Type> EnumTypes { get; }
        public bool IsDataLoaded => (ActualPage as IDBConnectionStateGettable).IsDataLoaded;
        public Controler(Frame mainFrame, IDBInfoProvider dbInfoProvider)
        {
            //MainFrame = mainFrame;
            _RecordsList = new ObservableRangeCollection<ISqlTableModelable>();
            mainFrame.Content = new AdmissionsPage();
            
            ActualPage = mainFrame.Content as IPageNavigateable;
            CommandText = SqlCommandFilterCreator.CreateCommand(ActualPage as IDBConnectionStateGettable);
            EnumTypes = CreateEnumTypesDictionary();
            DBReader = new DatabaseReader(new Validators.Validator());
            DBInfoProvider = dbInfoProvider;
        }

        internal void SetActualPage(IPageNavigateable page)
        {
            //ActualPage.UnloadPage();
            _RecordsList.Clear();
            ActualPage = page;
            CommandText = SqlCommandFilterCreator.CreateCommand(ActualPage as IDBConnectionStateGettable);
            OnPropertyChanged("IsDataLoaded");
        } 

        #region Connection DB methods
        // TODO: Dodac opis metody aby uzywac ja tylko gdy baza jest podlaczona
        public async void ReadDataFromDatabase()
        {
            Type typeOfModel = (ActualPage as IHasDefaultModelType).GetDefaultModelType();
            bool isTrue = await DBReader.ReadDataFromDatabase((App.Current as App).ConnectionString, CommandText, typeOfModel);
            if (isTrue)
            {
                _RecordsList.Clear();
                _RecordsList.AddRange(DBReader.LastReadedModels);
            }
                
           /* using (SqlConnection connection = new SqlConnection((App.Current as App).ConnectionString))
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
                            ObservableCollection<ISqlTableModelable> collection = new ObservableCollection<ISqlTableModelable>();
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
                                switch ((ActualPage as IDBConnectionStateGettable).GetModelType())
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
                                 //_RecordsList.Add(model);
                                collection.Add(model);
                            }
                            _RecordsList.AddRange(collection);
                        }
                    }
                }
            }*/
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

        public async void DeleteRecordAsync(ISqlTableModelable objectToDelete, IDBConnectionStateGettable actualPage)
        {
            string primaryKey = (objectToDelete.GetPrimaryKey());
            string primaryKeyName = objectToDelete.GetPrimaryKeyName();
            string commandText = null;
            bool isCommandToExecute = false;

            if (!await CheckCenterTableContainsElement(primaryKey))
            {
                commandText = SqlCommandFilterCreator.CreateDeleteCommand(actualPage, primaryKey, primaryKeyName);
                isCommandToExecute = true;
            }

            if (isCommandToExecute)
            {
                int rowsAffected = await ExecuteTransactCommandOnDatabaseAsync(commandText);
                await new MessageDialog("Usunięto " + rowsAffected.ToString() +
                    " rekordów z tabeli " + actualPage.GetTableDescriptionToSql() + ".").ShowAsync();
                ReadDataFromDatabase();
            }
            else
            {
                await new MessageDialog("Nie usunięto rekordów.").ShowAsync();
            }

        }

        public async void EditRecordAsync(ISqlTableModelable objectToUpdate, string fieldToUpdate, string valueToUpdate)
        {
            
            string command = SqlCommandFilterCreator.CreateUpdateCommand((ActualPage as IDBConnectionStateGettable), 
                objectToUpdate.GetPrimaryKey(), objectToUpdate.GetPrimaryKeyName(),
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
                " rekordów z tabeli " + (ActualPage as IDBConnectionStateGettable).GetTableDescriptionToSql() + ".").ShowAsync();

        }
        public async void AddNewRecord(List<string> valuesList)
        {

            string command = SqlCommandFilterCreator.CreateNewRecordCommand((ActualPage as IDBConnectionStateGettable),
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
                " rekordów do tabeli " + (ActualPage as IDBConnectionStateGettable).GetTableDescriptionToSql() + ".").ShowAsync();
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
            string selectSqlString = SqlCommandFilterCreator.CreateCommand(ActualPage as IDBConnectionStateGettable);
            List<string> columNamesList = await DBInfoProvider.GetColumnNamesFromTableAsync(selectSqlString) as List<string>;
            /* using (SqlConnection connection = new SqlConnection((App.Current as App).ConnectionString))
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
             }*/
            return columNamesList;
        }
        public async Task<Dictionary<int, string>> GetColumnTypesAsync()
        {
            string selectSqlString = SqlCommandFilterCreator.CreateCommand(ActualPage as IDBConnectionStateGettable);
            Dictionary<int, string> columnTypesDictionary = 
                await DBInfoProvider.GetColumnTypeNamesAsync(selectSqlString) as Dictionary<int,string>;
            /*
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

            }*/
            return columnTypesDictionary;
        }
        private string GetForeignKeyNameFromAdmissionsTable()
        {
            Tabels table = (ActualPage.TableOfPage);
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
            string sqlCommand = SqlCommandFilterCreator.CreateCommand(new AdmissionsPage(), GetForeignKeyNameFromAdmissionsTable(),
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
                                string deleteCommand = SqlCommandFilterCreator.CreateDeleteCommand(Tabels.Admissions.GetEnumDescription(), primaryKey,
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
            CommandText = SqlCommandFilterCreator.ResetCommand(ActualPage as IDBConnectionStateGettable);
            SortBy(orderBy, sortCriterium);
        }

       /* private void GetPrimaryKeyAndPrimaryKeyName(SqlTable objectToGetValues, out string primaryKey, out string primaryKeyName)
        {
            Tabels actualTable = (ActualPage as IDBConnectionStateGettable).GetTableDescriptionToSql();
            primaryKey = null;
            primaryKeyName = null;
            primaryKey = (objectToGetValues as ISqlTableModelable).GetPrimaryKey();
            primaryKeyName = (objectToGetValues as ISqlTableModelable).GetPrimaryKeyName();
        }*/
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
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
        }
        #endregion

    }
}