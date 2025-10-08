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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<User>(Observable.First<User>(Observable.Where<User>((IObservable<User>)userData.OnLogin, (Func<User, bool>)((User _user) => _user.PlayerID != null))), (Action<User>)delegate
		{
			SendEvent();
		}), (ICollection<IDisposable>)streams);
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
			((AnalyticsEvent)partnerEvent).AddEventCore("player_id", (object)userData.PlayerID);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response>(eventRequest.Post(((AnalyticsEvent)partnerEvent).Values), (Action<Response>)delegate(Response _request)
			{
				responseStatusSystem.SendLog(_request);
			}, (Action<Exception>)delegate(Exception exception)
			{
				throw exception.LogException();
			}), (ICollection<IDisposable>)streams);
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
		((AnalyticsEvent)analyticsEvent).AddEventCore("app_id", (object)app_id);
		((AnalyticsEvent)analyticsEvent).AddEventCore("device_id", (object)device_id);
		((AnalyticsEvent)analyticsEvent).AddEventCore("platform", (object)platform);
		((AnalyticsEvent)analyticsEvent).AddEventCore("app_version", (object)app_version);
		((AnalyticsEvent)analyticsEvent).AddEventCore("session_id", (object)session_id);
		((AnalyticsEvent)analyticsEvent).AddEventCore("x_forwarded_for", (object)x_forwarded_for);
	}

	public void Dispose()
	{
		streams.Dispose();
	}
}
