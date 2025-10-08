using System;
using System.Collections.Generic;
using GreenT.Data;
using UniRx;
using Zenject;

namespace GreenT.Net.User;

public sealed class RestoreSessionProcessor : UserPostRequestProcessor
{
	private readonly IPostRequest<Response<UserDataMapper>, string> playerCheckRequest;

	private readonly IDataStorage dataStorage;

	private readonly string playerIDKey;

	private CompositeDisposable disposables = new CompositeDisposable();

	public RestoreSessionProcessor(IPostRequest<Response<UserDataMapper>, string> playerCheckRequest, IDataStorage dataStorage, [Inject(Id = "GetData")] IPostRequest<Response<UserDataMapper>> getUserData, GreenT.User user, string player_id_key)
		: base(getUserData, user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		this.playerCheckRequest = playerCheckRequest;
		this.dataStorage = dataStorage;
		playerIDKey = player_id_key;
	}

	public void RestoreRequest()
	{
		DefaultRestoreRequest();
	}

	private void DefaultRestoreRequest()
	{
		if (dataStorage.HasKey(playerIDKey) || dataStorage.HasKey("Player ID"))
		{
			if (dataStorage.HasKey("Player ID"))
			{
				dataStorage.SetString(playerIDKey, dataStorage.GetString("Player ID"));
			}
			string player_id = dataStorage.GetString(playerIDKey);
			IObservable<Response<UserDataMapper>> observable = Observable.Share<Response<UserDataMapper>>(playerCheckRequest.Post(player_id));
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response<UserDataMapper>>(Observable.Where<Response<UserDataMapper>>(observable, (Func<Response<UserDataMapper>, bool>)((Response<UserDataMapper> _response) => _response.Status == 200)), (Action<Response<UserDataMapper>>)delegate
			{
				Request(player_id);
			}), (ICollection<IDisposable>)disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response<UserDataMapper>>(Observable.Where<Response<UserDataMapper>>(observable, (Func<Response<UserDataMapper>, bool>)((Response<UserDataMapper> _response) => _response.Status != 200)), (Action<Response<UserDataMapper>>)OnPlayerCheckFail), (ICollection<IDisposable>)disposables);
		}
		else
		{
			InitNew();
		}
		void OnPlayerCheckFail(Response<UserDataMapper> obj)
		{
			InitNew();
		}
	}

	private void Request(string player_id)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (!string.IsNullOrEmpty(player_id))
		{
			dictionary["player_id"] = player_id;
		}
		PostRequest(dictionary);
	}

	private void InitNew()
	{
		user.Init();
		user.LoginNotify();
	}

	public override void Dispose()
	{
		disposables.Dispose();
		base.Dispose();
	}
}
