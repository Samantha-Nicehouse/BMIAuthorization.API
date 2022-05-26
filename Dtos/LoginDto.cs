using System.ComponentModel.DataAnnotations;

namespace bmiWebAPI_3.Dtos;

public class LoginDto
{
    [Required] public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}