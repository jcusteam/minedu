using AutoMapper;
using RecaudacionApiGuiaSalidaBien.Application.Query.Dtos;
using RecaudacionApiGuiaSalidaBien.Domain;
using RecaudacionUtils;

namespace RecaudacionApiGuiaSalidaBien.Application.Query.Mapping
{
    public class MappingProfileQuery : Profile
    {
        public MappingProfileQuery()
        {
            CreateMap<GuiaSalidaBien, GuiaSalidaBienDto>();
            CreateMap<GuiaSalidaBienFilterDto, GuiaSalidaBienFilter>();
            CreateMap<GuiaSalidaBienDetalle, GuiaSalidaBienDetalleDto>();
            CreateMap<Pagination<GuiaSalidaBien>, Pagination<GuiaSalidaBienDto>>();
        }
    }
}
