using System.Collections.Concurrent;

using Gatekeeper.KeyLib.Models;

namespace Gatekeeper.KeyLib.Features.KeyStorage;

public sealed class InMemoryKeyRepository : IKeyRepository
{
	private readonly ConcurrentDictionary<string, GkpKey> _keyStorage = new();

	public Task SaveKeyAsync(GkpKey keyModel)
	{
		_keyStorage[keyModel.AppId] = keyModel;
		return Task.CompletedTask;
	}

	public Task<GkpKey?> GetKeyForAppIdAsync(string appId)
	{
		_keyStorage.TryGetValue(appId, out var keyModel);
		return Task.FromResult(keyModel);
	}
}
