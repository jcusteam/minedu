using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiPapeletaDeposito.DataAccess
{
    public class PapeletaDepositoRepository : IPapeletaDepositoRepository
    {
        private readonly PapeletaDepositoContext _context;
        private readonly string _connectionString;

        public PapeletaDepositoRepository(PapeletaDepositoContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<PapeletaDeposito> Add(PapeletaDeposito papeletaDeposito)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_PAPELETA_DEPOSITOS_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", papeletaDeposito.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@BANCO_ID", papeletaDeposito.BancoId));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", papeletaDeposito.CuentaCorrienteId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", papeletaDeposito.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_NUMERO", papeletaDeposito.Numero));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_FECHA", papeletaDeposito.Fecha));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_MONTO", papeletaDeposito.Monto));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_DESCRIPCION", papeletaDeposito.Descripcion));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_ESTADO", papeletaDeposito.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", papeletaDeposito.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", papeletaDeposito.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            papeletaDeposito.PapeletaDepositoId = (int)reader["PAPELETA_DEPOSITO_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return papeletaDeposito;
        }

        public async Task<int> Count(PapeletaDepositoFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_PAPELETA_DEPOSITOS_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", filter.UnidadEjecutoraId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@BANCO_ID", filter.BancoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", filter.CuentaCorrienteId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_NUMERO", filter.Numero ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_ESTADO", filter.Estado ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_ESTADOS", filter.Estados ?? SqlString.Null));
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

        public async Task Delete(PapeletaDeposito PapeletaDeposito)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_PAPELETA_DEPOSITOS_DEL {0}", PapeletaDeposito.PapeletaDepositoId);
        }

        public async Task<List<PapeletaDeposito>> FindAll()
        {
            var papeletaDepositos = await _context.PapeletaDepositos.FromSqlRaw<PapeletaDeposito>("USP_PAPELETA_DEPOSITOS_SELALL").ToListAsync();
            return papeletaDepositos;
        }

        public async Task<List<PapeletaDeposito>> FindAll(PapeletaDepositoFilter filter)
        {
            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "papeletaDepositoId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_10;

            var papeletaDepositos = await _context.PapeletaDepositos
            .FromSqlRaw<PapeletaDeposito>("USP_PAPELETA_DEPOSITOS_SEL_PAGE {0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
            filter.PageNumber, filter.PageSize, filter.SortColumn, filter.SortOrder,
            filter.UnidadEjecutoraId, filter.BancoId, filter.CuentaCorrienteId, filter.Numero, filter.Estado, filter.Estados).ToListAsync();
            return papeletaDepositos;
        }

        public async Task<PapeletaDeposito> FindById(int id)
        {
            var papeletaDeposito = (await _context.PapeletaDepositos.FromSqlRaw<PapeletaDeposito>("USP_PAPELETA_DEPOSITOS_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return papeletaDeposito;
        }

        public async Task<string> FindCorrelativo(int ejecutoraId, int tipoDocId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_PAPELETA_DEPOSITOS_CORRELATIVO", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ejecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", tipoDocId));
                    string numero = "";
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

        public async Task<Pagination<PapeletaDeposito>> FindPage(PapeletaDepositoFilter filter)
        {
            var pagination = new Pagination<PapeletaDeposito>();
            pagination.Items = await FindAll(filter);
            pagination.Total = await Count(filter);
            return pagination;
        }

        public async Task Update(PapeletaDeposito papeletaDeposito)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_PAPELETA_DEPOSITOS_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_ID", papeletaDeposito.PapeletaDepositoId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", papeletaDeposito.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@BANCO_ID", papeletaDeposito.BancoId));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", papeletaDeposito.CuentaCorrienteId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", papeletaDeposito.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_NUMERO", papeletaDeposito.Numero));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_FECHA", papeletaDeposito.Fecha));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_MONTO", papeletaDeposito.Monto));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_DESCRIPCION", papeletaDeposito.Descripcion));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_ESTADO", papeletaDeposito.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", papeletaDeposito.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", papeletaDeposito.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }
    }
}
