using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace easyshifthq.Invitations;

public interface IInvitationRepository : IRepository<Invitation, Guid>
{
    Task<Invitation> FindByEmailAsync(string email);
    Task<Invitation> FindByTokenHashAsync(string tokenHash);
    Task<List<Invitation>> GetPendingInvitationsAsync();
    Task<bool> EmailExistsAsync(string email);
}