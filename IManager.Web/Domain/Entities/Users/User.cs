using Microsoft.AspNetCore.Identity;

namespace IManager.Web.Domain.Entities.Users;

public class User : IdentityUser<Guid>
{
    public UserProfile? UserProfile { get; set; }
}
