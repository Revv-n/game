using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Saves;
using UniRx;

namespace GreenT.Data;

public class SaveLoader
{
	private readonly StringSerializer _stringSerializer;

	private readonly SaverState _saverState;

	private readonly User _user;

	public SaveLoader(StringSerializer stringSerializer, SaverState saverState, User user)
	{
		_stringSerializer = stringSerializer;
		_saverState = saverState;
		_user = user;
	}

	public IObservable<SavedData> LoadSave()
	{
		ValidateLoadSave();
		return Observable.Defer(() => Observable.Return(GetSavedData(), Scheduler.MainThreadIgnoreTimeScale));
	}

	private SavedData GetSavedData()
	{
		string currentDataString = _saverState.CurrentDataString;
		if (string.IsNullOrEmpty(currentDataString))
		{
			return new SavedData();
		}
		List<Memento> data = _stringSerializer.Deserialize<List<Memento>>(currentDataString);
		return new SavedData(_saverState.CurrentUpdatedAt, data);
	}

	protected virtual bool ValidateLoadSave()
	{
		if (!_user.IsPlayerValid())
		{
			throw new ArgumentException("Wrong user params PID: \"" + _user.PlayerID + "\"");
		}
		return true;
	}
}
