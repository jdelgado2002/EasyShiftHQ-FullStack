using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Volo.Abp;
using Volo.Abp.Identity;
using easyshifthq.Invitations;

namespace easyshifthq.Web.Pages;

public class AcceptInvitationModel : easyshifthqPageModel
{
    private readonly IInvitationAppService _invitationAppService;
    private readonly IdentityUserManager _userManager;
    private readonly SignInManager<Volo.Abp.Identity.IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;

    [BindProperty(SupportsGet = true)]
    public string Token { get; set; }

    [BindProperty]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [BindProperty]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    public InvitationDto Invitation { get; private set; }
    public string ErrorMessage { get; private set; }
    public bool IsSSOEnabled => _configuration.GetValue<bool>("Authentication:SSO:Enabled");

    public AcceptInvitationModel(
        IInvitationAppService invitationAppService,
        IdentityUserManager userManager,
        SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
        IConfiguration configuration)
    {
        _invitationAppService = invitationAppService;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(Token))
        {
            ErrorMessage = L["InvalidOrExpiredInvitation"];
            return Page();
        }

        try
        {
            Invitation = await _invitationAppService.VerifyTokenAsync(Token);
            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Verify token again to prevent tampering
            var invitation = await _invitationAppService.VerifyTokenAsync(Token);

            // Create user
            var user = new Volo.Abp.Identity.IdentityUser(GuidGenerator.Create(), invitation.Email, invitation.Email, CurrentTenant.Id);

            // For non-SSO setup, set the password
            if (!IsSSOEnabled)
            {
                if (string.IsNullOrEmpty(Password))
                {
                    throw new UserFriendlyException(L["PasswordRequired"]);
                }
                await _userManager.CreateAsync(user, Password);
            }
            else
            {
                await _userManager.CreateAsync(user);
            }

            // Assign role - normalize the role name to match our seeded roles
            var normalizedRole = invitation.Role.ToUpperInvariant();
            var result = await _userManager.AddToRoleAsync(user, normalizedRole);
            if (!result.Succeeded)
            {
                throw new UserFriendlyException(L["RoleAssignmentFailed", invitation.Role]);
            }

            // Accept invitation
            await _invitationAppService.AcceptAsync(invitation.Id);

            // Sign in the user
            await _signInManager.SignInAsync(user, isPersistent: true);

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}