using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.Net;
using GreenT.Settings.Data;
using Newtonsoft.Json;
using Nutaku.Unity;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Nutaku.Android;

public class NutakuAuthorization
{
	private readonly MonoBehaviour monoBehaviour;

	private readonly IDataStorage dataStorage;

	private readonly string playerIDKey;

	private readonly string authorizationUrl;

	private Subject<Unit> onSuccess = new Subject<Unit>();

	private Subject<string> onFail = new Subject<string>();

	public IObservable<Unit> OnSuccess => onSuccess.AsObservable();

	public IObservable<string> OnFail => onFail.AsObservable();

	public NutakuAuthorization(MonoBehaviour monoBehaviour, IDataStorage dataStorage, string playerIDKey, IRequestUrlResolver urlResolver)
	{
		this.dataStorage = dataStorage;
		this.playerIDKey = playerIDKey;
		authorizationUrl = urlResolver.PostRequestUrl(PostRequestType.Authorization);
		this.monoBehaviour = monoBehaviour;
	}

	public void ConnectToServer()
	{
		RestApiHelper.GetMyProfile(monoBehaviour, MyProfile);
	}

	private void MyProfile(RawResult rawResult)
	{
		if (rawResult.statusCode > 199 && rawResult.statusCode < 300)
		{
			Person firstEntry = RestApi.HandleRequestCallback<Person>(rawResult).GetFirstEntry();
			GetPlayerIdRequest(firstEntry);
			return;
		}
		onFail.OnNext("Can't get profile");
		throw new Exception(GetType().Name + ": My profile (nutaku)").LogException();
	}

	private void GetPlayerIdRequest(Person userData)
	{
		string callbackUrl = string.Format(authorizationUrl, userData.id, userData.nickname, userData.grade);
		Dictionary<string, string> postData = new Dictionary<string, string>();
		RestApiHelper.PostMakeRequest(callbackUrl, postData, monoBehaviour, CallRestoreProcessor);
	}

	private void CallRestoreProcessor(RawResult rawResult)
	{
		try
		{
			if (rawResult.statusCode > 199 && rawResult.statusCode < 300)
			{
				MakeRequestResult makeRequestResult = RestApi.HandlePostMakeRequestCallback(rawResult);
				if (makeRequestResult.rc == 500)
				{
					onFail.OnNext($"our server error, status {makeRequestResult.rc}, body: {makeRequestResult.body}");
					return;
				}
				Response<NutakuAuthData> response = null;
				try
				{
					response = JsonConvert.DeserializeObject<Response<NutakuAuthData>>(makeRequestResult.body);
				}
				catch (Exception innerException)
				{
					throw new Exception(makeRequestResult.body, innerException);
				}
				dataStorage.SetString(playerIDKey, response.Data.player_id);
				onSuccess.OnNext(default(Unit));
			}
			else
			{
				onFail.OnNext($"sdk authorization fail, status {rawResult.statusCode}");
			}
		}
		catch (Exception ex)
		{
			onFail.OnNext("error " + ex.Message);
			throw ex.LogException();
		}
	}

	private void DebugMakeRequestResult(MakeRequestResult result)
	{
		foreach (KeyValuePair<string, string> header in result.headers)
		{
			_ = header;
		}
	}
}
