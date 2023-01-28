using Identite.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identite.Context;

public class UserDbContext : IdentityDbContext<User,Role,long>
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) => base.OnModelCreating(builder);



}
