using CargoDesk.Data;
using CargoDesk.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CargoDesk.Repositories
{
    public static class StanjeZalihaRepository
    {
        public static async Task<List<StanjeZalihaRow>> GetAllRowsAsync()
        {
            var lista = new List<StanjeZalihaRow>();
            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
            select
                sz.stanje_id,
                sz.skladiste_id,
                s.naziv_skladista,
                sz.lokacija_id,
                sl.oznaka_lokacije,
                sz.proizvod_id,
                p.naziv_proizvoda,
                sz.kolicina
            from stanje_zaliha sz
            join skladiste s on s.skladiste_id = sz.skladiste_id
            join skladisna_lokacija sl on sl.lokacija_id = sz.lokacija_id
            join proizvod p on p.proizvod_id = sz.proizvod_id
            order by s.naziv_skladista, sl.oznaka_lokacije, p.naziv_proizvoda;", conn);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(new StanjeZalihaRow
                {
                    StanjeId = r.GetInt32(0),
                    SkladisteId = r.GetInt32(1),
                    NazivSkladista = r.GetString(2),
                    LokacijaId = r.GetInt32(3),
                    OznakaLokacije = r.GetString(4),
                    ProizvodId = r.GetInt32(5),
                    NazivProizvoda = r.GetString(6),
                    Kolicina = r.GetDecimal(7)
                });
            }

            return lista;
        }

        public static async Task<decimal?> GetKolicinaAsync(int skladisteId, int lokacijaId, int proizvodId)
        {
            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
            select kolicina
            from stanje_zaliha
            where skladiste_id = @s and lokacija_id = @l and proizvod_id = @p;", conn);

            cmd.Parameters.AddWithValue("@s", skladisteId);
            cmd.Parameters.AddWithValue("@l", lokacijaId);
            cmd.Parameters.AddWithValue("@p", proizvodId);

            var obj = await cmd.ExecuteScalarAsync();
            if (obj == null || obj == DBNull.Value) return null;

            return Convert.ToDecimal(obj);
        }

        public static async Task SetKolicinaAsync(int stanjeId, decimal novaKolicina)
        {
            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
            update stanje_zaliha
            set kolicina = @k
            where stanje_id = @id;", conn);

            cmd.Parameters.AddWithValue("@k", novaKolicina);
            cmd.Parameters.AddWithValue("@id", stanjeId);

            await cmd.ExecuteNonQueryAsync();
        }

    }
}
