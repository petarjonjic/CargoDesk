using System;
using System.Collections.Generic;
using System.Text;

namespace CargoDesk.Models
{
    public class Primka
    {
        public int PrimkaId { get; set; }

        public string BrojPrimke { get; set; }
        public DateTime Datum { get; set; }

        public string BrojNarudzbeniceDobavljaca { get; set; }
        public string BrojRacunaDobavljaca { get; set; }
        public string Napomena { get; set; }

        // FK
        public int DobavljacId { get; set; }
        public int SkladisteId { get; set; }
        public int ZaposlenikId { get; set; }

        public string DobavljacNaziv { get; set; }
        public string SkladisteNaziv { get; set; }
        public string ZaposlenikImePrezime { get; set; }

        public override string ToString()
        {
            return $"{BrojPrimke} ({Datum:dd.MM.yyyy})";
        }
    }
}
