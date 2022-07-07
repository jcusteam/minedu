using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiLiquidacion.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiLiquidacion.DataAccess
{
    public class LiquidacionRepository : ILiquidacionRepository
    {
        private readonly LiquidacionContext _context;
        private readonly string _connectionString;

        public LiquidacionRepository(LiquidacionContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<Liquidacion> Add(Liquidacion liquidacion)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_LIQUIDACION_RECAUDACION_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", liquidacion.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@FUENTE_FINANCIAMIENTO_ID", liquidacion.FuenteFinanciamientoId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", liquidacion.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", liquidacion.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", liquidacion.CuentaCorrienteId));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_NUMERO", liquidacion.Numero));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_PROCEDENCIA", liquidacion.Procedencia));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_FECHA", liquidacion.FechaRegistro));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_TOTAL", liquidacion.Total));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_FACTURA", liquidacion.Factura ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_BOLETA", liquidacion.BoletaVenta ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_ESTADO", liquidacion.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", liquidacion.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", liquidacion.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            liquidacion.LiquidacionId = (int)reader["LIQUIDACION_RECAUDACION_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return liquidacion;
        }

        public async Task<int> Count(LiquidacionFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_LIQUIDACION_RECAUDACION_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", filter.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_NUMERO", filter.Numero ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_ESTADO", filter.Estado ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_ESTADOS", filter.Estados ?? SqlString.Null));
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

        public async Task Delete(Liquidacion liquidacion)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_LIQUIDACION_RECAUDACION_DEL {0}", liquidacion.LiquidacionId);
        }

        public async Task<List<Liquidacion>> FindAll()
        {
            var Liquidacions = await _context.Liquidacions.FromSqlRaw<Liquidacion>("USP_LIQUIDACION_RECAUDACION_SELALL").ToListAsync();
            return Liquidacions;
        }

        public async Task<List<Liquidacion>> FindAll(LiquidacionFilter filter)
        {
            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "liquidacionId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_10;

            var liquidacions = await _context.Liquidacions
            .FromSqlRaw<Liquidacion>("USP_LIQUIDACION_RECAUDACION_SEL_PAGE {0},{1},{2},{3},{4},{5},{6},{7}",
            filter.PageNumber, filter.PageSize, filter.SortColumn, filter.SortOrder,
            filter.UnidadEjecutoraId, filter.Numero, filter.Estado, filter.Estados).ToListAsync();
            return liquidacions;
        }

        public async Task<Liquidacion> FindById(int id)
        {
            var liquidacion = (await _context.Liquidacions.FromSqlRaw<Liquidacion>("USP_LIQUIDACION_RECAUDACION_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return liquidacion;
        }

        public async Task<string> FindNumeroCorrelativo(int unidadEjecutoraId, int tipoDocumentoId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_LIQUIDACION_RECAUDACION_CORRELATIVO", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", unidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", tipoDocumentoId));
                    var numero = "";
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            numero = Convert.ToString(reader["NUMERO"]);
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                    return numero;
                }
            }
        }

        public async Task<List<LiquidacionDetalle>> FindDetalleById(int id)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_LIQUIDACION_RECAUDACION_DETALLE_SELALL", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_ID", id));
                    List<LiquidacionDetalle> list = new List<LiquidacionDetalle>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var detalle = new LiquidacionDetalle();
                            detalle.ClasificadorIngresoId = Convert.ToInt32(reader["CLASIFICADOR_INGRESO_ID"]);
                            detalle.TipoCaptacionId = Convert.ToInt32(reader["TIPO_CAPTACION_ID"]);
                            detalle.ImporteParcial = Convert.ToDecimal(reader["IMPORTE_PARCIAL"]);
                            list.Add(detalle);
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                    return list;
                }
            }
        }

        public async Task Update(Liquidacion liquidacion)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_LIQUIDACION_RECAUDACION_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_ID", liquidacion.LiquidacionId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", liquidacion.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@FUENTE_FINANCIAMIENTO_ID", liquidacion.FuenteFinanciamientoId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", liquidacion.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", liquidacion.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", liquidacion.CuentaCorrienteId));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_ID", liquidacion.ReciboIngresoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_NUMERO", liquidacion.Numero));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_PROCEDENCIA", liquidacion.Procedencia));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_FECHA", liquidacion.FechaRegistro));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_TOTAL", liquidacion.Total));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_FACTURA", liquidacion.Factura ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_BOLETA", liquidacion.BoletaVenta ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_ESTADO", liquidacion.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", liquidacion.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", liquidacion.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }

        public async Task<int> CountDetalleByFecha()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_LIQUIDACION_RECAUDACION_DETALLE_COUNT_FECHA", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    var total = 0;
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            total = Convert.ToInt32(reader["TOTAL"]);
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                    return total;
                }
            }
        }

        public async Task<Pagination<Liquidacion>> FindPage(LiquidacionFilter filter)
        {
            var pagination = new Pagination<Liquidacion>();
            pagination.Items = await FindAll(filter);
            pagination.Total = await Count(filter);
            return pagination;
        }

        public async Task<List<LiquidacionDetalle>> GrupDetalleByClasificador(int id)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_LIQUIDACION_RECAUDACION_DETALLE_SEL_CLASIFICADOR", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@LIQUIDACION_RECAUDACION_ID", id));
                    List<LiquidacionDetalle> list = new List<LiquidacionDetalle>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var detalle = new LiquidacionDetalle();
                            detalle.ClasificadorIngresoId = Convert.ToInt32(reader["CLASIFICADOR_INGRESO_ID"]);
                            detalle.ImporteParcial = Convert.ToDecimal(reader["IMPORTE_PARCIAL"]);
                            list.Add(detalle);
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                    return list;
                }
            }
        }
    }
}
