using Riok.Mapperly.Abstractions;
using UserService.DAL.Model;
using UserService.Dto;

namespace UserService.Mapping;

[Mapper]
public static partial class UserMapper
{
    public static partial User ToDbModel(this CreateUserDto user);

    public static partial UserDto ToDto(this User user);

    [MapperIgnoreSource(nameof(UpdateUserDto.Id))]
    public static partial void UpdateDbModel(this UpdateUserDto userDto, User user);
}