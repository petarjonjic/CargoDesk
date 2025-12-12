using System;
using System.Collections.Generic;
using System.Text;

namespace CargoDesk.Models
{
    public class StanjeZalihaRow
    {
        public int ProizvodId { get; set; }
        public string NazivProizvoda { get; set; }

        public int SkladisteId { get; set; }
        public string NazivSkladista { get; set; }

        public int LokacijaId { get; set; }
        public string OznakaLokacije { get; set; }

        public decimal Kolicina { get; set; }
    }
}
