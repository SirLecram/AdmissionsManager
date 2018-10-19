using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.View
{
    public interface IDatabaseConnectable
    {
        ObservableCollection<object> RecordsList { get; }
        bool IsConnectedToDb { get; }
        void ConnectToDatabase();
        Tabels GetModelType();
    }
}
