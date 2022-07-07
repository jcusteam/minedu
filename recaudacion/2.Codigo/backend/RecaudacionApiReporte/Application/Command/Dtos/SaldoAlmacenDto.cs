using System.Collections.Generic;
using RecaudacionApiReporte.Domain;

namespace RecaudacionApiReporte.Application.Command.Dtos
{
    public class SaldoAlmacenDto
    {
        public List<CatalogoBien> CatalogoBienes { get; set; }
    }
}