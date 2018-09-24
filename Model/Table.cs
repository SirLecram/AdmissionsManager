using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.Model
{
    public abstract class Table
    {
        // TODO: Dodac table do klas modelu
        public abstract string PrimaryKeyNameToSql { get; protected set; }
    }
}
