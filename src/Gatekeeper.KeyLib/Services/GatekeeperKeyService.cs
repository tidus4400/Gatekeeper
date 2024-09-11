using Gatekeeper.KeyLib.Features.KeyGeneration;
using Gatekeeper.KeyLib.Features.KeyStorage;
using Gatekeeper.KeyLib.Models;
using Gatekeeper.KeyLib.Errors;

namespace Gatekeeper.KeyLib.Services;

public sealed class GatekeeperKeyService
{
    private readonly IKeyGenerator _keyGenerator;
    private readonly IKeyRepository _keyRepository;

    public GatekeeperKeyService(IKeyGenerator keyGenerator, IKeyRepository keyRepository)
    {
        _keyGenerator = keyGenerator;
        _keyRepository = keyRepository;
    }

    public async Task<GkpKey> GenerateAndStoreKeyForAppIdAsync(string appId, string createdBy)
    {
        string cryptoKey = await _keyGenerator.GenerateCryptoKeyAsync();

        GkpKey gatekeeperKey = new(cryptoKey, appId, createdBy, DateTime.UtcNow);

        await _keyRepository.SaveKeyAsync(gatekeeperKey);

        return gatekeeperKey;
    }

    public async Task<GkpResult<GkpKey, GkpKeyNotFoundError>> GetKeyForAppIdAsync(string appId)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            return new GkpErr<GkpKey, GkpKeyNotFoundError>(new GkpKeyNotFoundError("AppId cannot be null or empty"));
        }

        GkpKey? retrievedKey = await _keyRepository.GetKeyForAppIdAsync(appId);

        if (retrievedKey is null)
        {
            return new GkpErr<GkpKey, GkpKeyNotFoundError>(new GkpKeyNotFoundError($"Key for appId {appId} not found"));
        }

        return new GkpOk<GkpKey, GkpKeyNotFoundError>(retrievedKey);
    }
}
