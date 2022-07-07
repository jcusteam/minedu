using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ESinadApiExpediente.Domain;

namespace ESinadApiExpediente.DataAccess
{
    public class ExpedienteRepository : IExpedienteRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public ExpedienteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _configuration = configuration;
        }

        public async Task<Expediente> FindByNumeroExpediente(string numeroExpediente)
        {
            var expediente = new Expediente();
            using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GEX_SEL_ExpedienteConsultaListar_MPVC ", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@NumeroExpediente", numeroExpediente));

                        expediente = null;
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var expedienteSinad = new Expediente();
                                expedienteSinad.NumeroExpediente = (string)reader["Expediente"];
                                expedienteSinad.Numero = (int)reader["Numero"];
                                expedienteSinad.FechaIngreso = (DateTime)reader["FechaIngreso"];
                                expedienteSinad.Remite = (string)reader["Remite"];
                                expedienteSinad.OficinaOrigen = (string)reader["OficinaOrigen"];
                                expedienteSinad.Registrador = (string)reader["Registrador"];
                                expedienteSinad.TipoDocumento = (string)reader["TipoDocumento"];
                                expedienteSinad.Anio = (int)reader["Anio"];
                                expedienteSinad.Estado = (string)reader["Estado"];
                                expediente = expedienteSinad;
                            }
                            await reader.CloseAsync();
                        }
                        await sql.CloseAsync();

                    }
                }
            return expediente;
        }
    }
}
