using System;
using System.Collections.Generic;
using System.Text;

namespace CargoDesk.Models
{
    public class Otpremnica
    {
        public int OtpremnicaId { get; set; }

        public string BrojOtpremnice { get; set; }
        public DateTime Datum { get; set; }

        public string BrojNarudzbeniceKupca { get; set; }
        public string BrojRacunaKupca { get; set; }
        public string Napomena { get; set; }

        // FK
        public int KupacId { get; set; }
        public int SkladisteId { get; set; }
        public int ZaposlenikId { get; set; }

        public string KupacNaziv { get; set; }
        public string SkladisteNaziv { get; set; }
        public string ZaposlenikImePrezime { get; set; }

        public override string ToString()
        {
            return $"{BrojOtpremnice} ({Datum:dd.MM.yyyy})";
        }
    }
}
