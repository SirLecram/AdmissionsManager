using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace AdmissionsManager.View
{
    public interface IDatabaseConnectable
    {
        ObservableCollection<object> RecordsList { get; }
        bool IsConnectedToDb { get; }
        bool IsDataLoaded { get; }
        Task<bool> ConnectToDatabase();
        Tabels GetModelType();
        void UnloadPage();
    }
}
