using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiIngresoPecosa.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace RecaudacionApiIngresoPecosa.DataAccess
{
    public class IngresoPecosaDetalleRepository : IIngresoPecosaDetalleRepository
    {
        private readonly IngresoPecosaContext _context;
        private readonly string _connectionString;

        public IngresoPecosaDetalleRepository(IngresoPecosaContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IngresoPecosaDetalle> Add(IngresoPecosaDetalle ingresoPecosaDetalle)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_INGRESO_PECOSA_DETALLE_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_ID", ingresoPecosaDetalle.IngresoPecosaId));
                    cmd.Parameters.Add(new SqlParameter("@CATALOGO_BIEN_ID", ingresoPecosaDetalle.CatalogoBienId));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_UNIDAD_MEDIDA", ingresoPecosaDetalle.UnidadMedida));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_CODIGO_ITEM", ingresoPecosaDetalle.CodigoItem));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_NOMBRE_ITEM", ingresoPecosaDetalle.NombreItem));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_NOMBRE_MARCA", ingresoPecosaDetalle.NombreMarca));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_CANTIDAD", ingresoPecosaDetalle.Cantidad));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_CANTIDAD_SALIDA", ingresoPecosaDetalle.CantidadSalida));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_PRECIO_UNITARIO", ingresoPecosaDetalle.PrecioUnitario));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_VALOR_TOTAL", ingresoPecosaDetalle.ValorTotal));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_SERIE_FORMATO", ingresoPecosaDetalle.SerieFormato));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_SERIE_DEL", ingresoPecosaDetalle.SerieDel));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_SERIE_AL", ingresoPecosaDetalle.SerieAl));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_ESTADO", ingresoPecosaDetalle.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", ingresoPecosaDetalle.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", ingresoPecosaDetalle.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ingresoPecosaDetalle.IngresoPecosaDetalleId = (int)reader["INGRESO_PECOSA_DETALLE_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return ingresoPecosaDetalle;
        }

        public async Task<int> CountByCatalogoBienSaldo(int catalogoBienId)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_INGRESO_PECOSA_DETALLE_COUNT_SALDO", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CATALOGO_BIEN_ID", catalogoBienId));
                    await sql.OpenAsync();
                    var count = 0;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            count = (int)reader["TOTAL"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();

                    return count;
                }
            }


        }

        public async Task<IEnumerable<IngresoPecosaDetalle>> FindAll(int id)
        {
            var ingresoPecosaDetalles = await _context.IngresoPecosaDetalles.FromSqlRaw<IngresoPecosaDetalle>("USP_INGRESO_PECOSA_DETALLE_SELALL {0}", id).ToListAsync();
            return ingresoPecosaDetalles;
        }

        public async Task<List<IngresoPecosaDetalle>> FindByCatalogoBienSaldo(int catalogoBienId)
        {
            var ingresoPecosaDetalles = await _context.IngresoPecosaDetalles.FromSqlRaw<IngresoPecosaDetalle>("USP_INGRESO_PECOSA_DETALLE_SELBYCATALOGO {0}", catalogoBienId).ToListAsync();

            return ingresoPecosaDetalles;
        }

        public async Task<IngresoPecosaDetalle> FindById(int id)
        {
            var ingresoPecosaDetalle = (await _context.IngresoPecosaDetalles.FromSqlRaw<IngresoPecosaDetalle>("USP_INGRESO_PECOSA_DETALLE_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return ingresoPecosaDetalle;
        }

        public async Task Update(IngresoPecosaDetalle ingresoPecosaDetalle)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_INGRESO_PECOSA_DETALLE_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_ID", ingresoPecosaDetalle.IngresoPecosaDetalleId));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_ID", ingresoPecosaDetalle.IngresoPecosaId));
                    cmd.Parameters.Add(new SqlParameter("@CATALOGO_BIEN_ID", ingresoPecosaDetalle.CatalogoBienId));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_UNIDAD_MEDIDA", ingresoPecosaDetalle.UnidadMedida));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_CODIGO_ITEM", ingresoPecosaDetalle.CodigoItem));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_NOMBRE_ITEM", ingresoPecosaDetalle.NombreItem));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_NOMBRE_MARCA", ingresoPecosaDetalle.NombreMarca));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_CANTIDAD", ingresoPecosaDetalle.Cantidad));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_CANTIDAD_SALIDA", ingresoPecosaDetalle.CantidadSalida));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_PRECIO_UNITARIO", ingresoPecosaDetalle.PrecioUnitario));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_VALOR_TOTAL", ingresoPecosaDetalle.ValorTotal));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_SERIE_FORMATO", ingresoPecosaDetalle.SerieFormato));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_SERIE_DEL", ingresoPecosaDetalle.SerieDel));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_SERIE_AL", ingresoPecosaDetalle.SerieAl));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_ESTADO", ingresoPecosaDetalle.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", ingresoPecosaDetalle.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", ingresoPecosaDetalle.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }
        }
    }
}
