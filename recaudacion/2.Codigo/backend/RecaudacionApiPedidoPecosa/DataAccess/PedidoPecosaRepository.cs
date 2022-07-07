using System.Collections.Generic;
using System.Threading.Tasks;
using RecaudacionApiPedidoPecosa.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System;

namespace RecaudacionApiPedidoPecosa.DataAccess
{
    public class PedidoPecosaRepository : IPedidoPecosaRepository
    {

        private readonly string _connectionString;

        public PedidoPecosaRepository(IConfiguration configuration)
        {

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<PedidoPecosa>> FindAll(string ejecutora, int anioEje, int numeroPecosa)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("USP_MINEDU_PEDIDO_PECOSA_OGA_RECAUDACION", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@EJECUTORA", ejecutora));
                    cmd.Parameters.Add(new SqlParameter("@ANIO_EJE", anioEje));
                    cmd.Parameters.Add(new SqlParameter("@NRO_PECOSA", numeroPecosa));

                    var listPedidoPecosa = new List<PedidoPecosa>();

                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PedidoPecosa PedidoPecosa = new PedidoPecosa();
                            PedidoPecosa.AnioEje = (int)reader["ANO_EJE"];
                            PedidoPecosa.Ejecutora = reader["EJECUTORA"].ToString();
                            PedidoPecosa.TipoBien = reader["TIPO_BIEN"].ToString();
                            PedidoPecosa.NumeroPecosa = (int)reader["NRO_PECOSA"];
                            PedidoPecosa.FechaPecosa = (DateTime)reader["FECHA_PECOSA"];
                            PedidoPecosa.NombreAlmacen = reader["NOMBRE_ALMACEN"].ToString();
                            PedidoPecosa.MotivoPedido = reader["MOTIVO_PEDIDO"].ToString();
                            PedidoPecosa.NombreMarca = reader["NOMBRE_MARCA"].ToString();
                            PedidoPecosa.CodigoItem = reader["CODIGO_ITEM"].ToString();
                            PedidoPecosa.NombreItem = reader["NOMBRE_ITEM"].ToString();
                            PedidoPecosa.NombreUnidad = reader["NOMBRE_UMEDIDA"].ToString();
                            PedidoPecosa.Clasificador = reader["CLASIFICADOR"].ToString();
                            PedidoPecosa.NombreClaficador = reader["NOMBRE_CLASIF"].ToString();
                            PedidoPecosa.CantidadAtendida = (int)reader["CANT_ATENDIDA"];
                            PedidoPecosa.CantidadAprobada = (int)reader["CANT_APROBADA"];
                            PedidoPecosa.PrecioUnitario = (decimal)reader["PRECIO_UNIT"];
                            PedidoPecosa.ValorTotal = (decimal)reader["VALOR_TOTAL"];
                            listPedidoPecosa.Add(PedidoPecosa);
                        }
                        await reader.CloseAsync();
                    }
                    await sql.CloseAsync();

                    return listPedidoPecosa;
                }
            }

        }

        public async Task<PedidoPecosa> FindByEjecutora(string ejecutora, int anioEje, int numeroPecosa)
        {
            var items = await FindAll(ejecutora, anioEje, numeroPecosa);

            if (items.Count > 0)
            {
                var pedidoPecosa = items[0];
                return pedidoPecosa;
            }
            else
            {
                return null;
            }

        }
    }
}
