using System;

namespace RecaudacionApiGuiaSalidaBien.Application.Command.Dtos
{
    public class GuiaSalidaBienDetalleFormDto
    {
        public int GuiaSalidaBienDetalleId { get; set; }
        public int GuiaSalidaBienId { get; set; }
        public int CatalogoBienId { get; set; }
        public int IngresoPecosaDetalleId { get; set; }
        public int Cantidad { get; set; }
        public string SerieFormato { get; set; }
        public int SerieDel { get; set; }
        public int SerieAl { get; set; }
        public string Estado { get; set; }
        public string UsuarioCreador { get; set; }
        public string UsuarioModificador { get; set; }
    }
}