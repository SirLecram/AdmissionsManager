using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.Model
{
    class Table
    {
        public string TableName { get; protected set; }
        public Tabels TableType { get; protected set; }

        public Table(Tabels typeOfTable)
        {
            TableType = typeOfTable;
            TableName = typeOfTable.ToString();
        }
    }
}
