using System;
using System.Collections.Generic;
using GreenT.Net;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class PartnerSender : IAnalyticSender<PartnerEvent>, IDisposable, IInitializable
{
	private string app_id;

	private string device_id;

	private string platform;

	private string app_version;

	private string session_id;

	private string x_forwarded_for = "11.12.12.12";

	private readonly PartnerEventRequest eventRequest;

	private readonly ResponseStatusSystem responseStatusSystem;

	private Queue<PartnerEvent> eventQueue = new Queue<PartnerEvent>();

	private CompositeDisposable streams = new CompositeDisposable();

	private DeviceSystem deviceSystem;

	private IUrlReader webglSiteReader;

	private User userData;

	public PartnerSender(PartnerEventRequest eventRequest, ResponseStatusSystem responseStatusSystem, DeviceSystem deviceSystem, IUrlReader webglSiteReader, User userData, string appName)
	{
		this.webglSiteReader = webglSiteReader;
		this.deviceSystem = deviceSystem;
		this.eventRequest = eventRequest;
		this.responseStatusSystem = responseStatusSystem;
		this.userData = userData;
		app_id = appName;
		InitializeFields();
	}

	private void InitializeFields()
	{
		device_id = deviceSystem.GetId();
		platform = deviceSystem.GetPlatform();
		app_version = Application.version;
		session_id = deviceSystem.GetSessionId();
	}

	public void Initialize()
	{
		userData.OnLogin.Where((User _user) => _user.PlayerID != null).First().Subscribe(delegate
		{
			SendEvent();
		})
			.AddTo(streams);
	}

	private void SendEvent()
	{
		if (eventQueue.Count == 0)
		{
			return;
		}
		int count = eventQueue.Count;
		for (int i = 0; i < count; i++)
		{
			PartnerEvent partnerEvent = eventQueue.Dequeue();
			partnerEvent.AddEventCore("player_id", userData.PlayerID);
			eventRequest.Post(partnerEvent.Values).Subscribe(delegate(Response _request)
			{
				responseStatusSystem.SendLog(_request);
			}, delegate(Exception exception)
			{
				throw exception.LogException();
			}).AddTo(streams);
		}
	}

	public void AddEvent(PartnerEvent analyticsEvent)
	{
		if (!Application.isEditor)
		{
			FillEventBaseFields(analyticsEvent);
			eventQueue.Enqueue(analyticsEvent);
			if (userData.PlayerID != null)
			{
				SendEvent();
			}
		}
	}

	private void FillEventBaseFields(PartnerEvent analyticsEvent)
	{
		analyticsEvent.AddEventCore("app_id", app_id);
		analyticsEvent.AddEventCore("device_id", device_id);
		analyticsEvent.AddEventCore("platform", platform);
		analyticsEvent.AddEventCore("app_version", app_version);
		analyticsEvent.AddEventCore("session_id", session_id);
		analyticsEvent.AddEventCore("x_forwarded_for", x_forwarded_for);
	}

	public void Dispose()
	{
		streams.Dispose();
	}
}
