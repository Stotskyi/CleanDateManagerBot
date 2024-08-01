using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Picker.Domain.Entities.CleaningTime;
using Picker.Domain.Entities.Users;

namespace Picker.Application.Data;

public interface IApplicationDbContext
{
    DatabaseFacade Database { get; }
    
    public DbSet<Coliver> Colivers { get; set; }
    public DbSet<CleaningTime> CleaningTimes { get; set; }
    public DbSet<UserState> UserStates { get; set; }
    public DbSet<Cycle> Cycles { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}