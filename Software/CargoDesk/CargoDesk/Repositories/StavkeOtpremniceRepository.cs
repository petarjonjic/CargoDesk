using CargoDesk.Data;
using CargoDesk.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CargoDesk.Repositories
{
    public static class StavkeOtpremniceRepository
    {
        public static async Task<List<StavkeOtpremnice>> GetByOtpremnicaIdAsync(int otpremnicaId)
        {
            var lista = new List<StavkeOtpremnice>();

            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
            select rb_stavke, otpremnica_id, kolicina, cijena, proizvod_id, lokacija_id
            from stavke_otpremnice
            where otpremnica_id = @id
            order by rb_stavke;", conn);

            cmd.Parameters.AddWithValue("@id", otpremnicaId);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(new StavkeOtpremnice
                {
                    RbStavke = r.GetInt32(0),
                    OtpremnicaId = r.GetInt32(1),
                    Kolicina = r.GetDecimal(2),
                    Cijena = r.GetDecimal(3),
                    ProizvodId = r.GetInt32(4),
                    LokacijaId = r.GetInt32(5)
                });
            }

            return lista;
        }

        public static async Task<int> GetNextRbStavkeAsync(int otpremnicaId)
        {
            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
            select coalesce(max(rb_stavke), 0) + 1
            from stavke_otpremnice
            where otpremnica_id = @id;", conn);

            cmd.Parameters.AddWithValue("@id", otpremnicaId);

            var obj = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(obj);
        }

        public static async Task InsertAsync(StavkeOtpremnice s)
        {
            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            if (s.RbStavke <= 0)
                s.RbStavke = await GetNextRbStavkeAsync(s.OtpremnicaId);

            await using var cmd = new NpgsqlCommand(@"
            insert into stavke_otpremnice
                (rb_stavke, otpremnica_id, kolicina, cijena, proizvod_id, lokacija_id)
            values
                (@rb, @oid, @kol, @cij, @pid, @lid);", conn);

            cmd.Parameters.AddWithValue("@rb", s.RbStavke);
            cmd.Parameters.AddWithValue("@oid", s.OtpremnicaId);
            cmd.Parameters.AddWithValue("@kol", s.Kolicina);
            cmd.Parameters.AddWithValue("@cij", s.Cijena);
            cmd.Parameters.AddWithValue("@pid", s.ProizvodId);
            cmd.Parameters.AddWithValue("@lid", s.LokacijaId);

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpdateAsync(StavkeOtpremnice s)
        {
            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
            update stavke_otpremnice
            set kolicina = @kol,
                cijena = @cij,
                proizvod_id = @pid,
                lokacija_id = @lid
            where otpremnica_id = @oid
              and rb_stavke = @rb;", conn);

            cmd.Parameters.AddWithValue("@kol", s.Kolicina);
            cmd.Parameters.AddWithValue("@cij", s.Cijena);
            cmd.Parameters.AddWithValue("@pid", s.ProizvodId);
            cmd.Parameters.AddWithValue("@lid", s.LokacijaId);
            cmd.Parameters.AddWithValue("@oid", s.OtpremnicaId);
            cmd.Parameters.AddWithValue("@rb", s.RbStavke);

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task DeleteAsync(int otpremnicaId, int rbStavke)
        {
            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
            delete from stavke_otpremnice
            where otpremnica_id = @oid
              and rb_stavke = @rb;", conn);

            cmd.Parameters.AddWithValue("@oid", otpremnicaId);
            cmd.Parameters.AddWithValue("@rb", rbStavke);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
