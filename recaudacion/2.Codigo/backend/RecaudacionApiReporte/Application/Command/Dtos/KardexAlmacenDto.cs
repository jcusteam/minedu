using System.Collections.Generic;
using RecaudacionApiReporte.Domain;

namespace RecaudacionApiReporte.Application.Command.Dtos
{
    public class KardexAlmacenDto
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public List<Kardex> Kardexs { get; set; }
    }
}