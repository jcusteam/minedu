using AutoMapper;
using RecaudacionApiGuiaSalidaBien.Application.Command.Dtos;
using RecaudacionApiGuiaSalidaBien.Domain;

namespace RecaudacionApiGuiaSalidaBien.Application.Command.Mapping
{
    public class MappingProfileCommand : Profile
    {
        public MappingProfileCommand()
        {
            CreateMap<GuiaSalidaBienFormDto, GuiaSalidaBien>();
             CreateMap<GuiaSalidaBienDetalleFormDto, GuiaSalidaBienDetalle>();
            CreateMap<GuiaSalidaBien, GuiaSalidaBienFormDto>();
        }
    }
}