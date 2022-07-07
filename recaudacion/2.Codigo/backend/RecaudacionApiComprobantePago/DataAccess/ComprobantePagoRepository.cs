using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiComprobantePago.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiComprobantePago.DataAccess
{
    public class ComprobantePagoRepository : IComprobantePagoRepository
    {
        private readonly ComprobantePagoContext _context;
        private readonly string _connectionString;

        public ComprobantePagoRepository(ComprobantePagoContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<ComprobantePago> Add(ComprobantePago ComprobantePago)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_PAGOS_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", ComprobantePago.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ComprobantePago.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_COMPROBANTE_PAGO_ID", ComprobantePago.TipoComprobanteId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", ComprobantePago.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_ID", ComprobantePago.DepositoBancoDetalleId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", ComprobantePago.CuentaCorrienteId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_CAPTACION_ID", ComprobantePago.TipoCaptacionId));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_ID", ComprobantePago.ComprobanteEmisorId));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_SERIE", ComprobantePago.Serie));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_CORRELATIVO", ComprobantePago.Correlativo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FECHA_EMISION", ComprobantePago.FechaEmision));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FECHA_VENCIMIENTO", ComprobantePago.FechaVencimiento));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TIPO_ADQUISICION", ComprobantePago.TipoAdquisicion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_CODIGO_TIPO_OPERACION", ComprobantePago.CodigoTipoOperacion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TIPO_CONDICION_PAGO", ComprobantePago.TipoCondicionPago));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_NUMERO_DEPOSITO", ComprobantePago.NumeroDeposito ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FECHA_DEPOSITO", ComprobantePago.FechaDeposito ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_VALIDAR_DEPOSITO", ComprobantePago.ValidarDeposito ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_NUMERO_CHEQUE", ComprobantePago.NumeroCheque ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ENCARGADO_TIPO_DOCUMENTO", ComprobantePago.EncargadoTipoDocumento ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ENCARGADO_NOMBRE", ComprobantePago.EncargadoNombre ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ENCARGADO_NUMERO_DOCUMENTO", ComprobantePago.EncargadoNumeroDocumento ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_ID", ComprobantePago.FuenteId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_TIPO_DOCUMENTO", ComprobantePago.FuenteTipoDocumento ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_SERIE", ComprobantePago.FuenteSerie ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_CORRELATIVO", ComprobantePago.FuenteCorrelativo ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_ORIGEN", ComprobantePago.FuenteOrigen ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_VALIDAR", ComprobantePago.FuenteValidar ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_SUSTENTO", ComprobantePago.Sustento ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_OBSERVACION", ComprobantePago.Observacion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_NOMBRE_ARCHIVO", ComprobantePago.NombreArchivo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TIPO_CAMBIO", ComprobantePago.TipoCambio));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_PAGADO", ComprobantePago.Pagado));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ESTADO_SUNAT", ComprobantePago.EstadoSunat ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_CODIGO_TIPO_MONEDA", ComprobantePago.CodigoTipoMoneda));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_IMPORTE_BRUTO", ComprobantePago.ImporteBruto));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_VALOR_IGV", ComprobantePago.ValorIGV));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_IGV_TOTAL", ComprobantePago.IGVTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ISC_TOTAL", ComprobantePago.ISCTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_OTR_TOTAL", ComprobantePago.OTRTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_OTRC_TOTAL", ComprobantePago.OTRCTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_IMPORTE_TOTAL", ComprobantePago.ImporteTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_IMPORTE_TOTAL_LETRA", ComprobantePago.ImporteTotalLetra));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TOTAL_VENTA_OPGRAVADA", ComprobantePago.TotalOpGravada));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TOTAL_VENTA_OPINAFECTA", ComprobantePago.TotalOpInafecta));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TOTAL_VENTA_OPEXONERADA", ComprobantePago.TotalOpExonerada));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TOTAL_VENTA_OPGRATUITA", ComprobantePago.TotalOpGratuita));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TOTAL_DESCUENTO", ComprobantePago.TotalDescuento));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ORDEN_COMPRA", ComprobantePago.OrdenCompra ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_GUIA_REMISION", ComprobantePago.GuiaRemision ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_CODIGO_TIPO_NOTA", ComprobantePago.CodigoTipoNota ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_CODIGO_MOTIVO_NOTA", ComprobantePago.CodigoMotivoNota ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ESTADO", ComprobantePago.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", ComprobantePago.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", ComprobantePago.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ComprobantePago.ComprobantePagoId = (int)reader["COMPROBANTE_PAGO_ID"];
                            ComprobantePago.ImporteTotalLetra = (string)reader["COMPROBANTE_PAGO_IMPORTE_TOTAL_LETRA"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return ComprobantePago;
        }

        public async Task<int> Count(ComprobantePagoFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_PAGOS_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", filter.UnidadEjecutoraId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", filter.TipoDocumentoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", filter.ClienteId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_SERIE", filter.Serie ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_CORRELATIVO", filter.Correlativo ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_CAPTACION_ID", filter.TipoCaptacionId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TIPO_ADQUISICION", filter.TipoAdquisicion ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FECHA_INICIO", filter.FechaInicio ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FECHA_FIN", filter.FechaFin ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ESTADO", filter.Estado ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ESTADOS", filter.Estados ?? SqlString.Null));
                    int count = 0;
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            count = Convert.ToInt32(reader["TOTAL"]);
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                    return count;
                }
            }
        }

        public async Task Delete(ComprobantePago ComprobantePago)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_COMPROBANTE_PAGOS_DEL {0}", ComprobantePago.ComprobantePagoId);
        }

        public async Task<List<ComprobantePago>> FindAll()
        {
            var ComprobantePagos = await _context.ComprobantePagos.FromSqlRaw<ComprobantePago>("USP_COMPROBANTE_PAGOS_SELALL").ToListAsync();
            return ComprobantePagos;
        }

        public async Task<List<ComprobantePago>> FindAll(ComprobantePagoFilter filter)
        {
            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "comprobantePagoId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_10;

            var comprobantePagos = await _context.ComprobantePagos
            .FromSqlRaw<ComprobantePago>("USP_COMPROBANTE_PAGOS_SEL_PAGE {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
            filter.PageNumber, filter.PageSize, filter.SortColumn, filter.SortOrder,
            filter.UnidadEjecutoraId, filter.TipoDocumentoId, filter.ClienteId, filter.Serie, filter.Correlativo,
            filter.TipoCaptacionId, filter.TipoAdquisicion, filter.FechaInicio, filter.FechaFin, filter.Estado, filter.Estados).ToListAsync();

            return comprobantePagos;
        }

        public async Task<ComprobantePago> FindById(int id)
        {
            var ComprobantePago = (await _context.ComprobantePagos.FromSqlRaw<ComprobantePago>("USP_COMPROBANTE_PAGOS_SELBYID {0}", id).AsNoTracking().ToListAsync()).FirstOrDefault();
            return ComprobantePago;
        }

        public async Task<List<Chart>> FindChartByTipo(int tipoId, int ejecutoraId, int anio)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_PAGOS_SELCHART", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TIPO_COMPROBANTE_PAGO_ID", tipoId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ejecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@ANIO", anio));
                    await sql.OpenAsync();

                    List<Chart> charts = new List<Chart>();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var chart = new Chart();
                            chart.Id = Convert.ToInt32(reader["ID"]);
                            chart.MonthName = Convert.ToString(reader["MONTH_NAME"]);
                            chart.TipoId = Convert.ToInt32(reader["TIPO_ID"]);
                            chart.Total = Convert.ToInt32(reader["TOTAL"]);
                            charts.Add(chart);
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                    return charts;
                }
            }
        }

        public async Task<ComprobantePagoParametro> FindParametro(int ejecutoraId, int tipo)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_PAGOS_CORRELATIVO", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ejecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", tipo));
                    ComprobantePagoParametro comprobantePagoParametro = new ComprobantePagoParametro();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            comprobantePagoParametro.Serie = reader["SERIE"].ToString();
                            comprobantePagoParametro.Correlativo = reader["CORRELATIVO"].ToString();
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();

                    return comprobantePagoParametro;
                }
            }
        }

        public async Task<Pagination<ComprobantePago>> FindPage(ComprobantePagoFilter filter)
        {
            var pagination = new Pagination<ComprobantePago>();
            var items = await FindAll(filter);
            var total = await Count(filter);

            pagination.Items = items;
            pagination.Total = total;

            return pagination;
        }

        public async Task Update(ComprobantePago ComprobantePago)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_PAGOS_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ID", ComprobantePago.ComprobantePagoId));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", ComprobantePago.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ComprobantePago.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_COMPROBANTE_PAGO_ID", ComprobantePago.TipoComprobanteId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", ComprobantePago.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_ID", ComprobantePago.DepositoBancoDetalleId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", ComprobantePago.CuentaCorrienteId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_CAPTACION_ID", ComprobantePago.TipoCaptacionId));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_ID", ComprobantePago.ComprobanteEmisorId));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_SERIE", ComprobantePago.Serie));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_CORRELATIVO", ComprobantePago.Correlativo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FECHA_EMISION", ComprobantePago.FechaEmision));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FECHA_VENCIMIENTO", ComprobantePago.FechaVencimiento));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TIPO_ADQUISICION", ComprobantePago.TipoAdquisicion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_CODIGO_TIPO_OPERACION", ComprobantePago.CodigoTipoOperacion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TIPO_CONDICION_PAGO", ComprobantePago.TipoCondicionPago));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_NUMERO_DEPOSITO", ComprobantePago.NumeroDeposito ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FECHA_DEPOSITO", ComprobantePago.FechaDeposito ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_VALIDAR_DEPOSITO", ComprobantePago.ValidarDeposito ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_NUMERO_CHEQUE", ComprobantePago.NumeroCheque ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ENCARGADO_TIPO_DOCUMENTO", ComprobantePago.EncargadoTipoDocumento ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ENCARGADO_NOMBRE", ComprobantePago.EncargadoNombre ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ENCARGADO_NUMERO_DOCUMENTO", ComprobantePago.EncargadoNumeroDocumento ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_ID", ComprobantePago.FuenteId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_TIPO_DOCUMENTO", ComprobantePago.FuenteTipoDocumento ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_SERIE", ComprobantePago.FuenteSerie ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_CORRELATIVO", ComprobantePago.FuenteCorrelativo ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_ORIGEN", ComprobantePago.FuenteOrigen ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_FUENTE_VALIDAR", ComprobantePago.FuenteValidar ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_SUSTENTO", ComprobantePago.Sustento ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_OBSERVACION", ComprobantePago.Observacion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_NOMBRE_ARCHIVO", ComprobantePago.NombreArchivo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TIPO_CAMBIO", ComprobantePago.TipoCambio));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_PAGADO", ComprobantePago.Pagado));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ESTADO_SUNAT", ComprobantePago.EstadoSunat ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_CODIGO_TIPO_MONEDA", ComprobantePago.CodigoTipoMoneda));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_IMPORTE_BRUTO", ComprobantePago.ImporteBruto));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_VALOR_IGV", ComprobantePago.ValorIGV));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_IGV_TOTAL", ComprobantePago.IGVTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ISC_TOTAL", ComprobantePago.ISCTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_OTR_TOTAL", ComprobantePago.OTRTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_OTRC_TOTAL", ComprobantePago.OTRCTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_IMPORTE_TOTAL", ComprobantePago.ImporteTotal));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_IMPORTE_TOTAL_LETRA", ComprobantePago.ImporteTotalLetra));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TOTAL_VENTA_OPGRAVADA", ComprobantePago.TotalOpGravada));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TOTAL_VENTA_OPINAFECTA", ComprobantePago.TotalOpInafecta));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TOTAL_VENTA_OPEXONERADA", ComprobantePago.TotalOpExonerada));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TOTAL_VENTA_OPGRATUITA", ComprobantePago.TotalOpGratuita));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_TOTAL_DESCUENTO", ComprobantePago.TotalDescuento));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ORDEN_COMPRA", ComprobantePago.OrdenCompra ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_GUIA_REMISION", ComprobantePago.GuiaRemision ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_CODIGO_TIPO_NOTA", ComprobantePago.CodigoTipoNota ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_CODIGO_MOTIVO_NOTA", ComprobantePago.CodigoMotivoNota ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_PAGO_ESTADO", ComprobantePago.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", ComprobantePago.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", ComprobantePago.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }

        public async Task<bool> VerifyExists(int tipo, ComprobantePago comprobantePago)
        {
            var exists = false;

            var comprobanteEx = await _context.ComprobantePagos
                .Where(x => x.NombreArchivo == comprobantePago.NombreArchivo &&
                       x.TipoDocumentoId == comprobantePago.TipoDocumentoId &&
                       x.UnidadEjecutoraId == comprobantePago.UnidadEjecutoraId).FirstOrDefaultAsync();

            if (comprobanteEx == null)
            {
                exists = false;
            }
            else
            {
                if (tipo == 1)
                {
                    exists = true;
                }
                else
                {
                    if (comprobanteEx.ComprobantePagoId == comprobantePago.ComprobantePagoId)
                    {
                        exists = false;
                    }
                    else
                    {
                        exists = true;
                    }
                }
            }
            return exists;

        }

        public async Task<ComprobantePago> FindByFuente(ComprobantePago comprobantePago)
        {
            var fuente = (await _context.ComprobantePagos.FromSqlRaw<ComprobantePago>("USP_COMPROBANTE_PAGOS_SEL_FUENTE {0},{1},{2},{3}",
            comprobantePago.UnidadEjecutoraId, comprobantePago.TipoComprobanteId, comprobantePago.Serie, comprobantePago.Correlativo).ToListAsync()).FirstOrDefault();

            return fuente;
        }

        public async Task<int> CountByFuente(int fuenteId)
        {
            var comprobantes = await _context.ComprobantePagos.Where(x => x.FuenteId == fuenteId).ToListAsync();
            return comprobantes.Count;
        }
    }
}
