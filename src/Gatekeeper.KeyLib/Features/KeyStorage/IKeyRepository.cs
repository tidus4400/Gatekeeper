using Gatekeeper.KeyLib.Models;

namespace Gatekeeper.KeyLib.Features.KeyStorage;

public interface IKeyRepository
{
    Task SaveKeyAsync(GkpKey key);
    Task<GkpKey?> GetKeyForAppIdAsync(string appId);
}
