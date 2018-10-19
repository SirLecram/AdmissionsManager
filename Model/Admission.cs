using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.Model
{
    class Admission : Table
    {

        public Admission()
        {
        }

        public override string PrimaryKeyNameToSql { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public override string GetPrimaryKey => throw new NotImplementedException();
    }
}
