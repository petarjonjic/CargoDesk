using System;
using System.Collections.Generic;
using System.Text;

namespace CargoDesk.Models
{
    public class StavkePrimke
    {
        public int RbStavke { get; set; }
        // PK + FK (dio složenog ključa)
        public int PrimkaId { get; set; }
        // FK
        public int ProizvodId { get; set; }
        public int LokacijaId { get; set; }
        public decimal Kolicina { get; set; }
        public decimal Cijena { get; set; }
        public string NazivProizvoda { get; set; }
        public string OznakaLokacije { get; set; }
    }
}
