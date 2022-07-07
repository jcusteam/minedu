using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiComprobanteRetencion.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiComprobanteRetencion.DataAccess
{
    public class ComprobanteRetencionRepository : IComprobanteRetencionRepository
    {
        private readonly ComprobanteRetencionContext _context;
        private readonly string _connectionString;

        public ComprobanteRetencionRepository(ComprobanteRetencionContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<ComprobanteRetencion> Add(ComprobanteRetencion comprobanteRetencion)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_RETENCION_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", comprobanteRetencion.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", comprobanteRetencion.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", comprobanteRetencion.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_COMPROBANTE_PAGO_ID", comprobanteRetencion.TipoComprobanteId));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_SERIE", comprobanteRetencion.Serie));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_CORRELATIVO", comprobanteRetencion.Correlativo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_FECHA_EMISION", comprobanteRetencion.FechaEmision));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_PERIODO", comprobanteRetencion.Periodo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_REGIMEN", comprobanteRetencion.RegimenRetencion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_TOTAL", comprobanteRetencion.Total));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_TOTAL_PAGO", comprobanteRetencion.TotalPago));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_PORCENTAJE", comprobanteRetencion.Porcentaje));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_NOMBRE_ARCHIVO", comprobanteRetencion.NombreArchivo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_OBSERVACION", comprobanteRetencion.Observacion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_ESTADO_SUNAT", comprobanteRetencion.EstadoSunat ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_ESTADO", comprobanteRetencion.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", comprobanteRetencion.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", comprobanteRetencion.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            comprobanteRetencion.ComprobanteRetencionId = (int)reader["COMPROBANTE_RETENCION_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return comprobanteRetencion;
        }

        public async Task<int> Count(ComprobanteRetencionFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_RETENCION_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", filter.UnidadEjecutoraId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", filter.ClienteId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_SERIE", filter.Serie ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_CORRELATIVO", filter.Correlativo ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_FECHA_INICIO", filter.FechaInicio ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_FECHA_FIN", filter.FechaFin ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_ESTADO", filter.Estado ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_ESTADOS", filter.Estados ?? SqlString.Null));
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

        public async Task Delete(ComprobanteRetencion ComprobanteRetencion)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_COMPROBANTE_RETENCION_DEL {0}", ComprobanteRetencion.ComprobanteRetencionId);
        }

        public async Task<List<ComprobanteRetencion>> FindAll()
        {
            var ComprobanteRetencions = await _context.ComprobanteRetenciones.FromSqlRaw<ComprobanteRetencion>("USP_COMPROBANTE_RETENCION_SELALL").ToListAsync();
            return ComprobanteRetencions;
        }

        public async Task<List<ComprobanteRetencion>> FindAll(ComprobanteRetencionFilter filter)
        {
            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "comprobanteRetencionId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_10;

            var comprobanteRetenciones = await _context.ComprobanteRetenciones
            .FromSqlRaw<ComprobanteRetencion>("USP_COMPROBANTE_RETENCION_SEL_PAGE {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
            filter.PageNumber, filter.PageSize, filter.SortColumn, filter.SortOrder,
            filter.UnidadEjecutoraId, filter.ClienteId, filter.Serie,
            filter.Correlativo, filter.FechaInicio, filter.FechaFin, filter.Estado, filter.Estados).ToListAsync();
            return comprobanteRetenciones;
        }

        public async Task<ComprobanteRetencion> FindById(int id)
        {
            var comprobanteRetencion = (await _context.ComprobanteRetenciones.FromSqlRaw<ComprobanteRetencion>("USP_COMPROBANTE_RETENCION_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return comprobanteRetencion;
        }

        public async Task<ComprobanteRetencionParametro> FindParametro(int ejecutoraId, int tipoDocumentoId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_RETENCION_CORRELATIVO", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ejecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", tipoDocumentoId));
                    ComprobanteRetencionParametro comprobante = new ComprobanteRetencionParametro();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            comprobante.Serie = reader["SERIE"].ToString();
                            comprobante.Correlativo = reader["CORRELATIVO"].ToString();
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();

                    return comprobante;
                }
            }
        }

        public async Task<Pagination<ComprobanteRetencion>> FindPage(ComprobanteRetencionFilter filter)
        {
            var pagination = new Pagination<ComprobanteRetencion>();
            var items = await FindAll(filter);
            var total = await Count(filter);

            pagination.Items = items;
            pagination.Total = total;

            return pagination;
        }

        public async Task Update(ComprobanteRetencion comprobanteRetencion)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_RETENCION_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_ID", comprobanteRetencion.ComprobanteRetencionId));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", comprobanteRetencion.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", comprobanteRetencion.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", comprobanteRetencion.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_COMPROBANTE_PAGO_ID", comprobanteRetencion.TipoComprobanteId));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_SERIE", comprobanteRetencion.Serie));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_CORRELATIVO", comprobanteRetencion.Correlativo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_FECHA_EMISION", comprobanteRetencion.FechaEmision));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_PERIODO", comprobanteRetencion.Periodo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_REGIMEN", comprobanteRetencion.RegimenRetencion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_TOTAL", comprobanteRetencion.Total));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_TOTAL_PAGO", comprobanteRetencion.TotalPago));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_PORCENTAJE", comprobanteRetencion.Porcentaje));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_NOMBRE_ARCHIVO", comprobanteRetencion.NombreArchivo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_OBSERVACION", comprobanteRetencion.Observacion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_ESTADO_SUNAT", comprobanteRetencion.EstadoSunat ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_RETENCION_ESTADO", comprobanteRetencion.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", comprobanteRetencion.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", comprobanteRetencion.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }
    }
}
