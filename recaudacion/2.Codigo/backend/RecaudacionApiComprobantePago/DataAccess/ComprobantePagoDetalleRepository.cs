using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecaudacionApiComprobantePago.Domain;

namespace RecaudacionApiComprobantePago.DataAccess
{
    public class ComprobantePagoDetalleRepository : IComprobantePagoDetalleRepository
    {
        private readonly ComprobantePagoContext _context;
        private readonly string _connectionString;

        public ComprobantePagoDetalleRepository(ComprobantePagoContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ComprobantePagoDetalle> Add(ComprobantePagoDetalle comprobantePagoDetalle)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_PAGOS_DETALLE_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ID", comprobantePagoDetalle.ComprobantePagoId));
                    cmd.Parameters.Add(new SqlParameter("@CATALOGO_BIEN_ID", comprobantePagoDetalle.CatalogoBienId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@TARIFARIO_ID", comprobantePagoDetalle.TarifarioId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CLASIFICADOR_INGRESO_ID", comprobantePagoDetalle.ClasificadorIngresoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_UNIDAD_MEDIDA", comprobantePagoDetalle.UnidadMedida));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_CANTIDAD", comprobantePagoDetalle.Cantidad));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_CODIGO", comprobantePagoDetalle.Codigo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_DESCRIPCION", comprobantePagoDetalle.Descripcion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_CODIGO_TIPO_MONEDA", comprobantePagoDetalle.CodigoTipoMoneda));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_PRECIO_UNITARIO", comprobantePagoDetalle.PrecioUnitario));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_CODIGO_TIPO_PRECIO", comprobantePagoDetalle.CodigoTipoPrecio));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_AFECTO_IGV", comprobantePagoDetalle.AfectoIGV));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_IGV_ITEM", comprobantePagoDetalle.IGVItem));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_CODIGO_TIPO_IGV", comprobantePagoDetalle.CodigoTipoIGV));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_DESCUENTO_ITEM", comprobantePagoDetalle.DescuentoItem));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_DESCUENTO_TOTAL", comprobantePagoDetalle.DescuentoTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_FACTOR_DESCUENTO", comprobantePagoDetalle.FactorDescuento));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_SUB_TOTAL", comprobantePagoDetalle.SubTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_VALOR_VENTA", comprobantePagoDetalle.ValorVenta));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_DETALLE_ID", comprobantePagoDetalle.IngresoPecosaDetalleId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_SERIE_FORMATO", comprobantePagoDetalle.SerieFormato ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_SERIE_DEL", comprobantePagoDetalle.SerieDel ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_SERIE_AL", comprobantePagoDetalle.SerieAl ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_DETALLE_ESTADO", comprobantePagoDetalle.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", comprobantePagoDetalle.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", comprobantePagoDetalle.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            comprobantePagoDetalle.ComprobantePagoDetalleId = (int)reader["COMPROBANTE_PAGO_DETALLE_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return comprobantePagoDetalle;
        }

        public async Task<IEnumerable<ComprobantePagoDetalle>> FindAll(int id)
        {
            return await _context.ComprobantePagoDetalles.FromSqlRaw<ComprobantePagoDetalle>("USP_COMPROBANTE_PAGOS_DETALLE_SELALL {0}", id).ToListAsync();
        }

        public async Task<ComprobantePagoDetalle> FindById(int id)
        {
            var comprobantePagoDetalle = (await _context.ComprobantePagoDetalles.FromSqlRaw<ComprobantePagoDetalle>("USP_COMPROBANTE_PAGOS_DETALLE_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return comprobantePagoDetalle;
        }
    }
}
