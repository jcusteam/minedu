using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiGuiaSalidaBien.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiGuiaSalidaBien.DataAccess
{
    public class GuiaSalidaBienRepository : IGuiaSalidaBienRepository
    {
        private readonly GuiaSalidaBienContext _context;
        private readonly string _connectionString;

        public GuiaSalidaBienRepository(GuiaSalidaBienContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<GuiaSalidaBien> Add(GuiaSalidaBien guiaSalidaBien)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_GUIA_SALIDA_BIENES_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", guiaSalidaBien.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", guiaSalidaBien.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_NUMERO", guiaSalidaBien.Numero));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_FECHA_REGISTRO", guiaSalidaBien.FechaRegistro));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_JUSTIFICACION", guiaSalidaBien.Justificacion));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_ESTADO", guiaSalidaBien.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", guiaSalidaBien.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", guiaSalidaBien.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            guiaSalidaBien.GuiaSalidaBienId = (int)reader["GUIA_SALIDA_BIEN_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return guiaSalidaBien;
        }

        public async Task<int> Count(GuiaSalidaBienFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_GUIA_SALIDA_BIENES_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", filter.UnidadEjecutoraId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_NUMERO", filter.Numero ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_FECHA_INICIO", filter.FechaInicio ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_FECHA_FIN", filter.FechaFin ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_ESTADO", filter.Estado ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_ESTADOS", filter.Estados ?? SqlString.Null));
                    var count = 0;
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            count = (int)reader["TOTAL"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();

                    return count;
                }
            }
        }

        public async Task Delete(GuiaSalidaBien GuiaSalidaBien)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_GUIA_SALIDA_BIENES_DEL {0}", GuiaSalidaBien.GuiaSalidaBienId);
        }

        public async Task<List<GuiaSalidaBien>> FindAll()
        {
            var guiaSalidaBienes = await _context.GuiaSalidaBienes.FromSqlRaw<GuiaSalidaBien>("USP_GUIA_SALIDA_BIENES_SELALL").ToListAsync();
            return guiaSalidaBienes;
        }

        public async Task<List<GuiaSalidaBien>> FindAll(GuiaSalidaBienFilter filter)
        {
            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "guiaSalidaBienId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_10;

            var guiaSalidaBienes = await _context.GuiaSalidaBienes
            .FromSqlRaw<GuiaSalidaBien>("USP_GUIA_SALIDA_BIENES_SEL_PAGE {0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
            filter.PageNumber, filter.PageSize, filter.SortColumn, filter.SortOrder,
            filter.UnidadEjecutoraId, filter.Numero, filter.FechaInicio, filter.FechaFin, filter.Estado, filter.Estados).ToListAsync();
            return guiaSalidaBienes;
        }

        public async Task<GuiaSalidaBien> FindById(int id)
        {
            var guiaSalidaBien = (await _context.GuiaSalidaBienes.FromSqlRaw<GuiaSalidaBien>("USP_GUIA_SALIDA_BIENES_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return guiaSalidaBien;
        }

        public async Task<string> FindCorrelativo(int ejecutoraId, int tipoDocId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_GUIA_SALIDA_BIENES_CORRELATIVO", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ejecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", tipoDocId));
                    await sql.OpenAsync();
                    var correlativo = "";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                            correlativo = reader["NUMERO"].ToString();
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();

                    return correlativo;
                }
            }
        }

        public async Task<Pagination<GuiaSalidaBien>> FindPage(GuiaSalidaBienFilter filter)
        {
            var pagination = new Pagination<GuiaSalidaBien>();
            pagination.Items = await FindAll(filter);
            pagination.Total = await Count(filter);
            return pagination;
        }

        public async Task Update(GuiaSalidaBien guiaSalidaBien)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_GUIA_SALIDA_BIENES_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_ID", guiaSalidaBien.GuiaSalidaBienId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", guiaSalidaBien.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", guiaSalidaBien.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_NUMERO", guiaSalidaBien.Numero));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_FECHA_REGISTRO", guiaSalidaBien.FechaRegistro));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_JUSTIFICACION", guiaSalidaBien.Justificacion));
                    cmd.Parameters.Add(new SqlParameter("@GUIA_SALIDA_BIEN_ESTADO", guiaSalidaBien.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", guiaSalidaBien.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", guiaSalidaBien.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }

    }
}
