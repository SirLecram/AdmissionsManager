using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.View
{
    public interface IDatabaseConnectable
    {
        void ConnectToDatabase();
        Tabels GetModelType();
    }
}
