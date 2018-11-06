using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class AdmissionsPage : Page, IDBConnectionStateGettable, IPageNavigateable
    {
        private Controler DatabaseController;
        private bool _IsDataLoaded { get; set; }
        public bool IsDataLoaded { get => true; }
        public Tabels TableOfPage { get; }
        public IDBInfoProvider DatabaseInfoProvider { get; set; }

        public AdmissionsPage(/*Controler dbController*/)
        {
            this.InitializeComponent();
            _IsDataLoaded = true;
            TableOfPage = Tabels.Admissions;
            //DatabaseController.OnPropertyChanged("IsDataLoaded");
            //DatabaseController = dbController;

        }

        public ObservableCollection<ISqlTableModelable> RecordsList => throw new NotImplementedException();

        public bool IsConnectedToDb => true;
        /*private bool test = false;
        public bool IsDataLoaded => test;*/

        public async Task<bool> ConnectToDatabaseAsync()
        {
            return true;
        }

        public string GetTableDescriptionToSql()
        {
            
            return Tabels.Admissions.GetEnumDescription();
        }

        public void UnloadPage()
        {
            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DatabaseController = e.Parameter as Controler;
            _IsDataLoaded = true;
           // DatabaseController.OnPropertyChanged("IsDataLoaded");
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*test = !test;
            DatabaseController.OnPropertyChanged("IsDataLoaded");*/
        }
        
    }
}
