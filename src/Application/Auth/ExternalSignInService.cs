using Microsoft.Extensions.Options;
using Template.Application.Abstractions;
using Template.Domain.Entities;

namespace Template.Application.Auth;

public sealed class ExternalSignInService
{
    private readonly IExternalIdTokenValidator _idTokenValidator;
    private readonly IExternalIdentityRepository _externalIdentityRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IOptions<RefreshTokenOptions> _refreshTokenOptions;
    private readonly IUnitOfWork _unitOfWork;

    public ExternalSignInService(
        IExternalIdTokenValidator idTokenValidator,
        IExternalIdentityRepository externalIdentityRepository,
        IUserRepository userRepository,
        IRefreshTokenService refreshTokenService,
        IOptions<RefreshTokenOptions> refreshTokenOptions,
        IUnitOfWork unitOfWork)
    {
        _idTokenValidator = idTokenValidator;
        _externalIdentityRepository = externalIdentityRepository;
        _userRepository = userRepository;
        _refreshTokenService = refreshTokenService;
        _refreshTokenOptions = refreshTokenOptions;
        _unitOfWork = unitOfWork;
    }

    public const string DefaultRole = "template-user";

    public async Task<ExternalSignInResult> SignInAsync(
        ExternalAuthProvider provider,
        string idToken,
        string? expectedNonce,
        CancellationToken cancellationToken)
    {
        var payload = await _idTokenValidator.ValidateAsync(provider, idToken, expectedNonce, cancellationToken);

        var externalIdentity = await _externalIdentityRepository.FindAsync(provider, payload.Subject, cancellationToken);
        if (externalIdentity is not null)
        {
            var existingUser = await _userRepository.GetByIdAsync(externalIdentity.UserId, cancellationToken)
                ?? throw new InvalidOperationException("External identity is linked to a missing user.");

            var existingRefreshToken = await IssueRefreshTokenAsync(existingUser.Id, cancellationToken);
            return new ExternalSignInResult(existingUser, [DefaultRole], payload, existingRefreshToken);
        }

        // Intentionally does not auto-link by email to prevent account takeover scenarios.
        var user = new User(BuildDisplayName(payload), payload.Email);
        _userRepository.Add(user);

        var identity = new ExternalIdentity(user.Id, provider, payload.Subject, payload.Email);
        _externalIdentityRepository.Add(identity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var refreshToken = await IssueRefreshTokenAsync(user.Id, cancellationToken);
        return new ExternalSignInResult(user, [DefaultRole], payload, refreshToken);
    }

    private async Task<RefreshTokenResult?> IssueRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (!_refreshTokenOptions.Value.Enabled)
        {
            return null;
        }

        return await _refreshTokenService.IssueAsync(userId, cancellationToken);
    }

    private static string BuildDisplayName(ExternalIdentityPayload payload)
    {
        if (!string.IsNullOrWhiteSpace(payload.Name))
        {
            return payload.Name;
        }

        if (!string.IsNullOrWhiteSpace(payload.Email))
        {
            var atIndex = payload.Email.IndexOf('@');
            return atIndex > 0 ? payload.Email[..atIndex] : payload.Email;
        }

        return $"user-{payload.Subject}";
    }
}

public sealed record ExternalSignInResult(
    User User,
    IReadOnlyList<string> Roles,
    ExternalIdentityPayload Payload,
    RefreshTokenResult? RefreshToken);
