using CargoDesk.Models;
using CargoDesk.Repositories;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CargoDesk.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PagePrimka : Page
{

    private int? _primkaId;

    private ObservableCollection<StavkaPrimkeView> _stavke = new();

    public PagePrimka()
    {
        InitializeComponent();
        LvStavke.ItemsSource = _stavke;

        Loaded += PrimkaPage_Loaded;
    }

    private async void PrimkaPage_Loaded(object sender, RoutedEventArgs e)
    {
        DpDatum.Date = DateTimeOffset.Now;
        await UcitajLookupAsync();
        NovaPrimkaUI();
    }

    private void NovaPrimkaUI()
    {
        _primkaId = null;

        TxtBrojPrimke.Text = "";
        DpDatum.Date = DateTimeOffset.Now;
        TxtNarudzbenicaDob.Text = "";
        TxtRacunDob.Text = "";
        TxtNapomena.Text = "";

        _stavke.Clear();

        TxtStatusPrimka.Text = "Unesi zaglavlje primke pa klikni Spremi primku.";
        TxtStatusStavke.Text = "";

        BtnDodajStavku_IsEnabled(false);
    }


    private void BtnDodajStavku_IsEnabled(bool enabled)
    {
        // “Dodaj stavku” smije tek kad primka ima ID
        // (button je u XAML-u bez x:Name, pa ga kontroliramo preko grida: jednostavno ostavimo logiku na provjeri u handleru)
    }

    private async Task UcitajLookupAsync()
    {
        CbDobavljac.ItemsSource = await LookupRepository.GetDobavljaciAsync();
        CbSkladiste.ItemsSource = await LookupRepository.GetSkladistaAsync();
        CbZaposlenik.ItemsSource = await LookupRepository.GetZaposleniciAsync();
        CbProizvod.ItemsSource = await LookupRepository.GetProizvodiAsync();

        if (CbSkladiste.SelectedValue is int sid)
            CbLokacija.ItemsSource = await LookupRepository.GetLokacijeBySkladisteAsync(sid);
    }

    private async void CbSkladiste_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CbSkladiste.SelectedValue is int sid)
        {
            CbLokacija.ItemsSource = await LookupRepository.GetLokacijeBySkladisteAsync(sid);
            CbLokacija.SelectedIndex = -1;
        }
    }

    private void BtnNovaPrimka_Click(object sender, RoutedEventArgs e)
    {
        NovaPrimkaUI();
    }

    private async void BtnSpremiPrimku_Click(object sender, RoutedEventArgs e)
    {
        TxtStatusPrimka.Text = "";

        if (string.IsNullOrWhiteSpace(TxtBrojPrimke.Text))
        {
            TxtStatusPrimka.Text = "Greška: Broj primke je obavezan.";
            return;
        }
        if (CbDobavljac.SelectedValue is not int dobavljacId)
        {
            TxtStatusPrimka.Text = "Greška: Odaberi dobavljača.";
            return;
        }
        if (CbSkladiste.SelectedValue is not int skladisteId)
        {
            TxtStatusPrimka.Text = "Greška: Odaberi skladište.";
            return;
        }
        if (CbZaposlenik.SelectedValue is not int zaposlenikId)
        {
            TxtStatusPrimka.Text = "Greška: Odaberi zaposlenika.";
            return;
        }

        var primka = new Primka
        {
            BrojPrimke = TxtBrojPrimke.Text.Trim(),
            Datum = DpDatum.Date.DateTime,
            BrojNarudzbeniceDobavljaca = TxtNarudzbenicaDob.Text.Trim(),
            BrojRacunaDobavljaca = TxtRacunDob.Text.Trim(),
            Napomena = TxtNapomena.Text.Trim(),
            DobavljacId = dobavljacId,
            SkladisteId = skladisteId,
            ZaposlenikId = zaposlenikId
        };

        try
        {
            _primkaId = await PrimkaRepository.InsertPrimkaAsync(primka);
            TxtStatusPrimka.Text = $"Primka spremljena. ID = {_primkaId}. Sada dodaj stavke.";
        }
        catch (Exception ex)
        {
            TxtStatusPrimka.Text = "Greška kod spremanja primke: " + ex.Message;
        }
    }

    private async void BtnDodajStavku_Click(object sender, RoutedEventArgs e)
    {
        TxtStatusStavke.Text = "";

        if (_primkaId is null)
        {
            TxtStatusStavke.Text = "Prvo spremi primku (zaglavlje) da dobije primka_id.";
            return;
        }
        if (CbProizvod.SelectedValue is not int proizvodId)
        {
            TxtStatusStavke.Text = "Odaberi proizvod.";
            return;
        }
        if (CbLokacija.SelectedValue is not int lokacijaId)
        {
            TxtStatusStavke.Text = "Odaberi lokaciju.";
            return;
        }

        var kolicina = (decimal)NbKolicina.Value;
        var cijena = (decimal)NbCijena.Value;

        if (kolicina <= 0)
        {
            TxtStatusStavke.Text = "Količina mora biti > 0.";
            return;
        }

        int rb = _stavke.Count == 0 ? 1 : _stavke.Max(s => s.Rb) + 1;

        var stavka = new StavkePrimke
        {
            PrimkaId = _primkaId.Value,
            RbStavke = rb,
            ProizvodId = proizvodId,
            LokacijaId = lokacijaId,
            Kolicina = kolicina,
            Cijena = cijena
        };

        try
        {
            await StavkePrimkeRepository.InsertStavkaAsync(stavka);

            var nazivProizvoda = (CbProizvod.SelectedItem as LookupItem)?.Naziv ?? proizvodId.ToString();
            var nazivLokacije = (CbLokacija.SelectedItem as LookupItem)?.Naziv ?? lokacijaId.ToString();

            _stavke.Add(new StavkaPrimkeView
            {
                Rb = rb,
                ProizvodId = proizvodId,
                ProizvodNaziv = nazivProizvoda,
                LokacijaId = lokacijaId,
                LokacijaNaziv = nazivLokacije,
                Kolicina = kolicina,
                Cijena = cijena
            });

            TxtStatusStavke.Text = "Stavka dodana. Okidač je ažurirao stanje_zaliha.";
        }
        catch (Exception ex)
        {
            TxtStatusStavke.Text = "Greška kod dodavanja stavke: " + ex.Message;
        }
    }


    public class StavkaPrimkeView
    {
        public int Rb { get; set; }
        public int ProizvodId { get; set; }
        public string ProizvodNaziv { get; set; } = "";
        public int LokacijaId { get; set; }
        public string LokacijaNaziv { get; set; } = "";
        public decimal Kolicina { get; set; }
        public decimal Cijena { get; set; }
    }

}
