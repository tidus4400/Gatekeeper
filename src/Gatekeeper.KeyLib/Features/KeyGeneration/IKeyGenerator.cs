namespace Gatekeeper.KeyLib.Features.KeyGeneration;

public interface IKeyGenerator
{
    Task<string> GenerateCryptoKeyAsync();
}
