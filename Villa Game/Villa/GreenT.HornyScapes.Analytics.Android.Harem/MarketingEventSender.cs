using System;
using System.Collections.Generic;
using GreenT.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Analytics.Android.Harem;

public class MarketingEventSender : IMarketingEventSender, IInitializable
{
	private const string PLAY_BUTTON_EVENT = "playBtn";

	private AndroidHaremEventRequest _eventRequest;

	private IDataStorage _dataStorage;

	private IDisposable _waitForPlayerIdDisposable;

	public MarketingEventSender(AndroidHaremEventRequest eventRequest, IDataStorage dataStorage)
	{
		_eventRequest = eventRequest;
		_dataStorage = dataStorage;
	}

	public void Initialize()
	{
		WaitForPlayerIdAndSend();
	}

	public void SendTutorStepEvent(int tutorStepNumber)
	{
		string eventName = $"tutor_{tutorStepNumber}";
		SendEvent(eventName);
	}

	public void SendPlayButtonEvent()
	{
		SendEvent("playBtn");
	}

	private void WaitForPlayerIdAndSend()
	{
		_waitForPlayerIdDisposable?.Dispose();
		_waitForPlayerIdDisposable = (from _ in Observable.EveryUpdate()
			select PlayerPrefs.GetString("player_id") into id
			where !string.IsNullOrEmpty(id)
			select id).Take(1).Subscribe(delegate
		{
			SendEvent("load");
		});
	}

	private void SendEvent(string eventName)
	{
		Dictionary<string, object> fields = new Dictionary<string, object>
		{
			{ "platform", "apk_hv" },
			{ "click_id", "" },
			{
				"player_id",
				_dataStorage.GetString("player_id")
			},
			{ "event_name", eventName },
			{
				"device_model",
				SystemInfo.deviceModel
			},
			{
				"version_OS",
				SystemInfo.operatingSystem
			}
		};
		_eventRequest.Post(fields).Subscribe(delegate
		{
		}, delegate(Exception error)
		{
			Debug.LogError($"Failed: {error}");
		});
	}
}
