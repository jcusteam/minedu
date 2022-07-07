using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiReciboIngreso.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiReciboIngreso.DataAccess
{
    public class ReciboIngresoRepository : IReciboIngresoRepository
    {
        private readonly ReciboIngresoContext _context;
        private readonly string _connectionString;

        public ReciboIngresoRepository(ReciboIngresoContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<ReciboIngreso> Add(ReciboIngreso reciboIngreso)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_RECIBO_INGRESOS_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", reciboIngreso.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_RECIBO_INGRESO_ID", reciboIngreso.TipoReciboIngresoId));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", reciboIngreso.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", reciboIngreso.CuentaCorrienteId));
                    cmd.Parameters.Add(new SqlParameter("@FUENTE_FINANCIAMIENTO_ID", reciboIngreso.FuenteFinanciamientoId));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_ID", reciboIngreso.RegistroLineaId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", reciboIngreso.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO", reciboIngreso.Numero));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_FECHA_EMISION", reciboIngreso.FechaEmision));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_CAPTACION_ID", reciboIngreso.TipoCaptacionId));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_ID", reciboIngreso.DepositoBancoDetalleId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_IMPORTE_TOTAL", reciboIngreso.ImporteTotal));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO_DEPOSITO", reciboIngreso.NumeroDeposito ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_FECHA_DEPOSITO", reciboIngreso.FechaDeposito ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO_CHEQUE", reciboIngreso.NumeroCheque ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO_OFICIO", reciboIngreso.NumeroOficio ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO_COMPROBANTE_PAGO", reciboIngreso.NumeroComprobantePago ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_EXPEDIENTE_SIAF", reciboIngreso.ExpedienteSiaf ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO_RESOLUCION", reciboIngreso.NumeroResolucion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_CARTA_ORDEN", reciboIngreso.CartaOrden ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_LIQUIDACION_INGRESO", reciboIngreso.LiquidacionIngreso ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_PAPELETA_DEPOSITO", reciboIngreso.PapeletaDeposito ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_CONCEPTO", reciboIngreso.Concepto ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_REFERENCIA", reciboIngreso.Referencia ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_ID", reciboIngreso.LiquidacionId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_VALIDAR_DEPOSITO", reciboIngreso.ValidarDeposito ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_ESTADO", reciboIngreso.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", reciboIngreso.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", reciboIngreso.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reciboIngreso.ReciboIngresoId = (int)reader["RECIBO_INGRESO_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return reciboIngreso;
        }

        public async Task<int> Count(ReciboIngresoFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_RECIBO_INGRESOS_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", filter.UnidadEjecutoraId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_RECIBO_INGRESO_ID", filter.TipoReciboIngresoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", filter.ClienteId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO", filter.Numero ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_CAPTACION_ID", filter.TipoCaptacionId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_ESTADO", filter.Estado ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_ESTADOS", filter.Estados ?? SqlString.Null));
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

        public async Task Delete(ReciboIngreso ReciboIngreso)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_RECIBO_INGRESOS_DEL {0}", ReciboIngreso.ReciboIngresoId);
        }

        public async Task<List<ReciboIngreso>> FindAll()
        {
            var reciboIngresos = await _context.ReciboIngresos.FromSqlRaw<ReciboIngreso>("USP_RECIBO_INGRESOS_SELALL").ToListAsync();
            return reciboIngresos;
        }

        public async Task<List<ReciboIngreso>> FindAll(ReciboIngresoFilter filter)
        {

            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "reciboIngresoId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_10;

            var reciboIngresos = await _context.ReciboIngresos
            .FromSqlRaw<ReciboIngreso>("USP_RECIBO_INGRESOS_SEL_PAGE {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
            filter.PageNumber, filter.PageSize, filter.SortColumn, filter.SortOrder,
            filter.UnidadEjecutoraId, filter.TipoReciboIngresoId, filter.ClienteId,
            filter.Numero, filter.TipoCaptacionId, filter.Estado, filter.Estados).ToListAsync();

            return reciboIngresos;
        }

        public async Task<ReciboIngreso> FindByNumeroAndEjecutoraAndCuenta(string numero, int ejecutoraId, int cuentaId)
        {
            var reciboIngreso = (await _context.ReciboIngresos.FromSqlRaw<ReciboIngreso>("USP_RECIBO_INGRESOS_SELBYNUMERO_EJECUTORA {0},{1},{2}", numero, ejecutoraId, cuentaId).ToListAsync()).FirstOrDefault();
            return reciboIngreso;
        }

        public async Task<ReciboIngreso> FindById(int id)
        {
            var reciboIngreso = (await _context.ReciboIngresos.FromSqlRaw<ReciboIngreso>("USP_RECIBO_INGRESOS_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return reciboIngreso;
        }

        public async Task<string> FindCorrelativo(int ejecutoraId, int tipoDocId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_RECIBO_INGRESOS_CORRELATIVO", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ejecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", tipoDocId));
                    var numero = "";
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            numero = reader["NUMERO"].ToString();
                        }
                        await reader.CloseAsync();
                    }

                    await sql.CloseAsync();

                    return numero;
                }
            }
        }

        public async Task Update(ReciboIngreso reciboIngreso)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_RECIBO_INGRESOS_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_ID", reciboIngreso.ReciboIngresoId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", reciboIngreso.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_RECIBO_INGRESO_ID", reciboIngreso.TipoReciboIngresoId));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", reciboIngreso.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", reciboIngreso.CuentaCorrienteId));
                    cmd.Parameters.Add(new SqlParameter("@FUENTE_FINANCIAMIENTO_ID", reciboIngreso.FuenteFinanciamientoId));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_ID", reciboIngreso.RegistroLineaId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", reciboIngreso.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO", reciboIngreso.Numero));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_FECHA_EMISION", reciboIngreso.FechaEmision));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_CAPTACION_ID", reciboIngreso.TipoCaptacionId));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_ID", reciboIngreso.DepositoBancoDetalleId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_IMPORTE_TOTAL", reciboIngreso.ImporteTotal));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO_DEPOSITO", reciboIngreso.NumeroDeposito ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_FECHA_DEPOSITO", reciboIngreso.FechaDeposito ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO_CHEQUE", reciboIngreso.NumeroCheque ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO_OFICIO", reciboIngreso.NumeroOficio ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO_COMPROBANTE_PAGO", reciboIngreso.NumeroComprobantePago ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_EXPEDIENTE_SIAF", reciboIngreso.ExpedienteSiaf ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_NUMERO_RESOLUCION", reciboIngreso.NumeroResolucion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_CARTA_ORDEN", reciboIngreso.CartaOrden ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_LIQUIDACION_INGRESO", reciboIngreso.LiquidacionIngreso ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_PAPELETA_DEPOSITO", reciboIngreso.PapeletaDeposito ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_CONCEPTO", reciboIngreso.Concepto ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_REFERENCIA", reciboIngreso.Referencia ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_ID", reciboIngreso.LiquidacionId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_VALIDAR_DEPOSITO", reciboIngreso.ValidarDeposito ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_ESTADO", reciboIngreso.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", reciboIngreso.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", reciboIngreso.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }

        public async Task<List<Chart>> FindChartTipoRecibo(int tipoReciboId, int ejecutoraId, int? anio)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_RECIBO_INGRESOS_SELCHART", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TIPO_RECIBO_INGRESO_ID", tipoReciboId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ejecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@ANIO", anio ?? SqlInt32.Null));
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

        public async Task<Pagination<ReciboIngreso>> FindPage(ReciboIngresoFilter filter)
        {
            var pagination = new Pagination<ReciboIngreso>();
            pagination.Items = await FindAll(filter);
            pagination.Total = await Count(filter);
            return pagination;
        }

        public async Task<List<ChartTipoRecibo>> FindChart(List<TipoReciboIngreso> lista, int ejecutoraId, int? anio)
        {
            var listCharts = new List<ChartTipoRecibo>();

            foreach (var item in lista)
            {
                var chartTipoRecibo = new ChartTipoRecibo();
                chartTipoRecibo.TipoId = item.tipoReciboIngresoId;
                chartTipoRecibo.TipoName = item.nombre;
                listCharts.Add(chartTipoRecibo);
            }

            foreach (var item in listCharts)
            {
                var charts = await FindChartTipoRecibo(item.TipoId, ejecutoraId, anio);
                item.charts = charts;
            }

            return listCharts;
        }
    }
}
