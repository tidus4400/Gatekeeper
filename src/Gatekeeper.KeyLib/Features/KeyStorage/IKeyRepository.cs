using Gatekeeper.KeyLib.Models;


namespace Gatekeeper.KeyLib.Features.KeyStorage;

public interface IKeyRepository
{
    Task SaveKeyAsync(GatekeeperKey key);
    Task<GatekeeperKey?> GetKeyForAppIdAsync(string appId);
}
