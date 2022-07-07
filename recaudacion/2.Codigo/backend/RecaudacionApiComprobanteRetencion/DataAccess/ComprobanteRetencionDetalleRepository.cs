using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecaudacionApiComprobanteRetencion.Domain;

namespace RecaudacionApiComprobanteRetencion.DataAccess
{
    public class ComprobanteRetencionDetalleRepository : IComprobanteRetencionDetalleRepository
    {

        private readonly ComprobanteRetencionContext _context;
        private readonly string _connectionString;

        public ComprobanteRetencionDetalleRepository(ComprobanteRetencionContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<ComprobanteRetencionDetalle> Add(ComprobanteRetencionDetalle comprobanteRetencionDetalle)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_RETENCION_DETALLE_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_ID", comprobanteRetencionDetalle.ComprobanteRetencionId));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ID", comprobanteRetencionDetalle.ComprobantePagoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_TIPO_DOCUMENTO", comprobanteRetencionDetalle.TipoDocumento));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_SERIE", comprobanteRetencionDetalle.Serie));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_CORRELATIVO", comprobanteRetencionDetalle.Correlativo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_FECHA_EMISION", comprobanteRetencionDetalle.FechaEmision));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_IMPORTE_TOTAL", comprobanteRetencionDetalle.ImporteTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_TIPO_MONEDA", comprobanteRetencionDetalle.TipoModena));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_IMPORTE_OPERACION", comprobanteRetencionDetalle.ImporteOperacion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_MODIFICA_NOTA_CREDITO", comprobanteRetencionDetalle.ModificaNotaCredito));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_NUMERO_CORRELATIVO_PAGO", comprobanteRetencionDetalle.NumeroCorrelativoPago));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_FECHA_PAGO", comprobanteRetencionDetalle.FechaPago));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_TIPO_CAMBIO", comprobanteRetencionDetalle.TipoCambio));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_IMPORTE_PAGO", comprobanteRetencionDetalle.ImportePago));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_TASA", comprobanteRetencionDetalle.Tasa));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_IMPORTE_RETENIDO", comprobanteRetencionDetalle.ImporteRetenido));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_FECHA", comprobanteRetencionDetalle.FechaRetencion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_IMPORTE_NETO_PAGAGO", comprobanteRetencionDetalle.ImporteNetoPagado));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_DETALLE_ESTADO", comprobanteRetencionDetalle.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", comprobanteRetencionDetalle.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", comprobanteRetencionDetalle.FechaCreacion));

                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            comprobanteRetencionDetalle.ComprobanteRetencionDetalleId = (int)reader["COMPROBANTE_RETENCION_DETALLE_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return comprobanteRetencionDetalle;
        }

        public async Task<IEnumerable<ComprobanteRetencionDetalle>> FindAll(int id)
        {
            var comprobanteRetencionDetalles = await _context.ComprobanteRetencionDetalles.FromSqlRaw<ComprobanteRetencionDetalle>("USP_COMPROBANTE_RETENCION_DETALLE_SELALL {0}", id).ToListAsync();
            return comprobanteRetencionDetalles;
        }

        public async Task<ComprobanteRetencionDetalle> FindById(int id)
        {
            var comprobanteRetencionDetalle = (await _context.ComprobanteRetencionDetalles.FromSqlRaw<ComprobanteRetencionDetalle>("USP_COMPROBANTE_RETENCION_DETALLE_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return comprobanteRetencionDetalle;
        }
    }
}
