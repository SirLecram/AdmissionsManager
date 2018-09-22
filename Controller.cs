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
        public SqlCommand Command { get; protected set; }
        public Controller(Frame mainFrame)
        {
            MainFrame = mainFrame;
            _PatientList = new ObservableCollection<object>();
            MainFrame.Content = new AdmissionsPage(this);
        }

        public void ChangeFrame(Tabels changeTo)
        {
            
            switch (changeTo)
            {
                case Tabels.Admissions:
                    MainFrame.Content = new AdmissionsPage(this);
                    break;
                case Tabels.Patients:
                    PatientsPage page = new PatientsPage(this);
                    MainFrame.Content = page;
                    ReadDataFromDatabase(page);
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
        public async void ReadDataFromDatabase(IDatabaseConnectable sender)
        {
            
            string command = "SELECT * FROM Pacjenci";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                if(connection.State == ConnectionState.Open)
                {
                    
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        switch (sender.GetModelType())
                        {
                            case Tabels.Admissions:
                            case Tabels.Doctors:
                            case Tabels.Diagnoses:
                            case Tabels.Surgeries:
                            case Tabels.Rooms:

                                break;
                            case Tabels.Patients:
                                command = "SELECT * FROM Pacjenci";
                                break;
                        }
                        
                        cmd.CommandText = command;
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            int fieldCount = reader.FieldCount;
                            
                            while (await reader.ReadAsync())
                            {
                                List<object> valueList = new List<object>();
                                for (int i = 0; i < fieldCount; i++)
                                {
                                    valueList.Add(reader[i]);
                                }
                                /*switch(sender.GetTableLabel())
                                {
                                    case Tabels.Admissions:
                                    case Tabels.Doctors:
                                    case Tabels.Diagnoses:
                                    case Tabels.Surgeries:
                                    case Tabels.Rooms:

                                        break;
                                    case Tabels.Patients:

                                        break;
                                }*/
                               /* string pesel = reader.GetString(0);
                                string surname = reader.GetString(1);
                                string name = reader.GetString(2);
                                DateTime date = reader.GetDateTime(3);
                                string state = reader[4] as string;*/

                                
                                object patient =  new Patient(valueList);
                                //var patient = new Patient(pesel,surname,name,date,state,patientSex);
                                _PatientList.Add(patient);
                            }
                            
                            
                        }
                    }
                }
            }
        }
    }
}
