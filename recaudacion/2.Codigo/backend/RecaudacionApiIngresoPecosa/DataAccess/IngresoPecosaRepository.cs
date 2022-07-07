using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiIngresoPecosa.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiIngresoPecosa.DataAccess
{
    public class IngresoPecosaRepository : IIngresoPecosaRepository
    {
        private readonly IngresoPecosaContext _context;
        private readonly string _connectionString;

        public IngresoPecosaRepository(IngresoPecosaContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IngresoPecosa> Add(IngresoPecosa ingresoPecosa)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_INGRESO_PECOSA_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ingresoPecosa.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", ingresoPecosa.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_ANIO", ingresoPecosa.AnioPecosa));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_TIPO_BIEN", ingresoPecosa.TipoBien));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_NUMERO", ingresoPecosa.NumeroPecosa));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_FECHA", ingresoPecosa.FechaPecosa));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_NOMBRE_ALMACEN", ingresoPecosa.NombreAlmacen));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_MOTIVO_PEDIDO", ingresoPecosa.MotivoPedido));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_FECHA_REGISTRO", ingresoPecosa.FechaRegistro));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_ESTADO", ingresoPecosa.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", ingresoPecosa.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", ingresoPecosa.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ingresoPecosa.IngresoPecosaId = (int)reader["INGRESO_PECOSA_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return ingresoPecosa;
        }

        public async Task<int> Count(IngresoPecosaFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_INGRESO_PECOSA_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", filter.UnidadEjecutoraId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_ANIO", filter.AnioPecosa ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_TIPO_BIEN", filter.TipoBien ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_NUMERO", filter.NumeroPecosa ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_FECHA_INICIO", filter.FechaInicio ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_FECHA_FIN", filter.FechaFin ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_ESTADO", filter.Estado ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_ESTADOS", filter.Estados ?? SqlString.Null));
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

        public async Task Delete(IngresoPecosa ingresoPecosa)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_INGRESO_PECOSA_DEL {0}", ingresoPecosa.IngresoPecosaId);
        }

        public async Task<List<IngresoPecosa>> FindAll()
        {
            var ingresoPecosas = await _context.IngresoPecosas.FromSqlRaw<IngresoPecosa>("USP_INGRESO_PECOSA_SELALL").ToListAsync();
            return ingresoPecosas;
        }

        public async Task<List<IngresoPecosa>> FindAll(IngresoPecosaFilter filter)
        {
            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "ingresoPecosaId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_10;

            var ingresoPecosas = await _context.IngresoPecosas
            .FromSqlRaw<IngresoPecosa>("USP_INGRESO_PECOSA_SEL_PAGE {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
            filter.PageNumber, filter.PageSize, filter.SortColumn, filter.SortOrder,
            filter.UnidadEjecutoraId, filter.AnioPecosa, filter.TipoBien, filter.NumeroPecosa,
            filter.FechaInicio, filter.FechaFin, filter.Estado, filter.Estados).ToListAsync();
            return ingresoPecosas;
        }

        public async Task<IngresoPecosa> FindById(int id)
        {
            var ingresoPecosa = (await _context.IngresoPecosas.FromSqlRaw<IngresoPecosa>("USP_INGRESO_PECOSA_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return ingresoPecosa;
        }

        public async Task<Pagination<IngresoPecosa>> FindPage(IngresoPecosaFilter filter)
        {
            var pagination = new Pagination<IngresoPecosa>();
            pagination.Items = await FindAll(filter);
            pagination.Total = await Count(filter);
            return pagination;
        }

        public async Task Update(IngresoPecosa ingresoPecosa)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_INGRESO_PECOSA_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_ID", ingresoPecosa.IngresoPecosaId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ingresoPecosa.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", ingresoPecosa.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_ANIO", ingresoPecosa.AnioPecosa));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_TIPO_BIEN", ingresoPecosa.TipoBien));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_NUMERO", ingresoPecosa.NumeroPecosa));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_FECHA", ingresoPecosa.FechaPecosa));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_NOMBRE_ALMACEN", ingresoPecosa.NombreAlmacen));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_MOTIVO_PEDIDO", ingresoPecosa.MotivoPedido));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_FECHA_REGISTRO", ingresoPecosa.FechaRegistro));
                    cmd.Parameters.Add(new SqlParameter("@INGRESO_PECOSA_ESTADO", ingresoPecosa.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", ingresoPecosa.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", ingresoPecosa.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }

        public async Task<bool> VerifyExists(int tipo, IngresoPecosa ingresoPecosa)
        {
            var ingresoPecosaEx = await _context.IngresoPecosas.Where(x =>
                x.UnidadEjecutoraId == ingresoPecosa.UnidadEjecutoraId &&
                x.NumeroPecosa == ingresoPecosa.NumeroPecosa &&
                x.AnioPecosa == ingresoPecosa.AnioPecosa).AsNoTracking().FirstOrDefaultAsync();

            if (ingresoPecosaEx == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
