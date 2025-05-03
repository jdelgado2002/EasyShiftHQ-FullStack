using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using easyshifthq.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace easyshifthq.Invitations;

public class EfCoreInvitationRepository : EfCoreRepository<EasyshifthqDbContext, Invitation, Guid>, IInvitationRepository
{
    public EfCoreInvitationRepository(IDbContextProvider<EasyshifthqDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Invitation> FindByEmailAsync(string email)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<Invitation> FindByTokenHashAsync(string tokenHash)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
    }

    public async Task<List<Invitation>> GetPendingInvitationsAsync()
    {
        var dbSet = await GetDbSetAsync();
        var now = DateTime.UtcNow;
        return await dbSet
            .Where(x => x.Status == InvitationStatus.Pending && x.ExpiresAt > now)
            .ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x => x.Email == email);
    }
}