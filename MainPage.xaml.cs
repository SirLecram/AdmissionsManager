using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AdmissionsManager.View;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace AdmissionsManager
{
    /// <summary>
    /// Zrobione ostatnio:
    /// Puste klasy modelu; wstepnie model pacjenta; enum typ tabeli; strony; dzialanie commandbar; przelaczanie miedzy frame;
    /// ConnectionString i laczenie z baza; Konstruktory modelu Pacjent; Wyświetlanie calej tabeli przy przelaczeniu karty
    /// (Na razie tylk opacjent); Dodanie do pacjent page ListView i bindowanie danych; Interfejs IDatabaseConnectable dla Page!;
    /// 
    /// Do zrobienia: Pozostałe klasy page; przemyslenie w ktorym miejscu laczyc z baza; Postarac sie nie uzywac Modelu w View;
    /// Dodanie w kontrolerze na stałe parametrów zapytania (Lub np. wlasciwosc z obecnym page (IDatabaseConnectable)), ma to pomoc
    /// w budowaniu zapytan; Moze dodanie klas odpowiedzialnych za czesc zapytania z wyszukiwaniem i filtrami ?
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Controller controler { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            controler = new Controller(mainFrame);
            
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Tabels frameNumber = (Tabels) int.Parse((sender as AppBarButton).Tag.ToString());
            controler.ChangeFrame(frameNumber);
        }
    }
}
