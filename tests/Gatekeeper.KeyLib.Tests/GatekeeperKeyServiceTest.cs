using Gatekeeper.KeyLib.Features.KeyGeneration;
using Gatekeeper.KeyLib.Features.KeyStorage;
using Gatekeeper.KeyLib.Models;
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
        (GkpKey? key, Exception? err) = await _service.GetKeyForAppIdAsync(appId);

        // Assert
        Assert.Null(err);
        Assert.Equal(expectedKey, key);

    }

    [Fact]
    public async Task GetKeyForAppIdAsync_ShouldReturnError_IfKeyNotExistsInRepo()
    {
        // Arrange
        string appId = "testAppId";
        GkpKey expectedKey = new("cryptoKey", appId, "createdBy", DateTime.UtcNow);

        await _inMemoryKeyRepository.SaveKeyAsync(expectedKey);

        // Act
        (GkpKey? key, Exception? err) = await _service.GetKeyForAppIdAsync("wrongAppId");

        // Assert
        Assert.NotNull(err);
        Assert.IsType<KeyNotFoundException>(err);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => throw err!);

    }
}