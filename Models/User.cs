using Microsoft.AspNetCore.Identity;

namespace UsersMS.Models;

public class User : IdentityUser
{
    public string Secret { get; set; }
}