using RecaudacionApiGuiaSalidaBien.Application.Command.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RecaudacionApiDepositoBanco.Application.Command.Validation
{
    public static class ValidationForm
    {
        public static bool CatalogoBienId(List<GuiaSalidaBienDetalleFormDto> detalles)
        {
            var items = detalles.Where(x => x.CatalogoBienId < 1).ToList();

            if (items.Count > 0)
            {
                return true;
            }

            return false;
        }
        public static bool IngresoPecosaDetalleId(List<GuiaSalidaBienDetalleFormDto> detalles)
        {

            var items = detalles.Where(x => x.IngresoPecosaDetalleId < 1).ToList();

            if (items.Count > 0)
            {
                return true;
            }

            return false;
        }
        public static bool Cantidad(List<GuiaSalidaBienDetalleFormDto> detalles)
        {

            var items = detalles.Where(x => x.Cantidad < 1).ToList();

            if (items.Count > 0)
            {
                return true;
            }

            return false;
        }
        public static bool SerieFormatoIsNullOrEmpty(List<GuiaSalidaBienDetalleFormDto> detalles)
        {
            var items = detalles.Where(x => String.IsNullOrEmpty(x.SerieFormato)).ToList();

            if (items.Count > 0)
            {
                return true;
            }

            return false;
        }
        public static bool SerieFormatoMaxLength(List<GuiaSalidaBienDetalleFormDto> detalles, int maxLegth)
        {
            var items = detalles.Where(x => x.SerieFormato.Length > maxLegth).ToList();

            if (items.Count > 0)
            {
                return true;
            }

            return false;
        }
        public static bool SerieFormatoPattern(List<GuiaSalidaBienDetalleFormDto> detalles)
        {
            var items = detalles.Where(x => Regex.Replace(x.SerieFormato, @"[A-Za-z0-9]", string.Empty).Length > 0).ToList();

            if (items.Count > 0)
            {
                return true;
            }

            return false;
        }
        public static bool SerieDel(List<GuiaSalidaBienDetalleFormDto> detalles)
        {
            var items = detalles.Where(x => x.SerieDel < 1).ToList();

            if (items.Count > 0)
            {
                return true;
            }

            return false;
        }
        public static bool SerieAl(List<GuiaSalidaBienDetalleFormDto> detalles)
        {
            var items = detalles.Where(x => x.SerieAl < 1).ToList();

            if (items.Count > 0)
            {
                return true;
            }

            return false;
        }
    }
}
