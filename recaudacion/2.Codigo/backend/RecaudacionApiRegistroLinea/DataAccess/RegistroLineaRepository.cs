using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiRegistroLinea.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;
using RecaudacionUtils;

namespace RecaudacionApiRegistroLinea.DataAccess
{
    public class RegistroLineaRepository : IRegistroLineaRepository
    {
        private readonly RegistroLineaContext _context;
        private readonly string _connectionString;

        public RegistroLineaRepository(RegistroLineaContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<RegistroLinea> Add(RegistroLinea registroLinea)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_REGISTROS_LINEA_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", registroLinea.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", registroLinea.CuentaCorrienteId));
                    cmd.Parameters.Add(new SqlParameter("@BANCO_ID", registroLinea.BancoId));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_ID", registroLinea.ReciboIngresoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", registroLinea.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", registroLinea.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO", registroLinea.Numero));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_FECHA_REGISTRO", registroLinea.FechaRegistro));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_RECIBO_INGRESO_ID", registroLinea.TipoReciboIngresoId));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO_DEPOSITO", registroLinea.NumeroDeposito));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_IMPORTE_DEPOSITO", registroLinea.ImporteDeposito));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_FECHA_DEPOSITO", registroLinea.FechaDeposito));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_VALIDAR_DEPOSITO", registroLinea.ValidarDeposito));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO_OFICIO", registroLinea.NumeroOficio ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO_COMPROBANTE_PAGO", registroLinea.NumeroComprobantePago ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_EXPEDIENTE_SIAF", registroLinea.ExpedienteSiaf ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO_RESOLUCION", registroLinea.NumeroResolucion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_EXPEDIENTE_ESINAD", registroLinea.ExpedienteESinad ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO_ESINAD", registroLinea.NumeroESinad ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_OBSERVACION", registroLinea.Observacion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_ESTADO", registroLinea.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", registroLinea.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", registroLinea.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            registroLinea.RegistroLineaId = (int)reader["REGISTRO_LINEA_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return await FindById(registroLinea.RegistroLineaId);
        }

        public async Task<int> Count(RegistroLineaFilter filter)
        {

            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_REGISTROS_LINEA_COUNT_PAGE", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", filter.UnidadEjecutoraId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", filter.CuentaCorrienteId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@BANCO_ID", filter.BancoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", filter.ClienteId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO", filter.Numero ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_RECIBO_INGRESO_ID", filter.TipoReciboIngresoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_ESTADO", filter.Estado ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_ESTADOS", filter.Estados ?? SqlString.Null));
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

        public async Task Delete(RegistroLinea RegistroLinea)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC USP_REGISTROS_LINEA_DEL {0}", RegistroLinea.RegistroLineaId);
        }

        public async Task<List<RegistroLinea>> FindAll()
        {
            var registroLineas = await _context.RegistroLineas.FromSqlRaw<RegistroLinea>("USP_REGISTROS_LINEA_SELALL").ToListAsync();
            return registroLineas;
        }

        public async Task<List<RegistroLinea>> FindAll(RegistroLineaFilter filter)
        {

            if (String.IsNullOrEmpty(filter.SortColumn))
                filter.SortColumn = "registroLineaId";

            if (String.IsNullOrEmpty(filter.SortOrder))
                filter.SortOrder = Definition.DESC;

            if (filter.PageNumber <= 0)
                filter.PageNumber = Definition.PAGE_NUMBER;

            if (filter.PageSize <= 0)
                filter.PageSize = Definition.PAGE_SIZE_10;

            var registroLineas = await _context.RegistroLineas
            .FromSqlRaw<RegistroLinea>("USP_REGISTROS_LINEA_SEL_PAGE {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
            filter.PageNumber, filter.PageSize, filter.SortColumn, filter.SortOrder,
            filter.UnidadEjecutoraId, filter.CuentaCorrienteId, filter.BancoId, filter.ClienteId,
            filter.Numero, filter.TipoReciboIngresoId, filter.Estado, filter.Estados).ToListAsync();
            return registroLineas;
        }

        public async Task<RegistroLinea> FindById(int id)
        {
            var RegistroLinea = (await _context.RegistroLineas.FromSqlRaw<RegistroLinea>("USP_REGISTROS_LINEA_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return RegistroLinea;
        }

        public async Task<string> FindCorrelativo(int ejecutoraId, int tipoDocId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_REGISTROS_LINEA_CORRELATIVO", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", ejecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", tipoDocId));
                    var numero = "";
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            numero = reader["NUMERO"].ToString();
                        }
                    }

                    return numero;
                }
            }
        }

        public async Task<Pagination<RegistroLinea>> FindPage(RegistroLineaFilter filter)
        {
            var pagination = new Pagination<RegistroLinea>();
            pagination.Items = await FindAll(filter);
            pagination.Total = await Count(filter);
            return pagination;
        }

        public async Task Update(RegistroLinea registroLinea)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_REGISTROS_LINEA_UPD", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_ID", registroLinea.RegistroLineaId));
                    cmd.Parameters.Add(new SqlParameter("@UNIDAD_EJECUTORA_ID", registroLinea.UnidadEjecutoraId));
                    cmd.Parameters.Add(new SqlParameter("@CUENTA_CORRIENTE_ID", registroLinea.CuentaCorrienteId));
                    cmd.Parameters.Add(new SqlParameter("@BANCO_ID", registroLinea.BancoId));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_ID", registroLinea.ReciboIngresoId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@CLIENTE_ID", registroLinea.ClienteId));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_DOCUMENTO_ID", registroLinea.TipoDocumentoId));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO", registroLinea.Numero));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_FECHA_REGISTRO", registroLinea.FechaRegistro));
                    cmd.Parameters.Add(new SqlParameter("@TIPO_RECIBO_INGRESO_ID", registroLinea.TipoReciboIngresoId));
                    cmd.Parameters.Add(new SqlParameter("@DEPOSITO_BANCO_DETALLE_ID", registroLinea.DepositoBancoDetalleId ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO_DEPOSITO", registroLinea.NumeroDeposito));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_IMPORTE_DEPOSITO", registroLinea.ImporteDeposito));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_FECHA_DEPOSITO", registroLinea.FechaDeposito));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_VALIDAR_DEPOSITO", registroLinea.ValidarDeposito));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO_OFICIO", registroLinea.NumeroOficio ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO_COMPROBANTE_PAGO", registroLinea.NumeroComprobantePago ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_EXPEDIENTE_SIAF", registroLinea.ExpedienteSiaf ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO_RESOLUCION", registroLinea.NumeroResolucion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_EXPEDIENTE_ESINAD", registroLinea.ExpedienteESinad ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_NUMERO_ESINAD", registroLinea.NumeroESinad ?? SqlInt32.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_OBSERVACION", registroLinea.Observacion ?? SqlString.Null));
                    cmd.Parameters.Add(new SqlParameter("@REGISTRO_LINEA_ESTADO", registroLinea.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_MODIFICADOR", registroLinea.UsuarioModificador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_MODIFICACION", registroLinea.FechaModificacion));
                    await sql.OpenAsync();
                    await cmd.ExecuteReaderAsync();
                    await sql.CloseAsync();
                }
            }

        }

        public async Task<bool> VerifyExists(int tipo, RegistroLinea registroLinea)
        {
            var exists = false;

            var registroLineaEx = await _context.RegistroLineas.Where(x =>
                x.ClienteId == registroLinea.ClienteId &&
                x.NumeroDeposito.Trim() == registroLinea.NumeroDeposito.Trim() &&
                x.FechaDeposito == registroLinea.FechaDeposito &&
                x.ImporteDeposito == x.ImporteDeposito).AsNoTracking().FirstOrDefaultAsync();

            if (registroLineaEx == null)
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
                    if (registroLineaEx.RegistroLineaId == registroLinea.RegistroLineaId)
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

        public async Task<bool> VerifyExistsESinad(string expedienteESinad)
        {
            var registroLinea = await _context.RegistroLineas.FirstOrDefaultAsync(x => x.ExpedienteESinad.Trim() == expedienteESinad.Trim());

            if (registroLinea == null)
            {
                return false;
            }

            return true;
        }
    }
}
