using api.Dtos;
using api.EntityFramework;
using AutoMapper;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UpdateUserDto, User>();
    }
}