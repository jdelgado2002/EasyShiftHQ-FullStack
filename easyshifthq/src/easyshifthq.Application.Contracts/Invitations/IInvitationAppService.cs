using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace easyshifthq.Invitations;

public interface IInvitationAppService : IApplicationService
{
    Task<InvitationDto> CreateAsync(CreateInvitationDto input);
    Task<List<InvitationDto>> CreateBulkAsync(BulkInvitationDto input);
    Task<List<InvitationDto>> GetPendingAsync();
    Task<InvitationDto> AcceptAsync(Guid id);
    Task RevokeAsync(Guid id);
    Task ResendAsync(Guid id);
    Task<InvitationDto> VerifyTokenAsync(string token);
}