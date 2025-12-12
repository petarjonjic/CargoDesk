using CargoDesk.Data;
using CargoDesk.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CargoDesk.Repositories
{
    public static class OtpremnicaRepository
    {
        public static async Task<List<Otpremnica>> GetAllAsync()
        {
            var lista = new List<Otpremnica>();

            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
                select otpremnica_id, broj_otpremnice, datum,
                       broj_narudzbenice_kupca, broj_racuna_kupca, napomena,
                       kupac_id, skladiste_id, zaposlenik_id
                from otpremnica
                order by datum desc, otpremnica_id desc;", conn);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(new Otpremnica
                {
                    OtpremnicaId = r.GetInt32(0),
                    BrojOtpremnice = r.GetString(1),
                    Datum = r.GetDateTime(2),
                    BrojNarudzbeniceKupca = r.IsDBNull(3) ? null : r.GetString(3),
                    BrojRacunaKupca = r.IsDBNull(4) ? null : r.GetString(4),
                    Napomena = r.IsDBNull(5) ? null : r.GetString(5),
                    KupacId = r.GetInt32(6),
                    SkladisteId = r.GetInt32(7),
                    ZaposlenikId = r.GetInt32(8)
                });
            }

            return lista;
        }

        public static async Task<int> InsertAsync(Otpremnica o)
        {
            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
                insert into otpremnica
                    (broj_otpremnice, datum, broj_narudzbenice_kupca, broj_racuna_kupca, napomena,
                     kupac_id, skladiste_id, zaposlenik_id)
                values
                    (@broj, @datum, @bnk, @brk, @nap, @kupac, @skladiste, @zap)
                returning otpremnica_id;", conn);

            cmd.Parameters.AddWithValue("@broj", o.BrojOtpremnice);
            cmd.Parameters.AddWithValue("@datum", o.Datum);

            cmd.Parameters.AddWithValue("@bnk", (object?)o.BrojNarudzbeniceKupca ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@brk", (object?)o.BrojRacunaKupca ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@nap", (object?)o.Napomena ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@kupac", o.KupacId);
            cmd.Parameters.AddWithValue("@skladiste", o.SkladisteId);
            cmd.Parameters.AddWithValue("@zap", o.ZaposlenikId);

            var id = (int)(await cmd.ExecuteScalarAsync() ?? 0);
            return id;
        }

        public static async Task UpdateAsync(Otpremnica o)
        {
            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
                update otpremnica
                set broj_otpremnice = @broj,
                    datum = @datum,
                    broj_narudzbenice_kupca = @bnk,
                    broj_racuna_kupca = @brk,
                    napomena = @nap,
                    kupac_id = @kupac,
                    skladiste_id = @skladiste,
                    zaposlenik_id = @zap
                where otpremnica_id = @id;", conn);

            cmd.Parameters.AddWithValue("@id", o.OtpremnicaId);
            cmd.Parameters.AddWithValue("@broj", o.BrojOtpremnice);
            cmd.Parameters.AddWithValue("@datum", o.Datum);

            cmd.Parameters.AddWithValue("@bnk", (object?)o.BrojNarudzbeniceKupca ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@brk", (object?)o.BrojRacunaKupca ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@nap", (object?)o.Napomena ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@kupac", o.KupacId);
            cmd.Parameters.AddWithValue("@skladiste", o.SkladisteId);
            cmd.Parameters.AddWithValue("@zap", o.ZaposlenikId);

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task DeleteAsync(int otpremnicaId)
        {
            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
                delete from otpremnica
                where otpremnica_id = @id;", conn);

            cmd.Parameters.AddWithValue("@id", otpremnicaId);
            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task<List<Otpremnica>> GetByKupacAsync(int kupacId)
        {
            var lista = new List<Otpremnica>();

            await using var conn = await Database.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(@"
                select otpremnica_id, broj_otpremnice, datum,
                       broj_narudzbenice_kupca, broj_racuna_kupca, napomena,
                       kupac_id, skladiste_id, zaposlenik_id
                from otpremnica
                where kupac_id = @kupac
                order by datum desc, otpremnica_id desc;", conn);

            cmd.Parameters.AddWithValue("@kupac", kupacId);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(new Otpremnica
                {
                    OtpremnicaId = r.GetInt32(0),
                    BrojOtpremnice = r.GetString(1),
                    Datum = r.GetDateTime(2),
                    BrojNarudzbeniceKupca = r.IsDBNull(3) ? null : r.GetString(3),
                    BrojRacunaKupca = r.IsDBNull(4) ? null : r.GetString(4),
                    Napomena = r.IsDBNull(5) ? null : r.GetString(5),
                    KupacId = r.GetInt32(6),
                    SkladisteId = r.GetInt32(7),
                    ZaposlenikId = r.GetInt32(8)
                });
            }

            return lista;
        }
    }
}
