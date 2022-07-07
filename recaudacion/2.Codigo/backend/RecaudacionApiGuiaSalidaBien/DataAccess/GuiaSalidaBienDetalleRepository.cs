using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecaudacionApiGuiaSalidaBien.Domain;

namespace RecaudacionApiGuiaSalidaBien.DataAccess
{
    public class GuiaSalidaBienDetalleRepository : IGuiaSalidaBienDetalleRepository
    {

        private readonly GuiaSalidaBienContext _context;
        private readonly string _connectionString;

        public GuiaSalidaBienDetalleRepository(GuiaSalidaBienContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<GuiaSalidaBienDetalle> Add(GuiaSalidaBienDetalle guiaSalidaBienDetalle)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_GUIA_SALIDA_BIENES_DETALLE_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_ID", guiaSalidaBienDetalle.GuiaSalidaBienId));
                    cmd.Parameters.Add(new SqlParameter("@CATALOGO_BIEN_ID", guiaSalidaBienDetalle.CatalogoBienId));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_ID", guiaSalidaBienDetalle.IngresoPecosaDetalleId));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_DETALLE_CANTIDAD", guiaSalidaBienDetalle.Cantidad));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_DETALLE_SERIE_FORMATO", guiaSalidaBienDetalle.SerieFormato));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_DETALLE_SERIE_DEL", guiaSalidaBienDetalle.SerieDel));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_DETALLE_SERIE_AL", guiaSalidaBienDetalle.SerieAl));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_DETALLE_ESTADO", guiaSalidaBienDetalle.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", guiaSalidaBienDetalle.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", guiaSalidaBienDetalle.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            guiaSalidaBienDetalle.GuiaSalidaBienDetalleId = (int)reader["GUIA_SALIDA_BIEN_DETALLE_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return guiaSalidaBienDetalle;
        }

        public async Task<List<GuiaSalidaBienDetalle>> FindAll(int id)
        {
            var guiaSalidaBienDetalles = await _context.GuiaSalidaBienDetalles.FromSqlRaw<GuiaSalidaBienDetalle>("USP_GUIA_SALIDA_BIENES_DETALLE_SELALL {0}", id).ToListAsync();
            return guiaSalidaBienDetalles;
        }

        public async Task<GuiaSalidaBienDetalle> FindById(int id)
        {
            var guiaSalidaBienDetalle = (await _context.GuiaSalidaBienDetalles.FromSqlRaw<GuiaSalidaBienDetalle>("USP_GUIA_SALIDA_BIENES_DETALLE_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return guiaSalidaBienDetalle;
        }
    }
}
