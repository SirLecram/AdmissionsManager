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
    /// Rozbudowa EditDialog - dodatkowe ograniczenia, informacje o typie, formacie, dodano dodatkowe informacje;
    /// Controller - nowa funkcja zwracajaca typy kolumn; Poprawa fukncjonowania edytowania pól; 
    /// Obsluga SqlException w metodzie EditRecordAsync - informacja w razie błędnego formatu;
    /// Dodano ContentDialog sluzacy do dodawania nowych rekordów; Dodano automatyczne generowanie wygladu ContentDialog do dodawania;
    /// Dodano opisy do wprowadzanych wartosci przy tworzeniu nowego rekordu;
    /// 
    /// Zrobione: 
    /// Puste klasy modelu; wstepnie model pacjenta; enum typ tabeli; strony; dzialanie commandbar; przelaczanie miedzy frame;
    /// ConnectionString i laczenie z baza; Konstruktory modelu Pacjent; Wyświetlanie calej tabeli przy przelaczeniu karty
    /// (Na razie tylk opacjent); Dodanie do pacjent page ListView i bindowanie danych; Interfejs IDatabaseConnectable dla Page!;
    /// GetColumnNamesFromTable() w controller - ładowanie wartosci do combobox; Sortowanie; SqlCommandFilterCreator; 
    /// Wyszukiwanie w danej kolumnie po wyrażenaiach; Rozszerzenie enum GetDescription dla łatwiejszego dostępu do 
    /// nazwy tabeli dla komendy SQL; Resetowanie wyszukiwania; Usuwanie rekordów;
    /// Sprawdzanie czy rekord znajduje sie tabeli Przyjecia i usuwanie go jeesli uzytkownik tego sobie zyczy;
    /// Informacja jesli nie udalo sie polaczyc z baza; Region w kontrolerze; Dodano EditDialog; Dodano wstępne edytowanie rekordów;
    /// Dodano nowy przycisk do edycji rekordu; Obsluga wyjatku w raize braku polaczenia z DB;
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
