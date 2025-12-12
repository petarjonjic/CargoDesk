using System;
using System.Collections.Generic;
using System.Text;

namespace CargoDesk.Models
{
    public class Dobavljac
    {
        public int DobavljacId { get; set; }
        public string NazivDobavljaca { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }

        public override string ToString()
        {
            return NazivDobavljaca;
        }
    }
}
