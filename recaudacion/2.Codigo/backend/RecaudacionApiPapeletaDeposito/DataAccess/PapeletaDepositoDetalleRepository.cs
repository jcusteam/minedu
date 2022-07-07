using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiPapeletaDeposito.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System;

namespace RecaudacionApiPapeletaDeposito.DataAccess
{
    public class PapeletaDepositoDetalleRepository : IPapeletaDepositoDetalleRepository
    {
        private readonly PapeletaDepositoContext _context;
        private readonly string _connectionString;

        public PapeletaDepositoDetalleRepository(PapeletaDepositoContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<PapeletaDepositoDetalle> Add(PapeletaDepositoDetalle papeletaDepositoDetalle)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_PAPELETA_DEPOSITOS_DETALLE_INS", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_ID", papeletaDepositoDetalle.PapeletaDepositoId));
                    cmd.Parameters.Add(new SqlParameter("@RECIBO_INGRESO_ID", papeletaDepositoDetalle.ReciboIngresoId));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_DETALLE_MONTO", papeletaDepositoDetalle.Monto));
                    cmd.Parameters.Add(new SqlParameter("@PAPELETA_DEPOSITO_DETALLE_ESTADO", papeletaDepositoDetalle.Estado));
                    cmd.Parameters.Add(new SqlParameter("@USUARIO_CREADOR", papeletaDepositoDetalle.UsuarioCreador));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_CREACION", papeletaDepositoDetalle.FechaCreacion));
                    await sql.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            papeletaDepositoDetalle.PapeletaDepositoDetalleId = (int)reader["PAPELETA_DEPOSITO_DETALLE_ID"];
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();
                }
            }
            return papeletaDepositoDetalle;
        }

        public async Task<int> Delete(PapeletaDepositoDetalle papeletaDepositoDetalle)
        {
            _context.PapeletaDepositoDetalles.Remove(papeletaDepositoDetalle);
            return await _context.SaveChangesAsync();

        }

        public async Task<List<PapeletaDepositoDetalle>> FindAll(int id)
        {
            var papeletaDepositoDetalles = await _context.PapeletaDepositoDetalles.FromSqlRaw<PapeletaDepositoDetalle>("USP_PAPELETA_DEPOSITOS_DETALLE_SELALL {0}", id).AsNoTracking().ToListAsync();
            return papeletaDepositoDetalles;
        }

        public async Task<PapeletaDepositoDetalle> FindById(int id)
        {
            var papeletaDepositoDetalle = (await _context.PapeletaDepositoDetalles.FromSqlRaw<PapeletaDepositoDetalle>("USP_PAPELETA_DEPOSITOS_DETALLE_SELBYID {0}", id).ToListAsync()).FirstOrDefault();
            return papeletaDepositoDetalle;
        }

        public async Task<PapeletaDepositoDetalle> FindByReciboIngreso(int reciboIngresoId)
        {
            return await _context.PapeletaDepositoDetalles.Where(x => x.ReciboIngresoId == reciboIngresoId).AsNoTracking().FirstOrDefaultAsync();
        }
    }
}
