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
        public async void ReadDataFromDatabase()
        {

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                if(connection.State == ConnectionState.Open)
                {
                    
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        
                        cmd.CommandText = CommandText;
                        // TODO: Dodac try (W razie jakby zapytanie bylo transact sql)
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            
                            int fieldCount = reader.FieldCount;
                            _PatientList.Clear();
                            while (await reader.ReadAsync())
                            {
                                List<object> valueList = new List<object>();
                                for (int i = 0; i < fieldCount; i++)
                                {
                                    valueList.Add(reader[i]);
                                }
                                
                                object patient =  new Patient(valueList);
                                //var patient = new Patient(pesel,surname,name,date,state,patientSex);
                                _PatientList.Add(patient);
                            }
                        }
                    }
                }
            }
        }
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

        public async Task<List<string>> GetColumnNamesFromTable()
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

        public async void DeleteRecord(object objectToDelete, Tabels actualTable)
        {
            string primaryKey = null;
            string primaryKeyName = null;
            string commandText = null;
            switch(actualTable)
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
            }

            if (int.TryParse(primaryKey, out int result))
                commandText = SqlCommandFilterCreator.CreateDeleteCommand(actualTable, result, primaryKeyName);
            else
                commandText = SqlCommandFilterCreator.CreateDeleteCommand(actualTable, primaryKey, primaryKeyName);

            int rowsAffected = await ExecuteTransactCommandOnDatabaseAsync(commandText);
            await new MessageDialog("Usunięto " + rowsAffected.ToString() +
                " rekordów z tabeli " + actualTable.GetTableDescription() + ".").ShowAsync();
            ReadDataFromDatabase();
        }
        public void ResetCommand(string orderBy, SortCriteria sortCriterium)
        {
            CommandText = SqlCommandFilterCreator.ResetCommand(_ActualPage);
            SortBy(orderBy, sortCriterium);
        }

        private bool CheckCenterTableContainsElement(Tabels primaryKeyTable, string primaryKey)
        {
            // TODO: Dokonczyc sprawdzanie czy usuwany element jest kluczem obcym i ewentualnie usunac
            // rowniez skojarzony wpis w innej tabeli.
            return true;
        }
    }
}
