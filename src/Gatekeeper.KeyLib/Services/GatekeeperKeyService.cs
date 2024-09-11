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

    public async Task<GatekeeperKey> GenerateAndStoreKeyForAppIdAsync(string appId, string createdBy)
    {
        string cryptoKey = await _keyGenerator.GenerateCryptoKeyAsync();

        GatekeeperKey gatekeeperKey = new(cryptoKey, appId, createdBy, DateTime.UtcNow);

        await _keyRepository.SaveKeyAsync(gatekeeperKey);

        return gatekeeperKey;
    }

    public async Task<GatekeeperKey?> GetKeyForAppIdAsync(string appId)
    {
        return await _keyRepository.GetKeyForAppIdAsync(appId);
    }
}
