using System;
using GreenT.HornyScapes.Saves;
using UniRx;

namespace GreenT.Data;

public sealed class SaveProviderFacade : IDisposable
{
	private const long ThrottleTime = 10L;

	private readonly User user;

	private readonly ISaveProvider localSaveProvider;

	private readonly ISaveProvider remoteSaveProvider;

	private IDisposable userUpdateStream;

	private IDisposable streams;

	private long lastSendToServer;

	public SaveProviderFacade(User user, ServerSaveProvider remoteSaveProvider, FileSerializer fileSerializer)
	{
		this.user = user;
		this.remoteSaveProvider = remoteSaveProvider;
		localSaveProvider = fileSerializer;
	}

	public bool SaveToServer(SavedData data)
	{
		bool flag = data.UpdatedAt - lastSendToServer < 10;
		if (user.Type.Contains(User.State.Registered) && !flag)
		{
			lastSendToServer = data.UpdatedAt;
			remoteSaveProvider.Serialize(data);
			return true;
		}
		return false;
	}

	public void SaveToLocal(SavedData data)
	{
		localSaveProvider.Serialize(data);
	}

	public IObservable<SavedData> Deserialize<T>() where T : SavedData, new()
	{
		if (user.Type.Contains(User.State.Registered))
		{
			return Observable.Select<(SavedData, SavedData), SavedData>(Observable.CombineLatest<SavedData, SavedData, (SavedData, SavedData)>(localSaveProvider.LoadSave(), remoteSaveProvider.LoadSave(), (Func<SavedData, SavedData, (SavedData, SavedData)>)((SavedData local, SavedData remote) => (local: local, remote: remote))), (Func<(SavedData, SavedData), SavedData>)CompareSaveDate);
		}
		return localSaveProvider.LoadSave();
	}

	private SavedData CompareSaveDate((SavedData local, SavedData remote) _pair)
	{
		if (!(_pair.local?.UpdatedAt >= _pair.remote?.UpdatedAt))
		{
			return _pair.remote;
		}
		return _pair.local;
	}

	public void Dispose()
	{
		userUpdateStream?.Dispose();
	}
}
