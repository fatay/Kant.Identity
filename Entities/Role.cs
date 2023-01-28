using Microsoft.AspNetCore.Identity;

namespace Identite.Entities;

public class Role : IdentityRole<long>
{
    public long CreatedBy { get; set; }

    public long UpdatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

}
