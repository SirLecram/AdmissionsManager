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
        public override string GetPrimaryKey => DoctorId.ToString();
        public int DoctorId { get; protected set; }
        public string Name { get; protected set; }
        public string Surname { get; protected set; }
        private AcademicDegrees _AcademicDegree { get;set; }
        public string AcademicDegree { get => _AcademicDegree.GetEnumDescription(); }
        private MedicalSpecializations _MedicalSpecialization { get;set; }
        public string MedicalSpecialization { get => _MedicalSpecialization.GetEnumDescription(); }
        public DateTime DateOfEmployment { get; protected set; }
        private JobPositions _JobPosition { get; set; }
        public string JobPosition { get => _JobPosition.GetEnumDescription(); }

        

        public Doctor() : base()
        {
            PrimaryKeyNameToSql = "Id_Lekarza";
        }
        public Doctor(int doctorId, string name, string surname, AcademicDegrees academicDegree, 
            MedicalSpecializations medicalSpecialization, DateTime dateOfEmployment, JobPositions jobPosition) : base()
        {
            DoctorId = doctorId;
            Name = name;
            Surname = surname;
            _AcademicDegree = academicDegree;
            _MedicalSpecialization = medicalSpecialization;
            DateOfEmployment = dateOfEmployment;
            _JobPosition = jobPosition;
            PrimaryKeyNameToSql = "Id_Lekarza";
        }
        /// <summary>
        /// List have to be in right order (doctor id, name, surname, academic degree, medical specialization,
        /// date of employment, jobposition).
        /// </summary>
        /// <param name="listOfValues"></param>
        public Doctor(List<string> listOfValues) : base()
        {
            DoctorId = int.Parse(listOfValues[0]);
            Name = listOfValues[1];
            Surname = listOfValues[2];
            _AcademicDegree = listOfValues[3].GetEnumFromDescription<AcademicDegrees>();
            _MedicalSpecialization = listOfValues[4].GetEnumFromDescription<MedicalSpecializations>();
            DateOfEmployment = DateTime.Parse(listOfValues[5]);
            _JobPosition = listOfValues[6].GetEnumFromDescription<JobPositions>();
            PrimaryKeyNameToSql = "Id_Lekarza";
        }
    }
}
