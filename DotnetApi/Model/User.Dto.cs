using System.ComponentModel.DataAnnotations;

namespace DotnetApi.Model;

public partial record UserDto
{
    [MaxLength(50)] public required string FirstName { get; set; }
    [MaxLength(50)] public required string LastName { get; set; }

    public static implicit operator User(UserDto dto)
    {
        return new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };
    }

    public static implicit operator UserDto(User user)
    {
        return new UserDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
    }
}