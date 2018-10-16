using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.Model
{
    class Doctor : Table
    {
        public override string PrimaryKeyNameToSql { get; protected set; }
        public int DoctorId { get; protected set; }
        public string Name { get; protected set; }
        public string Surname { get; protected set; }
        public AcademicDegrees AcademicDegree { get; protected set; }
        public string MedicalSpecialization { get; protected set; }
        public DateTime DateOfEmployment { get; protected set; }
        public string JobPosition { get; protected set; }

        public Doctor() : base()
        {
            PrimaryKeyNameToSql = "Id_Lekarza";
        }
        public Doctor(int doctorId, string name, string surname, AcademicDegrees academicDegree, string medicalSpecialization
            DateTime dateOfEmployment, string jobPosition) : base()
        {
            DoctorId = doctorId;
            Name = name;
            Surname = surname;
            AcademicDegree = academicDegree;
            MedicalSpecialization = medicalSpecialization;
            DateOfEmployment = dateOfEmployment;
            JobPosition = jobPosition;
            PrimaryKeyNameToSql = "Id_Lekarza";
        }
        /// <summary>
        /// List have to be in right order (pesel, surname, name, birth date, patient state, patient sex).
        /// </summary>
        /// <param name="listOfValues"></param>
        public Doctor(List<object> listOfValues) : base()
        {
            
            PrimaryKeyNameToSql = "Id_Lekarza";
        }
    }
}
