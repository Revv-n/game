using System;
using GreenT.HornyScapes.Saves;

namespace GreenT.Data;

public class ServerSaveProvider : ISaveProvider, ISaveSerializer, ISaveLoader
{
	private readonly StringSerializer _stringSerializer;

	private readonly SaveLoader _saveLoader;

	private readonly SaveSender _saveSender;

	private readonly User _user;

	private readonly SaverState _saverState;

	protected ServerSaveProvider(StringSerializer stringSerializer, SaverState saverState, SaveLoader saveLoader, SaveSender saveSender, User user)
	{
		_stringSerializer = stringSerializer;
		_saverState = saverState;
		_saveLoader = saveLoader;
		_saveSender = saveSender;
		_user = user;
	}

	public IObservable<SavedData> LoadSave()
	{
		return _saveLoader.LoadSave();
	}

	public async void Serialize(SavedData state)
	{
		if (ValidateSerialize(state))
		{
			_saverState.CurrentDataString = _stringSerializer.Serialize(state);
			await _saveSender.PostSaveAsync();
		}
	}

	private bool ValidateSerialize(SavedData state)
	{
		if (state == null)
		{
			_saverState.HandleException(new ArgumentNullException("Try to save empty state declined. Player id: \"" + _user.PlayerID + "\""));
			return false;
		}
		return _user.IsPlayerValid();
	}
}
