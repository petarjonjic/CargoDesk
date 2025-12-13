using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CargoDesk.Models;
using CargoDesk.Repositories;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CargoDesk.Views
{
    public sealed partial class OtpremnicaPage : Page
    {
        private int? _otpremnicaId;

        public OtpremnicaPage()
        {
            InitializeComponent();
            UcitajLookupe();
            DpDatum.Date = DateTimeOffset.Now;

            // Lokacije ovise o skladištu
            CbSkladiste.SelectionChanged += CbSkladiste_SelectionChanged;
        }

        // =========================
        // UČITAVANJE LOOKUPOVA
        // =========================
        private async void UcitajLookupe()
        {
            CbKupac.ItemsSource = await LookupRepository.GetKupciAsync();
            CbSkladiste.ItemsSource = await LookupRepository.GetSkladistaAsync();
            CbZaposlenik.ItemsSource = await LookupRepository.GetZaposleniciAsync();
            CbProizvod.ItemsSource = await LookupRepository.GetProizvodiAsync();

            CbLokacija.ItemsSource = null;
        }

        private async void CbSkladiste_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbSkladiste.SelectedItem == null)
                return;

            int skladisteId = ((LookupItem)CbSkladiste.SelectedItem).Id;

            CbLokacija.ItemsSource =
                await LookupRepository.GetLokacijeBySkladisteAsync(skladisteId);
        }

        private async void BtnSpremiOtpremnicu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtBrojOtpremnice.Text)
                || CbKupac.SelectedItem == null
                || CbSkladiste.SelectedItem == null
                || CbZaposlenik.SelectedItem == null)
            {
                TxtStatusOtpremnice.Text = "Popuni obavezna polja.";
                return;
            }

            var o = new Otpremnica
            {
                BrojOtpremnice = TxtBrojOtpremnice.Text.Trim(),
                Datum = DpDatum.Date.DateTime,
                KupacId = ((LookupItem)CbKupac.SelectedItem).Id,
                SkladisteId = ((LookupItem)CbSkladiste.SelectedItem).Id,
                ZaposlenikId = ((LookupItem)CbZaposlenik.SelectedItem).Id,
                BrojNarudzbeniceKupca = TxtBrojNarudzbenice.Text,
                BrojRacunaKupca = TxtBrojRacuna.Text,
                Napomena = TxtNapomena.Text
            };

            _otpremnicaId = await OtpremnicaRepository.InsertOtpremnicaAsync(o);

            TxtStatusOtpremnice.Text =
                $"Otpremnica spremljena. ID = {_otpremnicaId}. Sada dodaj stavke.";
        }

        private async void BtnDodajStavku_Click(object sender, RoutedEventArgs e)
        {
            if (_otpremnicaId == null)
            {
                TxtStatusStavke.Text = "Prvo spremi otpremnicu.";
                return;
            }

            if (CbProizvod.SelectedItem == null || CbLokacija.SelectedItem == null)
            {
                TxtStatusStavke.Text = "Odaberi proizvod i lokaciju.";
                return;
            }

            if (NbKolicina.Value <= 0)
            {
                TxtStatusStavke.Text = "Količina mora biti veća od 0.";
                return;
            }

            var stavka = new StavkeOtpremnice
            {
                OtpremnicaId = _otpremnicaId.Value,
                ProizvodId = ((LookupItem)CbProizvod.SelectedItem).Id,
                LokacijaId = ((LookupItem)CbLokacija.SelectedItem).Id,
                Kolicina = (decimal)NbKolicina.Value,
                Cijena = (decimal)NbCijena.Value
            };

            try
            {
                await StavkeOtpremniceRepository.InsertAsync(stavka);
                TxtStatusStavke.Text =
                    "Stavka dodana. Okidač je smanjio stanje_zaliha.";
            }
            catch (Exception ex)
            {
             
                TxtStatusStavke.Text = ex.Message;
            }
        }
    }
}