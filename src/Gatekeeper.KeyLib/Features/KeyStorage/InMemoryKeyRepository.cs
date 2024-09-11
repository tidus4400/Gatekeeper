using System.Collections.Concurrent;

using Gatekeeper.KeyLib.Models;


namespace Gatekeeper.KeyLib.Features.KeyStorage;

public class InMemoryKeyRepository : IKeyRepository
{
	private readonly ConcurrentDictionary<string, GatekeeperKey> _keyStorage = new();

	public Task SaveKeyAsync(GatekeeperKey keyModel)
	{
		_keyStorage[keyModel.AppId] = keyModel;
		return Task.CompletedTask;
	}

	public Task<GatekeeperKey?> GetKeyForAppIdAsync(string appId)
	{
		_keyStorage.TryGetValue(appId, out var keyModel);
		return Task.FromResult(keyModel);
	}
}
