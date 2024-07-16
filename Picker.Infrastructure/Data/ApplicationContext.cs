using Microsoft.EntityFrameworkCore;
using Picker.Infrastructure.Entities;

namespace Picker.Infrastructure.Data;

public sealed class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        //Database.EnsureCreated();
    }
    public DbSet<Coliver> Colivers { get; set; }
    public DbSet<CleaningTime> CleaningTimes { get; set; }
    public DbSet<UserState> UserStates { get; set; }
    public DbSet<Cycle> Cycles { get; set; }
}