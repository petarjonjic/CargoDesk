using CargoDesk.Data;
using CargoDesk.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargoDesk.Repositories
{
    public static class PrimkaRepository
    {
        public static async Task<List<Primka>> GetAllAsync()
        {
            var lista = new List<Primka>();
            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
            select primka_id, broj_primke, datum,
                   broj_narudzbenice_dobavljaca, broj_racuna_dobavljaca, napomena,
                   dobavljac_id, skladiste_id, zaposlenik_id
            from primka
            order by datum desc, primka_id desc;", conn);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(Map(r));
            }

            return lista;
        }
        public static async Task<Primka?> GetByIdAsync(int primkaId)
        {
            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
            select primka_id, broj_primke, datum,
                   broj_narudzbenice_dobavljaca, broj_racuna_dobavljaca, napomena,
                   dobavljac_id, skladiste_id, zaposlenik_id
            from primka
            where primka_id = @id;", conn);

            cmd.Parameters.AddWithValue("@id", primkaId);

            await using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
                return Map(r);

            return null;
        }

        public static async Task<int> InsertPrimkaAsync(Primka p)
        {
            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
            insert into primka
                (broj_primke, datum, broj_narudzbenice_dobavljaca, broj_racuna_dobavljaca, napomena,
                 dobavljac_id, skladiste_id, zaposlenik_id)
            values
                (@broj, @datum, @bnd, @brd, @nap, @dob, @skl, @zap)
            returning primka_id;", conn);

            cmd.Parameters.AddWithValue("@broj", p.BrojPrimke);
            cmd.Parameters.AddWithValue("@datum", p.Datum);

            cmd.Parameters.AddWithValue("@bnd",
                string.IsNullOrWhiteSpace(p.BrojNarudzbeniceDobavljaca) ? (object)DBNull.Value : p.BrojNarudzbeniceDobavljaca);
            cmd.Parameters.AddWithValue("@brd",
                string.IsNullOrWhiteSpace(p.BrojRacunaDobavljaca) ? (object)DBNull.Value : p.BrojRacunaDobavljaca);
            cmd.Parameters.AddWithValue("@nap",
                string.IsNullOrWhiteSpace(p.Napomena) ? (object)DBNull.Value : p.Napomena);

            cmd.Parameters.AddWithValue("@dob", p.DobavljacId);
            cmd.Parameters.AddWithValue("@skl", p.SkladisteId);
            cmd.Parameters.AddWithValue("@zap", p.ZaposlenikId);

            var idObj = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(idObj);
        }

        public static async Task UpdateAsync(Primka p)
        {
            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
            update primka
            set broj_primke = @broj,
                datum = @datum,
                broj_narudzbenice_dobavljaca = @bnd,
                broj_racuna_dobavljaca = @brd,
                napomena = @nap,
                dobavljac_id = @dob,
                skladiste_id = @skl,
                zaposlenik_id = @zap
            where primka_id = @id;", conn);

            cmd.Parameters.AddWithValue("@id", p.PrimkaId);
            cmd.Parameters.AddWithValue("@broj", p.BrojPrimke);
            cmd.Parameters.AddWithValue("@datum", p.Datum);

            cmd.Parameters.AddWithValue("@bnd",
                string.IsNullOrWhiteSpace(p.BrojNarudzbeniceDobavljaca) ? (object)DBNull.Value : p.BrojNarudzbeniceDobavljaca);
            cmd.Parameters.AddWithValue("@brd",
                string.IsNullOrWhiteSpace(p.BrojRacunaDobavljaca) ? (object)DBNull.Value : p.BrojRacunaDobavljaca);
            cmd.Parameters.AddWithValue("@nap",
                string.IsNullOrWhiteSpace(p.Napomena) ? (object)DBNull.Value : p.Napomena);

            cmd.Parameters.AddWithValue("@dob", p.DobavljacId);
            cmd.Parameters.AddWithValue("@skl", p.SkladisteId);
            cmd.Parameters.AddWithValue("@zap", p.ZaposlenikId);

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task DeleteAsync(int primkaId)
        {
            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand("delete from primka where primka_id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", primkaId);
            await cmd.ExecuteNonQueryAsync();
        }
        private static Primka Map(NpgsqlDataReader r)
        {
            return new Primka
            {
                PrimkaId = r.GetInt32(0),
                BrojPrimke = r.GetString(1),
                Datum = r.GetDateTime(2),

                BrojNarudzbeniceDobavljaca = r.IsDBNull(3) ? null : r.GetString(3),
                BrojRacunaDobavljaca = r.IsDBNull(4) ? null : r.GetString(4),
                Napomena = r.IsDBNull(5) ? null : r.GetString(5),

                DobavljacId = r.GetInt32(6),
                SkladisteId = r.GetInt32(7),
                ZaposlenikId = r.GetInt32(8)
            };
        }

    }
}
