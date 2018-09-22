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
using AdmissionsManager.Model;

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
                    pageTitle.Text = "Pacjenci (Connected)";
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
        /*private async void ReadValuesFromDatabase()
{

}*/
    }
}
