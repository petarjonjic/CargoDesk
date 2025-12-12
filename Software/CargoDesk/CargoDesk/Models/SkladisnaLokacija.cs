using System;
using System.Collections.Generic;
using System.Text;

namespace CargoDesk.Models
{
    public class SkladisnaLokacija
    {
        public int LokacijaId { get; set; }

        public string OznakaLokacije { get; set; }
        public string Opis { get; set; }

        // FK
        public int SkladisteId { get; set; }

        public string NazivSkladista { get; set; }

        public override string ToString()
        {
            return OznakaLokacije;
        }
    }
}
