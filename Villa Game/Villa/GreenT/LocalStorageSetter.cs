using System;
using GreenT.Data;
using UniRx;
using Zenject;

namespace GreenT;

public class LocalStorageSetter : IInitializable, IDisposable
{
	private readonly User _user;

	private readonly IDataStorage _dataStorage;

	private readonly string _playerIdKey;

	private IDisposable _stream;

	public LocalStorageSetter(User user, IDataStorage dataStorage, string playerIdKey)
	{
		_user = user;
		_dataStorage = dataStorage;
		_playerIdKey = playerIdKey;
	}

	public void Initialize()
	{
		_stream = _user.OnUpdate.Where((User _user) => !string.IsNullOrEmpty(_user.PlayerID)).Subscribe(delegate(User _user)
		{
			_dataStorage.SetString(_playerIdKey, _user.PlayerID);
		});
	}

	public void Dispose()
	{
		_stream?.Dispose();
	}
}
