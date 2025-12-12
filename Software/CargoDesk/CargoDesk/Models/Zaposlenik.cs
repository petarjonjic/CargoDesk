using System;
using System.Collections.Generic;
using System.Text;

namespace CargoDesk.Models
{
    public class Zaposlenik
    {
        public int ZaposlenikId { get; set; }

        public string Ime { get; set; }
        public string Prezime { get; set; }

        public string Email { get; set; }
        public string Telefon { get; set; }

        public string Uloga { get; set; }

        public string ImePrezime => $"{Ime} {Prezime}";

        public override string ToString()
        {
            return ImePrezime;
        }
    }
}
