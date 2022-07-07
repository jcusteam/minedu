using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiTipoDocumento.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiTipoDocumento.DataAccess
{
    public class TipoDocumentoRepository : ITipoDocumentoRepository
    {
        private readonly TipoDocumentoContext _context;
        private readonly string _connectionString;

        public TipoDocumentoRepository(TipoDocumentoContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<TipoDocumento> Add(TipoDocumento tipoDocumento)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_TIPO_DOCUMENTO_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", tipoDocumento.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_NOMBRE", tipoDocumento.Nombre));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ABREVIATURA", tipoDocumento.Abreviatura));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ESTADO", tipoDocumento.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", tipoDocumento.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", tipoDocumento.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tipoDocumento.TipoDocumentoId = (int)reader["TIPO_DOCUMENTO_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return tipoDocumento;
        }

        public async Task<int> Count(TipoDocumentoFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_TIPO_DOCUMENTO_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_NOMBRE", filter.Nombre ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ESTADO", filter.Estado ?? SqlBoolean.Null));
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

        public async Task Delete(TipoDocumento tipoDocumento)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_TIPO_DOCUMENTO_DEL {0}", tipoDocumento.TipoDocumentoId);
        }

        public async Task<List<TipoDocumento>> FindAll()
        {
            var tipoDocumentos = await _context.TipoDocumentos.FromSqlRaw<TipoDocumento>("USP_TIPO_DOCUMENTO_SELALL").ToListAsync();
            return tipoDocumentos;
        }

        public async Task<List<TipoDocumento>> FindAll(TipoDocumentoFilter filter)
        {

            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "tipoDocumentoId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_10;

            var tipoDocumentos = await _context.TipoDocumentos
            .FromSqlRaw<TipoDocumento>("USP_TIPO_DOCUMENTO_SEL_PAGE {0},{1},{2},{3},{4},{5}",
            filter.PageNumber, filter.PageSize, filter.SortColumn,
            filter.SortOrder, filter.Nombre, filter.Estado).ToListAsync();
            return tipoDocumentos;
        }

        public async Task<TipoDocumento> FindById(int id)
        {
            var TipoDocumento = (await _context.TipoDocumentos.FromSqlRaw<TipoDocumento>("USP_TIPO_DOCUMENTO_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return TipoDocumento;
        }

        public async Task<Pagination<TipoDocumento>> FindPage(TipoDocumentoFilter filter)
        {
            var pagination = new Pagination<TipoDocumento>();
            pagination.Items = await FindAll(filter);
            pagination.Total = await Count(filter);
            return pagination;
        }

        public async Task<int> GenerateId()
        {
            var id = 1;
            int? maxId = await _context.TipoDocumentos.AsNoTracking().MaxAsync(x => (int?)x.TipoDocumentoId);
            if (maxId != null)
            {
                id = (int)maxId + 1;
            }

            return id;
        }

        public async Task Update(TipoDocumento tipoDocumento)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_TIPO_DOCUMENTO_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", tipoDocumento.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_NOMBRE", tipoDocumento.Nombre));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ABREVIATURA", tipoDocumento.Abreviatura));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ESTADO", tipoDocumento.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", tipoDocumento.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", tipoDocumento.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }

        public async Task<bool> VerifyExists(int tipo, TipoDocumento tipoDocumento)
        {
            var exists = false;

            var tipoDocumentoEx = await _context.TipoDocumentos.Where(x =>
                x.Nombre.ToUpper().Trim() == tipoDocumento.Nombre.ToUpper().Trim() ||
                x.Abreviatura.ToUpper().Trim() == tipoDocumento.Abreviatura.ToUpper().Trim()).AsNoTracking().FirstOrDefaultAsync();

            if (tipoDocumentoEx == null)
            {
                exists = false;
            }
            else
            {
                if (tipo == Definition.INSERT)
                {
                    exists = true;
                }
                else
                {
                    if (tipoDocumentoEx.TipoDocumentoId == tipoDocumento.TipoDocumentoId)
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
