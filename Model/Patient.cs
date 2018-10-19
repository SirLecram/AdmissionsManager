using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.Model
{
    public class Patient : Table
    {
        public override string PrimaryKeyNameToSql { get; protected set; }
        public override string GetPrimaryKey => PeselNumber;
        public string PeselNumber { get; protected set; }
        public string Surname { get; protected set; }
        public string Name { get; protected set; }
        public DateTime BirthDate { get; protected set; }
        private PatientState _PatientState { get; set; }
        public string PatientState { get
            {
                if (_PatientState.GetEnumDescription() == "NULL")
                    return string.Empty;
                else
                    return _PatientState.GetEnumDescription();
            }
        }
        private Sex _PatientSex { get; set; }
        public string PatientSex { get => _PatientSex.GetEnumDescription(); }
        


        public Patient() : base()
        {
            PrimaryKeyNameToSql = "PESEL";
        }
        public Patient(string pesel, string surname, string name, DateTime birthDate, PatientState patientState,
            Sex patientSex) : base()
        {
            if (pesel.Length < 11 || pesel.Length > 11)
                throw new FormatException("PESEL musi mieć 11 cyfr");
            PeselNumber = pesel;
            Surname = surname;
            Name = name;
            BirthDate = birthDate;
            _PatientState = patientState;
            _PatientSex = patientSex;
            PrimaryKeyNameToSql = "PESEL";
        }
        /// <summary>
        /// List have to be in right order (pesel, surname, name, birth date, patient state, patient sex).
        /// </summary>
        /// <param name="listOfValues"></param>
        public Patient(List<string> listOfValues) : base()
        {
            // TODO: Dodać zabezpieczenia dla pozostałych wartosci
            if (listOfValues[0].Length < 11 || listOfValues[0].Length > 11)
                throw new FormatException("PESEL musi mieć 11 cyfr");
            PeselNumber = listOfValues[0];
            Surname = listOfValues[1];
            Name = listOfValues[2];
            BirthDate = DateTime.Parse(listOfValues[3]);
            _PatientState = listOfValues[4].GetEnumFromDescription<PatientState>();
            _PatientSex = (Sex)Enum.Parse(typeof(Sex), listOfValues[5]);
            PrimaryKeyNameToSql = "PESEL";
        }
    }
}
