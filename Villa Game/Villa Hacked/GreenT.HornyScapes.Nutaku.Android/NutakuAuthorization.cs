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

	public IObservable<Unit> OnSuccess => Observable.AsObservable<Unit>((IObservable<Unit>)onSuccess);

	public IObservable<string> OnFail => Observable.AsObservable<string>((IObservable<string>)onFail);

	public NutakuAuthorization(MonoBehaviour monoBehaviour, IDataStorage dataStorage, string playerIDKey, IRequestUrlResolver urlResolver)
	{
		this.dataStorage = dataStorage;
		this.playerIDKey = playerIDKey;
		authorizationUrl = urlResolver.PostRequestUrl(PostRequestType.Authorization);
		this.monoBehaviour = monoBehaviour;
	}

	public void ConnectToServer()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		RestApiHelper.GetMyProfile(monoBehaviour, new callbackFunctionDelegate(MyProfile), (PeopleQueryParameterBuilder)null);
	}

	private void MyProfile(RawResult rawResult)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		string text = string.Format(authorizationUrl, userData.id, userData.nickname, userData.grade);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		RestApiHelper.PostMakeRequest(text, (IEnumerable<KeyValuePair<string, string>>)dictionary, monoBehaviour, new callbackFunctionDelegate(CallRestoreProcessor));
	}

	private void CallRestoreProcessor(RawResult rawResult)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (rawResult.statusCode > 199 && rawResult.statusCode < 300)
			{
				MakeRequestResult val = RestApi.HandlePostMakeRequestCallback(rawResult);
				if (val.rc == 500)
				{
					onFail.OnNext($"our server error, status {val.rc}, body: {val.body}");
					return;
				}
				Response<NutakuAuthData> response = null;
				try
				{
					response = JsonConvert.DeserializeObject<Response<NutakuAuthData>>(val.body);
				}
				catch (Exception innerException)
				{
					throw new Exception(val.body, innerException);
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
