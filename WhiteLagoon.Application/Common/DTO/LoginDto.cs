using System.ComponentModel.DataAnnotations;

namespace WhiteLagoon.Application.Common.DTO;

public class LoginDto
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    public bool RememberMe { get; set; } 
    
    public string? RedirectUrl { get; set; } 
}