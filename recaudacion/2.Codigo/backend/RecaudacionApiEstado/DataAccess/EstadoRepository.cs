using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiEstado.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiEstado.DataAccess
{
    public class EstadoRepository : IEstadoRepository
    {
        private readonly EstadoContext _context;
        private readonly string _connectionString;

        public EstadoRepository(EstadoContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<Estado> Add(Estado estado)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_ESTADOS_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", estado.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@ESTADO_NUMERO", estado.Numero));
                    cmd.Parameters.Add(new SqlParameter("@ESTADO_ORDEN", estado.Orden));
                    cmd.Parameters.Add(new SqlParameter("@ESTADO_NOMBRE", estado.Nombre));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", estado.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", estado.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            estado.EstadoId = (int)reader["ESTADO_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return estado;
        }

        public async Task<int> Count(EstadoFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_ESTADOS_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", filter.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@ESTADO_NOMBRE", filter.Nombre ?? SqlString.Null));
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

        public async Task Delete(Estado estado)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_ESTADOS_DEL {0}", estado.EstadoId);
        }

        public async Task<List<Estado>> FindAll()
        {
            var estados = await _context.Estados.FromSqlRaw<Estado>("USP_ESTADOS_SELALL").ToListAsync();
            return estados;
        }

        public async Task<List<Estado>> FindAll(EstadoFilter filter)
        {
            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "estadoId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_5;

            var estados = await _context.Estados
            .FromSqlRaw<Estado>("USP_ESTADOS_SEL_PAGE {0},{1},{2},{3},{4},{5}",
            filter.PageNumber, filter.PageSize, filter.SortColumn, filter.SortOrder,
            filter.TipoDocumentoId, filter.Nombre).ToListAsync();
            return estados;
        }

        public async Task<Estado> FindById(int id)
        {
            var estado = (await _context.Estados.FromSqlRaw<Estado>("USP_ESTADOS_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return estado;
        }

        public async Task<List<Estado>> FindByTipoDoc(int tipoDocumentoId)
        {
            var estados = await _context.Estados.FromSqlRaw<Estado>("USP_ESTADOS_SELBYTIPO_DOCUMENTO {0}", tipoDocumentoId).ToListAsync();
            return estados;
        }

        public async Task<Estado> FindByTipoDocAndNumero(int tipoDocumentoId, int numero)
        {
            var estado = (await _context.Estados.FromSqlRaw<Estado>("USP_ESTADOS_SELBYTIPO_DOCUMENTO_NUMERO {0},{1}", tipoDocumentoId, numero).ToListAsync()).FirstOrDefault();
            return estado;
        }

        public async Task<Pagination<Estado>> FindPage(EstadoFilter filter)
        {
            var pagination = new Pagination<Estado>();
            pagination.Items = await FindAll(filter);
            pagination.Total = await Count(filter);
            return pagination;
        }

        public async Task Update(Estado estado)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_ESTADOS_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ESTADO_ID", estado.EstadoId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", estado.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@ESTADO_NUMERO", estado.Numero));
                    cmd.Parameters.Add(new SqlParameter("@ESTADO_ORDEN", estado.Orden));
                    cmd.Parameters.Add(new SqlParameter("@ESTADO_NOMBRE", estado.Nombre));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", estado.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", estado.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }

        public async Task<bool> VerifyExists(int tipo, Estado estado)
        {
            var exists = false;

            var estadoEx = await _context.Estados.Where(x =>
                x.TipoDocumentoId == estado.TipoDocumentoId &&
                (x.Numero == estado.Numero ||
                x.Orden == estado.Orden ||
                x.Nombre.ToUpper().Trim() == estado.Nombre.ToUpper().Trim())).FirstOrDefaultAsync();

            if (estadoEx == null)
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
                    if (estadoEx.EstadoId == estado.EstadoId)
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
