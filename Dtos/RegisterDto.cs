using System.ComponentModel.DataAnnotations;

namespace bmiWebAPI_3.Dtos;

public class RegisterDto

{
    [Required] [EmailAddress] public string Email { get; set; }
    [Required] public string Password { get; set; }
}