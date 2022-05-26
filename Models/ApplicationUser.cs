using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace bmiWebAPI_3.Models; 

public class ApplicationUser
{
    [JsonProperty(PropertyName = "id")] public string Id { get; set; }

    [Required]
    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }

    [Required]
    [JsonProperty(PropertyName = "password")]
    public string Password { get; set; }
}