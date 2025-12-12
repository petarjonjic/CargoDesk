using System;
using System.Collections.Generic;
using System.Text;

namespace CargoDesk.Models
{
    public class Kupac
    {
        public int KupacId { get; set; }
        public string NazivKupca { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }

        public override string ToString()
        {
            return NazivKupca;
        }
    }
}
