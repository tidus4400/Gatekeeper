using Gatekeeper.KeyLib.Features.KeyGeneration;
using Gatekeeper.KeyLib.Features.KeyStorage;
using Gatekeeper.KeyLib.Models;
using Gatekeeper.KeyLib.Services;
using Moq;


namespace Gatekeeper.KeyLib.Tests.Services;

public class GatekeeperKeyServiceTest
{
    private readonly Mock<IKeyGenerator> _mockKeyGenerator;
    private readonly Mock<IKeyRepository> _mockKeyRepository;
    private readonly GatekeeperKeyService _service;

    public GatekeeperKeyServiceTest()
    {
        _mockKeyGenerator = new Mock<IKeyGenerator>();
        _mockKeyRepository = new Mock<IKeyRepository>();
        _service = new GatekeeperKeyService(_mockKeyGenerator.Object, _mockKeyRepository.Object);
    }

    [Fact]
    public async Task GenerateAndStoreKeyForAppIdAsync_ShouldGenerateAndStoreKey()
    {
        // Arrange
        string appId = "testAppId";
        string createdBy = "testUser";
        string cryptoKey = "generatedCryptoKey";
        GatekeeperKey expectedKey = new(cryptoKey, appId, createdBy, DateTime.UtcNow);

        _mockKeyGenerator.Setup(x => x.GenerateCryptoKeyAsync()).ReturnsAsync(cryptoKey);
        _mockKeyRepository.Setup(x => x.SaveKeyAsync(It.IsAny<GatekeeperKey>())).Returns(Task.CompletedTask);

        // Act
        GatekeeperKey result = await _service.GenerateAndStoreKeyForAppIdAsync(appId, createdBy);

        // Assert
        Assert.Equal(expectedKey.AppId, result.AppId);
        Assert.Equal(expectedKey.CreatedBy, result.CreatedBy);
        Assert.Equal(expectedKey.Key, result.Key);
        _mockKeyGenerator.Verify(x => x.GenerateCryptoKeyAsync(), Times.Once);
        _mockKeyRepository.Verify(x => x.SaveKeyAsync(It.IsAny<GatekeeperKey>()), Times.Once);
    }

    [Fact]
    public async Task GetKeyForAppIdAsync_ShouldReturnKey()
    {
        // Arrange
        string appId = "testAppId";
        GatekeeperKey expectedKey = new("cryptoKey", appId, "createdBy", DateTime.UtcNow);

        _mockKeyRepository.Setup(x => x.GetKeyForAppIdAsync(appId)).ReturnsAsync(expectedKey);

        // Act
        GatekeeperKey? result = await _service.GetKeyForAppIdAsync(appId);

        // Assert
        Assert.Equal(expectedKey, result);
        _mockKeyRepository.Verify(x => x.GetKeyForAppIdAsync(appId), Times.Once);
    }
}