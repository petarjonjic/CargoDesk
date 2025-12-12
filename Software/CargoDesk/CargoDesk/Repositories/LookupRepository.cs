using CargoDesk.Data;
using CargoDesk.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CargoDesk.Repositories
{
    public static class LookupRepository
    {
        public static async Task<List<LookupItem>> GetSkladistaAsync()
        {
            var lista = new List<LookupItem>();

            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(
                "select skladiste_id, naziv_skladista from skladiste order by naziv_skladista;", conn);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(new LookupItem
                {
                    Id = r.GetInt32(0),
                    Naziv = r.GetString(1)
                });
            }

            return lista;
        }

        public static async Task<List<LookupItem>> GetLokacijeZaSkladisteAsync(int skladisteId)
        {
            var lista = new List<LookupItem>();

            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(
                "select lokacija_id, oznaka_lokacije from skladisna_lokacija where skladiste_id = @sid order by oznaka_lokacije;", conn);
            cmd.Parameters.AddWithValue("@sid", skladisteId);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(new LookupItem
                {
                    Id = r.GetInt32(0),
                    Naziv = r.GetString(1)
                });
            }

            return lista;
        }

        public static async Task<List<LookupItem>> GetProizvodiAsync(bool samoAktivni = true)
        {
            var lista = new List<LookupItem>();

            await using var conn = await Database.OpenConnectionAsync();
            var sql = "select proizvod_id, naziv_proizvoda from proizvod";
            if (samoAktivni)
                sql += " where status_proizvoda = true";
            sql += " order by naziv_proizvoda;";

            await using var cmd = new NpgsqlCommand(sql, conn);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(new LookupItem
                {
                    Id = r.GetInt32(0),
                    Naziv = r.GetString(1)
                });
            }

            return lista;
        }

        public static async Task<List<LookupItem>> GetDobavljaciAsync()
        {
            var lista = new List<LookupItem>();

            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(
                "select dobavljac_id, naziv_dobavljaca from dobavljac order by naziv_dobavljaca;", conn);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(new LookupItem
                {
                    Id = r.GetInt32(0),
                    Naziv = r.GetString(1)
                });
            }

            return lista;
        }

        public static async Task<List<LookupItem>> GetKupciAsync()
        {
            var lista = new List<LookupItem>();

            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(
                "select kupac_id, naziv_kupca from kupac order by naziv_kupca;", conn);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(new LookupItem
                {
                    Id = r.GetInt32(0),
                    Naziv = r.GetString(1)
                });
            }

            return lista;
        }

        public static async Task<List<LookupItem>> GetZaposleniciAsync()
        {
            var lista = new List<LookupItem>();

            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(
                "select zaposlenik_id, ime_prezime from zaposlenik order by ime_prezime;", conn);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(new LookupItem
                {
                    Id = r.GetInt32(0),
                    Naziv = r.GetString(1)
                });
            }

            return lista;
        }

    }
}
