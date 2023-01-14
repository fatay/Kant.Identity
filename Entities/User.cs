using Microsoft.AspNetCore.Identity;

namespace Kant.Identity.Entities;

public class User : IdentityUser<long>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public long CreatedBy { get; set; }

    public long UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

}
