using System;
using System.Collections.Generic;
using System.Text;

namespace CargoDesk.Models
{
    public class Proizvod
    {
        public int ProizvodId { get; set; }

        public string NazivProizvoda { get; set; }
        public string Sifra { get; set; }

        public bool StatusProizvoda { get; set; }

        public string Napomena { get; set; }

        public decimal NabavnaCijena { get; set; }
        public decimal ProdajnaCijena { get; set; }

        // FK
        public int KategorijaId { get; set; }
        public int JmId { get; set; }

        public string NazivKategorije { get; set; }
        public string OznakaJediniceMjere { get; set; }

        public override string ToString()
        {
            return $"{NazivProizvoda} ({Sifra})";
        }
    }
}
