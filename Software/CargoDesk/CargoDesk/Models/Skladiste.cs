using System;
using System.Collections.Generic;
using System.Text;

namespace CargoDesk.Models
{
    public class Skladiste
    {
        public int SkladisteId { get; set; }

        public string NazivSkladista { get; set; }
        public string Adresa { get; set; }
        public string Opis { get; set; }

        public override string ToString()
        {
            return NazivSkladista;
        }
    }
}
