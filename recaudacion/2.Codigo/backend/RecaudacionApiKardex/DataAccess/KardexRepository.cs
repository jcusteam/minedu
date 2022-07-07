using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiKardex.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;

namespace RecaudacionApiKardex.DataAccess
{
    public class KardexRepository : IKardexRepository
    {

        private readonly string _connectionString;

        public KardexRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Kardex>> FindAll(int catalogoBienId)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_KARDEX_SELALL", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CATALOGO_BIEN_ID", catalogoBienId));

                    var kardexs = new List<Kardex>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Kardex kardex = new Kardex();
                            kardex.KardexId = (int)reader["KARDEX_ID"];
                            kardex.Documento = (string)reader["KARDEX_DOCUMENTO"];
                            kardex.AnioPecosa = (int)reader["KARDEX_ANIO_PECOSA"];
                            kardex.NumeroPecosa = (int)reader["KARDEX_NUMERO_PECOSA"];
                            kardex.Fecha = (DateTime)reader["KARDEX_FECHA"];
                            kardex.EntradaDocumento = (string)reader["KARDEX_ENTRADA_DOCUMENTO"];
                            kardex.EntradaDel = (int)reader["KARDEX_ENTRADA_DEL"];
                            kardex.EntradaAl = (int)reader["KARDEX_ENTRADA_AL"];
                            kardex.EntradaTotal = (int)reader["KARDEX_ENTRADA_TOTAL"];
                            kardex.SalidaDocumento = (string)reader["KARDEX_SALIDA_DOCUMENTO"];
                            kardex.SalidaDocumentoNumero = (string)reader["KARDEX_SALIDA_DOCUMENTO_NUMERO"];
                            kardex.SalidaDel = (int)reader["KARDEX_SALIDA_DEL"];
                            kardex.SalidaAl = (int)reader["KARDEX_SALIDA_AL"];
                            kardex.SalidaTotal = (int)reader["KARDEX_SALIDA_TOTAL"];
                            kardex.Saldo = (int)reader["KARDEX_SALDO"];
                            kardexs.Add(kardex);
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();

                    foreach (var item in kardexs)
                    {
                        if (item.KardexId > 1)
                        {
                            var kardex = kardexs.Where(x => x.KardexId == (item.KardexId - 1)).FirstOrDefault();
                            var saldo = (item.EntradaTotal + (kardex.Saldo)) - item.SalidaTotal;
                            item.Saldo = saldo;
                        }

                    }

                    return kardexs;
                }
            }

        }

        public async Task<Kardex> FindById(int id, int catalogoBienId)
        {
            var kardexs = await FindAll(catalogoBienId);
            var kardex = kardexs.Find(x => x.KardexId == id);

            return kardex;
        }
    }
}
