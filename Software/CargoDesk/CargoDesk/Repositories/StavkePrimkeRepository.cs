using CargoDesk.Data;
using CargoDesk.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CargoDesk.Repositories
{
    public static class StavkePrimkeRepository
    {
        public static async Task<List<StavkePrimke>> GetByPrimkaIdAsync(int primkaId)
        {
            var lista = new List<StavkePrimke>();

            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
            select rb_stavke, primka_id, kolicina, cijena, proizvod_id, lokacija_id
            from stavke_primke
            where primka_id = @id
            order by rb_stavke;", conn);

            cmd.Parameters.AddWithValue("@id", primkaId);

            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                lista.Add(new StavkePrimke
                {
                    RbStavke = r.GetInt32(0),
                    PrimkaId = r.GetInt32(1),
                    Kolicina = r.GetDecimal(2),
                    Cijena = r.GetDecimal(3),
                    ProizvodId = r.GetInt32(4),
                    LokacijaId = r.GetInt32(5)
                });
            }

            return lista;
        }

        public static async Task<int> GetNextRbStavkeAsync(int primkaId)
        {
            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
            select coalesce(max(rb_stavke), 0) + 1
            from stavke_primke
            where primka_id = @id;", conn);

            cmd.Parameters.AddWithValue("@id", primkaId);

            var obj = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(obj);
        }

        public static async Task InsertAsync(StavkePrimke s)
        {
            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            if (s.RbStavke <= 0)
                s.RbStavke = await GetNextRbStavkeAsync(s.PrimkaId);

            await using var cmd = new NpgsqlCommand(@"
            insert into stavke_primke
                (rb_stavke, primka_id, kolicina, cijena, proizvod_id, lokacija_id)
            values
                (@rb, @pid, @kol, @cij, @prid, @lid);", conn);

            cmd.Parameters.AddWithValue("@rb", s.RbStavke);
            cmd.Parameters.AddWithValue("@pid", s.PrimkaId);
            cmd.Parameters.AddWithValue("@kol", s.Kolicina);
            cmd.Parameters.AddWithValue("@cij", s.Cijena);
            cmd.Parameters.AddWithValue("@prid", s.ProizvodId);
            cmd.Parameters.AddWithValue("@lid", s.LokacijaId);

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpdateAsync(StavkePrimke s)
        {
            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
            update stavke_primke
            set kolicina = @kol,
                cijena = @cij,
                proizvod_id = @prid,
                lokacija_id = @lid
            where primka_id = @pid
              and rb_stavke = @rb;", conn);

            cmd.Parameters.AddWithValue("@kol", s.Kolicina);
            cmd.Parameters.AddWithValue("@cij", s.Cijena);
            cmd.Parameters.AddWithValue("@prid", s.ProizvodId);
            cmd.Parameters.AddWithValue("@lid", s.LokacijaId);
            cmd.Parameters.AddWithValue("@pid", s.PrimkaId);
            cmd.Parameters.AddWithValue("@rb", s.RbStavke);

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task DeleteAsync(int primkaId, int rbStavke)
        {
            await using var conn = Database.GetConnection();
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(@"
            delete from stavke_primke
            where primka_id = @pid
              and rb_stavke = @rb;", conn);

            cmd.Parameters.AddWithValue("@pid", primkaId);
            cmd.Parameters.AddWithValue("@rb", rbStavke);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
