
using Microsoft.AspNetCore.Identity;

namespace BlogEngine.Models;

public class Author: IdentityUser
{
    public virtual ICollection<Post> Posts {get; set;}
}