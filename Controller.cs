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

namespace AdmissionsManager
{
    public class Controller
    {
        private readonly Frame MainFrame;
        public readonly string ConnectionString = @"Data Source=MARCEL\SQLEXPRESS;Initial Catalog = DB_s439397; Integrated Security = true;";
        private ObservableCollection<object> _PatientList { get; set; }
        public ObservableCollection<object> PatientList { get => _PatientList; }
        public string CommandText { get; protected set; }
        private IDatabaseConnectable _ActualPage { get; set; }
        public Controller(Frame mainFrame)
        {
            MainFrame = mainFrame;
            _PatientList = new ObservableCollection<object>();
            MainFrame.Content = new AdmissionsPage(this);
        }

        internal void ChangeFrame(Tabels changeTo)
        {

            switch (changeTo)
            {
                case Tabels.Admissions:
                    MainFrame.Content = new AdmissionsPage(this);

                    break;
                case Tabels.Patients:
                    PatientsPage page = new PatientsPage(this);
                    MainFrame.Content = page;
                    _ActualPage = page;
                    CommandText = SqlCommandFilterCreator.CreateCommand(_ActualPage);
                    if((page as PatientsPage).IsConnectedToDb)
                        ReadDataFromDatabase();
                    break;
                case Tabels.Doctors:
                    MainFrame.Content = new DoctorsPage(this);
                    break;
                case Tabels.Diagnoses:
                    MainFrame.Content = new DiagnosisPage(this);

                    break;
                case Tabels.Surgeries:
                    MainFrame.Content = new SurgeriesPage(this);
                    break;
                case Tabels.Rooms:
                    MainFrame.Content = new RoomsPage(this);
                    break;
            }
        }

        #region Connection DB methods
        // TODO: Dodac opis metody aby uzywac ja tylko gdy baza jest podlaczona
        public async void ReadDataFromDatabase()
        {

            using (SqlConnection connection = new SqlConnection(ConnectionString))
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
                            _PatientList.Clear();
                            // TODO: Dodac try w razie blednej daty w wyszukiwaniu
                            while (await reader.ReadAsync())
                            {
                                List<object> valueList = new List<object>();
                                for (int i = 0; i < fieldCount; i++)
                                {
                                    valueList.Add(reader[i]);
                                }

                                object patient = new Patient(valueList);
                                //var patient = new Patient(pesel,surname,name,date,state,patientSex);
                                _PatientList.Add(patient);
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
            using (SqlConnection connection = new SqlConnection(ConnectionString))
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
            GetPrimaryKeyAndPrimaryKeyName(objectToDelete, out primaryKey, out primaryKeyName);
            string commandText = null;
            /*switch (actualTable)
            {
                // TODO: Uzupełnić Switch()
                case Tabels.Admissions:

                    break;
                case Tabels.Patients:
                    primaryKey = (objectToDelete as Patient).PeselNumber;
                    primaryKeyName = (objectToDelete as Patient).PrimaryKeyNameToSql;

                    break;
                case Tabels.Doctors:

                    break;
                case Tabels.Diagnoses:

                    break;
                case Tabels.Surgeries:

                    break;
                case Tabels.Rooms:

                    break;
            }*/
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
                    " rekordów z tabeli " + actualTable.GetTableDescription() + ".").ShowAsync();
                ReadDataFromDatabase();
            }
            else
            {
                await new MessageDialog("Nie usunięto rekordów.").ShowAsync();
            }

        }

        public async void EditRecordAsync(object objectToUpdate, string fieldToUpdate, string valueToUpdate)
        {
            GetPrimaryKeyAndPrimaryKeyName(objectToUpdate, out string primaryKey, out string primaryKeyName);
            string command = SqlCommandFilterCreator.CreateUpdateCommand(_ActualPage.GetModelType(), primaryKey, primaryKeyName,
                new List<string> { fieldToUpdate }, new List<string> { valueToUpdate });
            
            int rowsAffected = 0;
            try
            {
                rowsAffected = await ExecuteTransactCommandOnDatabaseAsync(command);
            }
            catch(SqlException e)
            {
                await new MessageDialog(e.Message, "Błędny format danych").ShowAsync();
            }
            
            await new MessageDialog("Zaktualizowano " + rowsAffected.ToString() +
                " rekordów z tabeli " + _ActualPage.GetModelType().GetTableDescription() + ".").ShowAsync();
            ReadDataFromDatabase();
        }
        #endregion

        #region Filters and searching

        public void SortBy(string orderBy, SortCriteria sortCriterium)
        {
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
            using (SqlConnection connection = new SqlConnection(ConnectionString))
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
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = CommandText;
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        int columnAmount = reader.FieldCount;
                        Dictionary<int, string> typesDictionary = new Dictionary<int, string>();
                        for(int i = 0; i < columnAmount; i++)
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
            // TODO: Dokonczyc sprawdzanie czy usuwany element jest kluczem obcym i ewentualnie usunac
            // rowniez skojarzony wpis w innej tabeli.
            string sqlCommand = SqlCommandFilterCreator.CreateCommand(new AdmissionsPage(this), GetForeignKeyNameFromAdmissionsTable(),
                primaryKey);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
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
        
        private void GetPrimaryKeyAndPrimaryKeyName(object objectToGetValues, out string primaryKey, out string primaryKeyName)
        {

            
            Tabels actualTable = _ActualPage.GetModelType();
            primaryKey = null;
            primaryKeyName = null;
            switch (actualTable)
            {
                // TODO: Uzupełnić Switch()
                case Tabels.Admissions:

                    break;
                case Tabels.Patients:
                    primaryKey = (objectToGetValues as Patient).PeselNumber;
                    primaryKeyName = (objectToGetValues as Patient).PrimaryKeyNameToSql;

                    break;
                case Tabels.Doctors:

                    break;
                case Tabels.Diagnoses:

                    break;
                case Tabels.Surgeries:

                    break;
                case Tabels.Rooms:

                    break;
            }
        }
        #endregion

    }
}
