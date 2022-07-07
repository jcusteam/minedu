using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiParametro.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiParametro.DataAccess
{
    public class ParametroRepository : IParametroRepository
    {
        private readonly ParametroContext _context;
        private readonly string _connectionString;

        public ParametroRepository(ParametroContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<Parametro> Add(Parametro parametro)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_PARAMETROS_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", parametro.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", parametro.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@PARAMETRO_SERIE", parametro.Serie));
                    cmd.Parameters.Add(new SqlParameter("@PARAMETRO_CORRELATIVO", parametro.Correlativo));
                    cmd.Parameters.Add(new SqlParameter("@PARAMETRO_ESTADO", parametro.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", parametro.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", parametro.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            parametro.ParametroId = (int)reader["PARAMETRO_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return parametro;
        }

        public async Task<int> Count(ParametroFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_PARAMETROS_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", filter.UnidadEjecutoraId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", filter.TipoDocumentoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@PARAMETRO_ESTADO", filter.Estado ?? SqlBoolean.Null));
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

        public async Task Delete(Parametro Parametro)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_PARAMETROS_DEL {0}", Parametro.ParametroId);
        }

        public async Task<List<Parametro>> FindAll()
        {
            var parametros = await _context.Parametros.FromSqlRaw<Parametro>("USP_PARAMETROS_SELALL").ToListAsync();
            return parametros;
        }

        public async Task<List<Parametro>> FindAll(ParametroFilter filter)
        {
            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "parametroId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_5;

            var parametros = await _context.Parametros
            .FromSqlRaw<Parametro>("USP_PARAMETROS_SEL_PAGE {0},{1},{2},{3},{4},{5},{6}",
            filter.PageNumber, filter.PageSize, filter.SortColumn, filter.SortOrder,
            filter.UnidadEjecutoraId, filter.TipoDocumentoId, filter.Estado).ToListAsync();
            return parametros;
        }

        public async Task<Parametro> FindByEjecutoraAndTipo(int ejecutoraId, int tipoId)
        {
            var parametro = (await _context.Parametros.FromSqlRaw<Parametro>("USP_PARAMETROS_SELBYUNIDAD_TIPO_DOC {0},{1}", ejecutoraId, tipoId).ToListAsync()).FirstOrDefault();
            return parametro;
        }

        public async Task<Parametro> FindById(int id)
        {
            var parametro = (await _context.Parametros.FromSqlRaw<Parametro>("USP_PARAMETROS_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return parametro;
        }

        public async Task<Pagination<Parametro>> Findpage(ParametroFilter filter)
        {
            var pagination = new Pagination<Parametro>();
            pagination.Items = await FindAll(filter);
            pagination.Total = await Count(filter);
            return pagination;
        }

        public async Task Update(Parametro parametro)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_PARAMETROS_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@PARAMETRO_ID", parametro.ParametroId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", parametro.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", parametro.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@PARAMETRO_SERIE", parametro.Serie));
                    cmd.Parameters.Add(new SqlParameter("@PARAMETRO_CORRELATIVO", parametro.Correlativo));
                    cmd.Parameters.Add(new SqlParameter("@PARAMETRO_ESTADO", parametro.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", parametro.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", parametro.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }

        public async Task<bool> VerifyExists(int tipo, Parametro parametro)
        {
            var exists = false;

            var paramentroEx = await _context.Parametros.Where(x =>
                x.TipoDocumentoId == parametro.TipoDocumentoId &&
                x.UnidadEjecutoraId == parametro.UnidadEjecutoraId).AsNoTracking().FirstOrDefaultAsync();

            if (paramentroEx == null)
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
                    if (paramentroEx.ParametroId == paramentroEx.ParametroId)
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
