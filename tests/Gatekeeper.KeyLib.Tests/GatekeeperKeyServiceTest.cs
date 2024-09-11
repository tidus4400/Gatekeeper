using Gatekeeper.KeyLib.Features.KeyGeneration;
using Gatekeeper.KeyLib.Features.KeyStorage;
using Gatekeeper.KeyLib.Models;
using Gatekeeper.KeyLib.Errors;
using Gatekeeper.KeyLib.Services;


namespace Gatekeeper.KeyLib.Tests.Services;

public class GatekeeperKeyServiceTest
{
    private readonly IKeyGenerator _keyGenerator;
    private readonly IKeyRepository _inMemoryKeyRepository;
    private readonly GatekeeperKeyService _service;

    public GatekeeperKeyServiceTest()
    {
        _keyGenerator = new KeyGenerator();
        _inMemoryKeyRepository = new InMemoryKeyRepository();
        _service = new GatekeeperKeyService(_keyGenerator, _inMemoryKeyRepository);
    }

    [Fact]
    public async Task GenerateAndStoreKeyForAppIdAsync_ShouldGenerateAndStoreKey()
    {
        // Arrange
        string appId = "testAppId";
        string createdBy = "testUser";

        // Act
        GkpKey result = await _service.GenerateAndStoreKeyForAppIdAsync(appId, createdBy);

        // Assert
        Assert.Equal(appId, result.AppId);
        Assert.Equal(createdBy, result.CreatedBy);
    }

    [Fact]
    public async Task GetKeyForAppIdAsync_ShouldReturnOk_IfKeyExistsInRepo()
    {
        // Arrange
        string appId = "testAppId";
        GkpKey expectedKey = new("cryptoKey", appId, "createdBy", DateTime.UtcNow);

        await _inMemoryKeyRepository.SaveKeyAsync(expectedKey);

        // Act
        GkpResult<GkpKey, GkpKeyNotFoundError> result = await _service.GetKeyForAppIdAsync(appId);

        GkpKey? gkpKey = result switch
        {
            GkpOk<GkpKey, GkpKeyNotFoundError> => (result as GkpOk<GkpKey, GkpKeyNotFoundError>)!.Value,
            GkpErr<GkpKey, GkpKeyNotFoundError> => null,
            _ => null
        };

        // Assert
        Assert.IsType<GkpOk<GkpKey, GkpKeyNotFoundError>>(result);
        Assert.Equal(expectedKey, gkpKey);
    }

    [Fact]
    public async Task GetKeyForAppIdAsync_ShouldReturnError_IfKeyNotExistsInRepo()
    {
        // Arrange
        string appId = "testAppId";
        GkpKey expectedKey = new("cryptoKey", appId, "createdBy", DateTime.UtcNow);

        await _inMemoryKeyRepository.SaveKeyAsync(expectedKey);

        // Act
        GkpResult<GkpKey, GkpKeyNotFoundError> result = await _service.GetKeyForAppIdAsync("wrongAppId");

        GkpKey? gkpKey = result switch
        {
            GkpOk<GkpKey, GkpKeyNotFoundError> => (result as GkpOk<GkpKey, GkpKeyNotFoundError>)!.Value,
            GkpErr<GkpKey, GkpKeyNotFoundError> => null,
            _ => null
        };

        // Assert
        Assert.IsType<GkpErr<GkpKey, GkpKeyNotFoundError>>(result);
        Assert.Equal("Key for appId wrongAppId not found", (result as GkpErr<GkpKey, GkpKeyNotFoundError>)!.Error.Message);
        Assert.Null(gkpKey);
    }
}