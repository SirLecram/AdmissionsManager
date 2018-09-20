using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using AdmissionsManager.View;

namespace AdmissionsManager
{
    class Controller
    {
        private readonly Frame MainFrame;

        public Controller(Frame mainFrame)
        {
            MainFrame = mainFrame;
            MainFrame.Content = new AdmissionsPage();
        }

        public void ChangeFrame(Tabels changeTo)
        {
            
            switch (changeTo)
            {
                case Tabels.Admissions:
                    MainFrame.Content = new AdmissionsPage();
                    break;
                case Tabels.Patients:
                    MainFrame.Content = new PatientsPage();
                    break;
                case Tabels.Doctors:
                    MainFrame.Content = new DoctorsPage();
                    break;
                case Tabels.Diagnoses:
                    MainFrame.Content = new DiagnosisPage();
                
                    break;
                case Tabels.Surgeries:
                    MainFrame.Content = new SurgeriesPage();
                    break;
                case Tabels.Rooms:
                    MainFrame.Content = new RoomsPage();
                    break;
            }
        }
    }
}
