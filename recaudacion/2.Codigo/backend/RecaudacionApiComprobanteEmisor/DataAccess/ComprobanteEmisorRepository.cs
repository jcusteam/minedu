using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiComprobanteEmisor.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiComprobanteEmisor.DataAccess
{
    public class ComprobanteEmisorRepository : IComprobanteEmisorRepository
    {
        private readonly ComprobanteEmisorContext _context;
        private readonly string _connectionString;

        public ComprobanteEmisorRepository(ComprobanteEmisorContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<ComprobanteEmisor> Add(ComprobanteEmisor ComprobanteEmisor)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_EMISOR_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //copiar
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_ID", ComprobanteEmisor.ComprobanteEmisorId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ComprobanteEmisor.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_FIRMANTE", ComprobanteEmisor.Firmante));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_RUC", ComprobanteEmisor.NumeroRuc));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_TIPO_DOCUMENTO", ComprobanteEmisor.TipoDocumento));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_NOMBRE_COMERCIAL", ComprobanteEmisor.NombreComercial));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_RAZON_SOCIAL", ComprobanteEmisor.RazonSocial));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_UBIGEO", ComprobanteEmisor.Ubigeo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_DIRECCION", ComprobanteEmisor.Direccion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_URBANIZACION", ComprobanteEmisor.Urbanizacion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_DEPARTAMENTO", ComprobanteEmisor.Departamento));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_PROVINCIA", ComprobanteEmisor.Provincia));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_DISTRITO", ComprobanteEmisor.Distrito));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_CODIGO_PAIS", ComprobanteEmisor.CodigoPais));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_TELEFONO", ComprobanteEmisor.Telefono ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_DIRECCION_ALTERNATIVA", ComprobanteEmisor.DireccionAlternativa ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_NUMERO_RESOLUCION", ComprobanteEmisor.NumeroResolucion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_USUARIO_OSE", ComprobanteEmisor.UsuarioOSE));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_CLAVE_OSE", ComprobanteEmisor.ClaveOSE));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_CORREO_ENVIO", ComprobanteEmisor.CorreoEnvio ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_CORREO_CLAVE", ComprobanteEmisor.CorreoClave ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_SERVER_MAIL", ComprobanteEmisor.ServerMail ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_SEVER_PORT", ComprobanteEmisor.ServerPort ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_NOMBRE_CERTIFICADO", ComprobanteEmisor.NombreArchivoCer));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_NOMBRE_KEY", ComprobanteEmisor.NombreArchivoKey));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_ESTADO", ComprobanteEmisor.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", ComprobanteEmisor.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", ComprobanteEmisor.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ComprobanteEmisor.ComprobanteEmisorId = (int)reader["COMPROBANTE_EMISOR_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return ComprobanteEmisor;
        }

        public async Task<int> Count(ComprobanteEmisorFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_EMISOR_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", filter.UnidadEjecutoraId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_ESTADO", filter.Estado ?? SqlBoolean.Null));
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

        public async Task Delete(ComprobanteEmisor ComprobanteEmisor)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_COMPROBANTE_EMISOR_DEL {0}", ComprobanteEmisor.ComprobanteEmisorId);
        }

        public async Task<List<ComprobanteEmisor>> FindAll()
        {
            var ComprobanteEmisors = await _context.ComprobanteEmisors.FromSqlRaw<ComprobanteEmisor>("USP_COMPROBANTE_EMISOR_SELALL").ToListAsync();
            return ComprobanteEmisors;
        }

        public async Task<List<ComprobanteEmisor>> FindAll(ComprobanteEmisorFilter filter)
        {
            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "comprobanteEmisorId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_10;

            var ComprobanteEmisors = await _context.ComprobanteEmisors
            .FromSqlRaw<ComprobanteEmisor>("USP_COMPROBANTE_EMISOR_SEL_PAGE {0},{1},{2},{3},{4},{5}",
            filter.PageNumber, filter.PageSize, filter.SortColumn,
            filter.SortOrder, filter.UnidadEjecutoraId, filter.Estado).ToListAsync();
            return ComprobanteEmisors;
        }

        public async Task<ComprobanteEmisor> FindById(int id)
        {
            var ComprobanteEmisor = (await _context.ComprobanteEmisors.FromSqlRaw<ComprobanteEmisor>("USP_COMPROBANTE_EMISOR_SELBYID {0}", id).AsNoTracking().ToListAsync()).FirstOrDefault();
            return ComprobanteEmisor;
        }

        public async Task<ComprobanteEmisor> FindByUnidadEjecutoraId(int unidadEjecutoraId)
        {
            var comprobanteEmisor = (await _context.ComprobanteEmisors.FromSqlRaw<ComprobanteEmisor>("USP_COMPROBANTE_EMISOR_SELBYUNIDAD_EJECUTORA {0}", unidadEjecutoraId).ToListAsync()).FirstOrDefault();
            return comprobanteEmisor;
        }

        public async Task<Pagination<ComprobanteEmisor>> FindPage(ComprobanteEmisorFilter filter)
        {
            var pagination = new Pagination<ComprobanteEmisor>();
            pagination.Items = await FindAll(filter);
            pagination.Total = await Count(filter);

            return pagination;

        }

        public async Task<int> GenerateId()
        {
            var id = 1;
            int? maxId = await _context.ComprobanteEmisors.AsNoTracking().MaxAsync(x => (int?)x.ComprobanteEmisorId);
            if (maxId != null)
            {
                id = (int)maxId + 1;
            }

            return id;
        }

        public async Task Update(ComprobanteEmisor ComprobanteEmisor)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_COMPROBANTE_EMISOR_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_ID", ComprobanteEmisor.ComprobanteEmisorId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ComprobanteEmisor.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_FIRMANTE", ComprobanteEmisor.Firmante));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_RUC", ComprobanteEmisor.NumeroRuc));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_TIPO_DOCUMENTO", ComprobanteEmisor.TipoDocumento));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_NOMBRE_COMERCIAL", ComprobanteEmisor.NombreComercial));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_RAZON_SOCIAL", ComprobanteEmisor.RazonSocial));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_UBIGEO", ComprobanteEmisor.Ubigeo));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_DIRECCION", ComprobanteEmisor.Direccion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_URBANIZACION", ComprobanteEmisor.Urbanizacion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_DEPARTAMENTO", ComprobanteEmisor.Departamento));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_PROVINCIA", ComprobanteEmisor.Provincia));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_DISTRITO", ComprobanteEmisor.Distrito));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_CODIGO_PAIS", ComprobanteEmisor.CodigoPais));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_TELEFONO", ComprobanteEmisor.Telefono ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_DIRECCION_ALTERNATIVA", ComprobanteEmisor.DireccionAlternativa ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_NUMERO_RESOLUCION", ComprobanteEmisor.NumeroResolucion));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_USUARIO_OSE", ComprobanteEmisor.UsuarioOSE));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_CLAVE_OSE", ComprobanteEmisor.ClaveOSE));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_CORREO_ENVIO", ComprobanteEmisor.CorreoEnvio ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_CORREO_CLAVE", ComprobanteEmisor.CorreoClave ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_SERVER_MAIL", ComprobanteEmisor.ServerMail ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_SEVER_PORT", ComprobanteEmisor.ServerPort ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_NOMBRE_CERTIFICADO", ComprobanteEmisor.NombreArchivoCer));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_NOMBRE_KEY", ComprobanteEmisor.NombreArchivoKey));
                    cmd.Parameters.Add(new SqlParameter("@COMPROBANTE_EMISOR_ESTADO", ComprobanteEmisor.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", ComprobanteEmisor.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", ComprobanteEmisor.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }

        public async Task<bool> VerifyExists(int tipo, ComprobanteEmisor comprobanteEmisor)
        {
            var exists = false;

            var comprobanteEmisorEx = await _context.ComprobanteEmisors
                .Where(x => x.UnidadEjecutoraId == comprobanteEmisor.UnidadEjecutoraId && x.Estado == true).AsNoTracking().FirstOrDefaultAsync();

            if (comprobanteEmisorEx == null)
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
                    if (comprobanteEmisorEx.ComprobanteEmisorId == comprobanteEmisor.ComprobanteEmisorId)
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
    }
}
