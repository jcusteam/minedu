using System;
using RecaudacionApiGuiaSalidaBien.Domain;

namespace RecaudacionApiGuiaSalidaBien.Application.Query.Dtos
{
    public class GuiaSalidaBienDetalleDto
    {
         public int GuiaSalidaBienDetalleId { get; set; }
        public int GuiaSalidaBienId { get; set; }
        public CatalogoBien CatalogoBien { get; set; }
        public int CatalogoBienId { get; set; }
        public int IngresoPecosaDetalleId { get; set; }
        public int Cantidad { get; set; }
        public string SerieFormato { get; set; }
        public int SerieDel { get; set; }
        public int SerieAl { get; set; }
        public string Estado { get; set; }
    }
}