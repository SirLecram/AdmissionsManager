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
using System.Threading.Tasks;
using AdmissionsManager.Controlers;
//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace AdmissionsManager
{
    /// <summary>
    /// Zrobione ostatnio: 
    /// Nowe Interfejsy, Table -> SqlTable : ISqlTableModelable, IPrimaryKeyGetable; ConnectionString -> App; INavigator,
    /// IPageNavigateable; Kontroler ViewNavigator : INavigator - przeniesiono nawigację z kontrolera;
    /// DataValidator + ValidateIfTypeImplementInterface; NavigationPageTypeProvider : TypeProvider i IProvideType;
    /// Segregacja plikow wzgledem warstw;
    /// SingleResponsibility:
    /// Liskov Substitution:
    /// Interface Segregation:
    /// Open/Closed:
    
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
    /// Rozbudowa EditDialog - dodatkowe ograniczenia, informacje o typie, formacie, dodano dodatkowe informacje;
    /// Controller - nowa funkcja zwracajaca typy kolumn; Poprawa fukncjonowania edytowania pól; 
    /// Obsluga SqlException w metodzie EditRecordAsync - informacja w razie błędnego formatu;
    /// Dodano ContentDialog sluzacy do dodawania nowych rekordów; Dodano automatyczne generowanie wygladu ContentDialog do dodawania;
    /// Dodano opisy do wprowadzanych wartosci przy tworzeniu nowego rekordu;
    /// Do NewDialog zostało dodane sprawdzanie czy wzystkie pola są wypełnione; Anulowanie zamykania w razie pozostawienia pustych pól;
    /// Dodano textblock z alertem; Dodanie mechanizmu działania dialoga; Additional security and data validation; 
    /// Dodawanie nowego pacjenta w pełni działa; Enum AcademicDegrees, ; Wstepnie model Doctor; Rozszerzenie string GetEnumFromDescription;
    /// Rozbudowa modelu Doctor; Kolejne enumy: EnumTypes, JobPositions, MedicalSpecializations;
    /// Do dialogu umożliwiającego tworzenie nowych rekordów dodano  wstępne renderowanie ComboBox zamiast 
    /// TextBox w przypadku wartości reprezentowanych w kodzie przez enum oraz potrzebne właściwości;
    /// Dodano prywatne wlasciwosci do modelu przy wlasciwosciach Enum (Do bindowania);
    /// Wstępnie działa widok i model Doctor; Poprawiono błędy związane z przełączaniem się między kartami; 
    /// Widok i model lekarze działa sprawnie ( do testów); Przerobione niektóre metody aby były bardziej uniwersalne 
    /// i zeby można je było stosowac to wszystkich widoków i modeli (np. GetPrimaryKeyAndPrimaryKeyName);
    /// Przeniesienie EnumTypes do controller z NewDialog aby uniknąć redundancji; 
    /// Przystosowanie NewDialog i EditDialog do korzystania z przeniesionych metod w celu tworzenia interfejsu;
    /// Do EditDialog również został dodany combobox w wypadku wartości przyjmujących ENUM;
    /// Wyeliminowany blad w momencie zmiany strony; Zmiana sposobu nawigowania po aplikacji; 
    /// Przeniesienie inicjowania strony do eventu Loaded; Zmieniono metode ConnectToDatabase() z IDatabaseConnectable na Task<bool>;
    /// Event Page_Loaded i metoda czyszcząca dane z poprzedniego page przed Navigate zostala dodana (do IDatabaseConnectable);
    /// Regions i poprawiona czytelnosc kodu (DoctorsView i PatientsView);
    /// 
    /// Do zrobienia: Pozostałe klasy page; przemyslenie w ktorym miejscu laczyc z baza; Postarac sie nie uzywac Modelu w View;
    /// Dodanie w kontrolerze na stałe parametrów zapytania (Lub np. wlasciwosc z obecnym page (IDatabaseConnectable)), ma to pomoc
    /// w budowaniu zapytan; Moze dodanie klas odpowiedzialnych za czesc zapytania z wyszukiwaniem i filtrami ?;
    /// Dodać i zaimplementować interfejs do modeli zamiast Table (DependencyInjection) korzysci: GetPrimaryKeyAndPrimaryKeyName znikne,
    /// bedzie mozna sie dostac do tych wartosci bezposredno. Odczytywanie obiektów z bazy bedze latwiejsze bo zrobimy funkcje zwracajaca obiekt;
    /// DUŻE ZMIANY - WDRAŻANIE SOLID I DEPENDENCY INJECTION!!! REFRAKTORYZACJA (IDbControllAllowable)
    /// 1
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Controler controler { get; set; }
        private INavigator Navigator { get; set; }
        private IProvideType TypeProvider { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            controler = new Controler(mainFrame);
            navigationBar.DataContext = controler;
            
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            string pageTypeName = (sender as AppBarButton).Tag.ToString();
            Type pageType = TypeProvider.GetTypeFromString(pageTypeName);
            Navigator.ChangeFrame(pageType, mainFrame); 
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeProperties();
        }
        private void InitializeProperties()
        {
            IValidateIfInterfaceIsImplemented validator = new Validator();
            Navigator = new ViewNavigator(validator, mainFrame.Content as IPageNavigateable);
            Navigator.SetParameter(controler);
            TypeProvider = new DataProviders.NavigationPageTypeProvider(validator,
                new List<Type>
                {
                    typeof(PatientsPage), typeof(AdmissionsPage), typeof(DoctorsPage),
                });
        }
    }
    
}
