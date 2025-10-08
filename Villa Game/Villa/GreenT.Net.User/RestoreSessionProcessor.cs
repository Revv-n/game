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
			IObservable<Response<UserDataMapper>> source = playerCheckRequest.Post(player_id).Share();
			source.Where((Response<UserDataMapper> _response) => _response.Status == 200).Subscribe(delegate
			{
				Request(player_id);
			}).AddTo(disposables);
			source.Where((Response<UserDataMapper> _response) => _response.Status != 200).Subscribe(OnPlayerCheckFail).AddTo(disposables);
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
