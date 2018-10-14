using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager
{
    public enum Tabels
    {
        /// <summary>
        /// UWAGA !! DODAC ROZSZERZENIE GetDescription
        /// </summary>
        [Description("Strona domowa")]
        Home = 0,
        [Description("Przyjecia")]
        Admissions = 1,
        [Description("Pacjenci")]
        Patients,
        [Description("Lekarze")]
        Doctors,
        [Description("Diagnozy")]
        Diagnoses,
        [Description("Operacje i zabiegi")]
        Surgeries,
        [Description("Sale chorych")]
        Rooms,
    }

    public enum Sex
    {
        K, M,
    }

    public enum SortCriteria
    {
        Ascending,
        Descending,
    }
}
