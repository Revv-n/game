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
		_stream = ObservableExtensions.Subscribe<User>(Observable.Where<User>(_user.OnUpdate, (Func<User, bool>)((User _user) => !string.IsNullOrEmpty(_user.PlayerID))), (Action<User>)delegate(User _user)
		{
			_dataStorage.SetString(_playerIdKey, _user.PlayerID);
		});
	}

	public void Dispose()
	{
		_stream?.Dispose();
	}
}
