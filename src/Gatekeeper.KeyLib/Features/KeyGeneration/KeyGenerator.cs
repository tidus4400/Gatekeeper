using System.Security.Cryptography;

namespace Gatekeeper.KeyLib.Features.KeyGeneration;

public sealed class KeyGenerator : IKeyGenerator
{
    public Task<string> GenerateCryptoKeyAsync()
    {
        byte[] key = new byte[256];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);

        return Task.FromResult(Convert.ToBase64String(key));
    }

}
