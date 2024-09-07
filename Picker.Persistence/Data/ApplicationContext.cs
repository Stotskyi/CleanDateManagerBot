using Microsoft.EntityFrameworkCore;
using Picker.Application.Data;
using Picker.Domain.Entities.CleaningTime;
using Picker.Domain.Entities.Users;

namespace Picker.Persistence.Data;

public sealed  class ApplicationContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) 
        : base(options)
    {
       
    }
    
    public DbSet<Coliver> Colivers { get; set; }
    public DbSet<CleaningTime> CleaningTimes { get; set; }
    public DbSet<UserState> UserStates { get; set; }
    public DbSet<Cycle> Cycles { get; set; }
    public DbSet<User> Users { get; set; }
}