using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace RecaudacionApiRegistroLinea.DataAccess
{
    public class RegistroLineaDetalleRepository : IRegistroLineaDetalleRepository
    {

        private readonly RegistroLineaContext _context;
        private readonly string _connectionString;

        public RegistroLineaDetalleRepository(RegistroLineaContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<RegistroLineaDetalle> Add(RegistroLineaDetalle registroLineaDetalle)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_REGISTROS_LINEA_DETALLE_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_ID", registroLineaDetalle.RegistroLineaId));
                    cmd.Parameters.Add(new SqlParameter("@CLASIFICADOR_INGRESO_ID", registroLineaDetalle.ClasificadorIngresoId));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_DETALLE_IMPORTE", registroLineaDetalle.Importe));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_DETALLE_REFERENCIA", registroLineaDetalle.Referencia));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_DETALLE_ESTADO", registroLineaDetalle.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", registroLineaDetalle.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", registroLineaDetalle.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            registroLineaDetalle.RegistroLineaDetalleId = (int)reader["REGISTRO_LINEA_DETALLE_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return registroLineaDetalle;
        }

        public async Task Delete(RegistroLineaDetalle registroLineaDetalle)
        {
            _context.RegistroLineaDetalles.Remove(registroLineaDetalle);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RegistroLineaDetalle>> FindAll(int id)
        {
            var registroLineaDetalles = await _context.RegistroLineaDetalles.FromSqlRaw<RegistroLineaDetalle>("USP_REGISTROS_LINEA_DETALLE_SELALL {0}", id).ToListAsync();
            return registroLineaDetalles;
        }

        public async Task<RegistroLineaDetalle> FindById(int id)
        {
            var registroLineaDetalle = (await _context.RegistroLineaDetalles.FromSqlRaw<RegistroLineaDetalle>("USP_REGISTROS_LINEA_DETALLE_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return registroLineaDetalle;
        }

        public decimal SumImporte(List<RegistroLineaDetalle> detalles)
        {
            return detalles.Sum(x => x.Importe);
        }
    }
}
