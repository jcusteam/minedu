using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiDepositoBanco.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiDepositoBanco.DataAccess
{
    public class DepositoBancoRepository : IDepositoBancoRepository
    {
        private readonly DepositoBancoContext _context;
        private readonly string _connectionString;

        public DepositoBancoRepository(DepositoBancoContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<DepositoBanco> Add(DepositoBanco depositoBanco)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_DEPOSITO_BANCOS_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", depositoBanco.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@BANCO_ID", depositoBanco.BancoId));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", depositoBanco.CuentaCorrienteId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", depositoBanco.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_NUMERO", depositoBanco.Numero));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_IMPORTE", depositoBanco.Importe));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_FECHA_DEPOSITO", depositoBanco.FechaDeposito));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_FECHA_REGISTRO", depositoBanco.FechaRegistro));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_NOMBRE_ARCHIVO", depositoBanco.NombreArchivo));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_CANTIDAD", depositoBanco.Cantidad));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_ESTADO", depositoBanco.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", depositoBanco.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", depositoBanco.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            depositoBanco.DepositoBancoId = (int)reader["DEPOSITO_BANCO_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return await FindById(depositoBanco.DepositoBancoId);
        }

        public async Task<int> Count(DepositoBancoFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_DEPOSITO_BANCOS_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", filter.UnidadEjecutoraId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@BANCO_ID", filter.BancoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", filter.CuentaCorrienteId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_NUMERO", filter.Numero ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_NOMBRE_ARCHIVO", filter.NombreArchivo ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_ESTADO", filter.Estado ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_ESTADOS", filter.Estados ?? SqlString.Null));
                    var count = 0;
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

        public async Task Delete(DepositoBanco DepositoBanco)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_DEPOSITO_BANCOS_DEL {0}", DepositoBanco.DepositoBancoId);
        }

        public async Task<bool> VerifyExistsNombreArchivo(string nombreArchivo)
        {
            var depositoBanco = (await _context.DepositoBancos.FromSqlRaw<DepositoBanco>("USP_DEPOSITO_BANCOS_SELBYNOMBREARCHIVO {0}", nombreArchivo).ToListAsync()).FirstOrDefault();

            if (depositoBanco == null)
                return false;

            return true;
        }

        public async Task<List<DepositoBanco>> FindAll()
        {
            var DepositoBancos = await _context.DepositoBancos.FromSqlRaw<DepositoBanco>("USP_DEPOSITO_BANCOS_SELALL").ToListAsync();
            return DepositoBancos;
        }

        public async Task<List<DepositoBanco>> FindAll(DepositoBancoFilter filter)
        {
            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "depositoBancoId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_10;

            var depositoBancos = await _context
                .DepositoBancos.FromSqlRaw<DepositoBanco>("USP_DEPOSITO_BANCOS_SEL_PAGE {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                filter.PageNumber, filter.PageSize, filter.SortColumn, filter.SortOrder,
                filter.UnidadEjecutoraId, filter.BancoId, filter.CuentaCorrienteId,
                filter.Numero, filter.NombreArchivo, filter.Estado, filter.Estados).ToListAsync();

            return depositoBancos;
        }
        public async Task<Pagination<DepositoBanco>> FindPage(DepositoBancoFilter filter)
        {
            var pagination = new Pagination<DepositoBanco>();
            pagination.Items = await FindAll(filter);
            pagination.Total = await Count(filter);
            return pagination;
        }

        public async Task<DepositoBanco> FindById(int id)
        {
            var depositoBanco = (await _context.DepositoBancos.FromSqlRaw<DepositoBanco>("USP_DEPOSITO_BANCOS_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return depositoBanco;
        }

        public async Task<string> FindCorrelativo(int ejecutoraId, int tipoDocumentoId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_DEPOSITO_BANCOS_CORRELATIVO", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ejecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", tipoDocumentoId));

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

        public async Task Update(DepositoBanco depositoBanco)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_DEPOSITO_BANCOS_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_ID", depositoBanco.DepositoBancoId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", depositoBanco.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@BANCO_ID", depositoBanco.BancoId));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", depositoBanco.CuentaCorrienteId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", depositoBanco.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_NUMERO", depositoBanco.Numero));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_IMPORTE", depositoBanco.Importe));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_FECHA_DEPOSITO", depositoBanco.FechaDeposito));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_FECHA_REGISTRO", depositoBanco.FechaRegistro));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_NOMBRE_ARCHIVO", depositoBanco.NombreArchivo));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_CANTIDAD", depositoBanco.Cantidad));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_ESTADO", depositoBanco.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", depositoBanco.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", depositoBanco.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }

    }
}
