using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecaudacionApiDepositoBanco.Domain;

namespace RecaudacionApiDepositoBanco.DataAccess
{
    public class DepositoBancoDetalleReposiory : IDepositoBancoDetalleReposiory
    {

        private readonly DepositoBancoContext _context;
        private readonly string _connectionString;

        public DepositoBancoDetalleReposiory(DepositoBancoContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<DepositoBancoDetalle> Add(DepositoBancoDetalle depositoBancoDetalle)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_DEPOSITO_BANCOS_DETALLE_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_ID", depositoBancoDetalle.DepositoBancoId));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", depositoBancoDetalle.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_NUMERO_DEPOSITO", depositoBancoDetalle.NumeroDeposito));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_IMPORTE", depositoBancoDetalle.Importe));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_FECHA_DEPOSITO", depositoBancoDetalle.FechaDeposito));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_UTILIZADO", depositoBancoDetalle.Utilizado));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_ESTADO", depositoBancoDetalle.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", depositoBancoDetalle.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", depositoBancoDetalle.FechaCreacion));

                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            depositoBancoDetalle.DepositoBancoDetalleId = (int)reader["DEPOSITO_BANCO_DETALLE_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return await FindById(depositoBancoDetalle.DepositoBancoDetalleId);
        }

        public async Task<IEnumerable<DepositoBancoDetalle>> FindAll(int depositoBancoId)
        {
            var depositoBancoDetalles = await _context.DepositoBancoDetalles.FromSqlRaw<DepositoBancoDetalle>("USP_DEPOSITO_BANCOS_DETALLE_SELALL {0}", depositoBancoId).ToListAsync();
            return depositoBancoDetalles;
        }

        public async Task<DepositoBancoDetalle> FindById(int id)
        {
            var depositoBancoDetalle = (await _context.DepositoBancoDetalles.FromSqlRaw<DepositoBancoDetalle>("USP_DEPOSITO_BANCOS_DETALLE_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return depositoBancoDetalle;
        }

        public async Task<DepositoBancoDetalle> FindByNumeroAndFechaAndCuentaCorriente(string numeroDeposito, DateTime fechaDeposito, int cuentaCorrienteId, int clienteId)
        {
            var depositoBancoDetalle = (await _context.DepositoBancoDetalles.FromSqlRaw<DepositoBancoDetalle>("USP_DEPOSITO_BANCOS_DETALLE_SELBYNUMERO_DEPOSITO {0},{1},{2},{3}",
            numeroDeposito, fechaDeposito, cuentaCorrienteId, clienteId).ToListAsync()).FirstOrDefault();
            return depositoBancoDetalle;
        }

        public async Task Update(DepositoBancoDetalle depositoBancoDetalle)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_DEPOSITO_BANCOS_DETALLE_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_ID", depositoBancoDetalle.DepositoBancoDetalleId));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_ID", depositoBancoDetalle.DepositoBancoId));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", depositoBancoDetalle.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_NUMERO_DEPOSITO", depositoBancoDetalle.NumeroDeposito));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_IMPORTE", depositoBancoDetalle.Importe));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_FECHA_DEPOSITO", depositoBancoDetalle.FechaDeposito));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_SECUENCIA", depositoBancoDetalle.Secuencia ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_TIPO_DOCUMENTO", depositoBancoDetalle.TipoDocumento ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_SERIE_DOCUMENTO", depositoBancoDetalle.SerieDocumento ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_NUMERO_DOCUMENTO", depositoBancoDetalle.NumeroDocumento ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_FECHA_DOCUMENTO", depositoBancoDetalle.FechaDocumento ?? SqlDateTime.Null));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_UTILIZADO", depositoBancoDetalle.Utilizado));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_ESTADO", depositoBancoDetalle.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", depositoBancoDetalle.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", depositoBancoDetalle.FechaModificacion));

                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }
        }
    }
}
