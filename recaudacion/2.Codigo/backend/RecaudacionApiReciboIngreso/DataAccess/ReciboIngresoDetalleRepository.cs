using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace RecaudacionApiReciboIngreso.DataAccess
{
    public class ReciboIngresoDetalleRepository : IReciboIngresoDetalleRepository
    {
        private readonly ReciboIngresoContext _context;
        private readonly string _connectionString;

        public ReciboIngresoDetalleRepository(ReciboIngresoContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<ReciboIngresoDetalle> Add(ReciboIngresoDetalle reciboIngresoDetalle)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_RECIBO_INGRESOS_DETALLE_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_ID", reciboIngresoDetalle.ReciboIngresoId));
                    cmd.Parameters.Add(new SqlParameter("@CLASIFICADOR_INGRESO_ID", reciboIngresoDetalle.ClasificadorIngresoId));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_DETALLE_IMPORTE", reciboIngresoDetalle.Importe));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_DETALLE_REFERENCIA", reciboIngresoDetalle.Referencia));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_DETALLE_ESTADO", reciboIngresoDetalle.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", reciboIngresoDetalle.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", reciboIngresoDetalle.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reciboIngresoDetalle.ReciboIngresoDetalleId = (int)reader["RECIBO_INGRESO_DETALLE_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return reciboIngresoDetalle;
        }

        public async Task Delete(ReciboIngresoDetalle reciboIngresoDetalle)
        {
            _context.ReciboIngresoDetalles.Remove(reciboIngresoDetalle);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ReciboIngresoDetalle>> FindAll(int id)
        {
            var reciboIngresoDetalles = await _context.ReciboIngresoDetalles.FromSqlRaw<ReciboIngresoDetalle>("USP_RECIBO_INGRESOS_DETALLE_SELALL {0}", id).ToListAsync();
            return reciboIngresoDetalles;
        }

        public async Task<ReciboIngresoDetalle> FindById(int id)
        {
            var reciboIngresoDetalle = (await _context.ReciboIngresoDetalles.FromSqlRaw<ReciboIngresoDetalle>("USP_RECIBO_INGRESOS_DETALLE_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return reciboIngresoDetalle;
        }

        public decimal SumImporte(List<ReciboIngresoDetalle> detalles)
        {
            return detalles.Sum(x => x.Importe);
        }
    }
}
