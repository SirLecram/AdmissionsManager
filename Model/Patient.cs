using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.Model
{
    public class Patient
    {
        public string PeselNumber { get; protected set; }
        public string Surname { get; protected set; }
        public string Name { get; protected set; }
        public DateTime BirthDate { get; protected set; }
        public string PatientState { get; protected set; }
        public Sex PatientSex { get; protected set; }
        public Patient()
        {
            
        }
        public Patient(string pesel, string surname, string name, DateTime birthDate, string patientState,
            Sex patientSex)
        {
            if (pesel.Length < 11 || pesel.Length > 11)
                throw new FormatException("PESEL musi mieć 11 cyfr");
            PeselNumber = pesel;
            Surname = surname;
            Name = name;
            BirthDate = birthDate;
            PatientState = patientState;
            PatientSex = patientSex;
        }
        /// <summary>
        /// List have to be in right order (pesel, surname, name, birth date, patient state, patient sex).
        /// </summary>
        /// <param name="listOfValues"></param>
        public Patient(List<object> listOfValues)
        {
            if (listOfValues[0].ToString().Length < 11 || listOfValues[0].ToString().Length > 11)
                throw new FormatException("PESEL musi mieć 11 cyfr");
            PeselNumber = listOfValues[0].ToString();
            Surname = listOfValues[1].ToString();
            Name = listOfValues[2].ToString();
            BirthDate = DateTime.Parse(listOfValues[3].ToString());
            PatientState = listOfValues[4].ToString();
            PatientSex = (Sex)Enum.Parse(typeof(Sex), listOfValues[5].ToString());
        }
    }
}
