using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenT.Net;
using UniRx;
using Zenject;

namespace GreenT.Data;

public class SaveSender : IDisposable
{
	private readonly IPostRequest<Response<UserDataMapper>> _setDataRequest;

	private readonly SaverState _saverState;

	private readonly User _user;

	private Task<Response<UserDataMapper>> SaveTask;

	public SaveSender([Inject(Id = "SetData")] IPostRequest<Response<UserDataMapper>> setDataRequest, SaverState saverState, User user)
	{
		_setDataRequest = setDataRequest;
		_saverState = saverState;
		_user = user;
	}

	public async Task<Response<UserDataMapper>> PostSaveAsync()
	{
		IDictionary<string, string> userRequestParameters = GetUserRequestParameters();
		userRequestParameters["data"] = _saverState.CurrentDataString;
		SaveTask = TaskObservableExtensions.ToTask<Response<UserDataMapper>>(_setDataRequest.Post(userRequestParameters));
		return await SaveTask;
	}

	private IDictionary<string, string> GetUserRequestParameters()
	{
		return new Dictionary<string, string>
		{
			["player_id"] = _user.PlayerID,
			["user_name"] = _user.Nickname
		};
	}

	public void Dispose()
	{
		SaveTask?.Dispose();
	}
}
