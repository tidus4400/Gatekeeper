using Gatekeeper.KeyLib.Features.KeyGeneration;
using Gatekeeper.KeyLib.Features.KeyStorage;
using Gatekeeper.KeyLib.Models;

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

    public async Task<(GkpKey?, Exception?)> GetKeyForAppIdAsync(string appId)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            return (null, new ArgumentNullException(nameof(appId)));
        }

        GkpKey? retrievedKey = await _keyRepository.GetKeyForAppIdAsync(appId);

        if (retrievedKey is null)
        {
            return (null, new KeyNotFoundException($"Key not found for appId: {appId}"));
        }

        return (retrievedKey, null);
    }
}
