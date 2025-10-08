using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using StripClub.Extensions;
using UnityEngine;
using UnityEngine.Networking;

namespace GreenT.HornyScapes.Analytics;

public class AmplitudeEventRequest
{
	private readonly string urlBase;

	private readonly string deviceId;

	private readonly string apiKey;

	private readonly string platform;

	private readonly string ip;

	private readonly string appVersion;

	private readonly long sessionId;

	private readonly int maxEventsPerBatch;

	private readonly int maxSecondsPerBatch;

	private readonly User user;

	private readonly TournamentPointsStorage tournamentPointsStorage;

	private readonly IClock clock;

	private readonly AnalyticUserProperties analyticUserProperties;

	public bool DebugMessages = true;

	private Queue<AmplitudeEvent> eventQueue = new Queue<AmplitudeEvent>();

	private int timeSinceLastSend;

	public AmplitudeEventRequest(string urlBase, string deviceId, string apiKey, string platform, string ip, string appVersion, long sessionId, int maxEventsPerBatch, int maxSecondsPerBatch, User user, IClock clock, TournamentPointsStorage tournamentPointsStorage, AnalyticUserProperties analyticUserProperties)
	{
		this.urlBase = urlBase;
		this.deviceId = deviceId;
		this.apiKey = apiKey;
		this.platform = platform;
		this.user = user;
		this.ip = ip;
		this.appVersion = appVersion;
		this.sessionId = sessionId;
		this.maxEventsPerBatch = maxEventsPerBatch;
		this.maxSecondsPerBatch = maxSecondsPerBatch;
		this.user = user;
		this.clock = clock;
		this.tournamentPointsStorage = tournamentPointsStorage;
		this.analyticUserProperties = analyticUserProperties;
	}

	public void AddSendEvent(AmplitudeEvent analyticsEvent)
	{
		AddConstFields();
		eventQueue.Enqueue(analyticsEvent);
		void AddConstFields()
		{
			((AnalyticsEvent)analyticsEvent).AddEventCore("platform", (object)platform);
			((AnalyticsEvent)analyticsEvent).AddEventCore("device_id", (object)deviceId);
			string playerID = user.PlayerID;
			if (playerID != null && playerID.Length > 0)
			{
				((AnalyticsEvent)analyticsEvent).AddEventCore("user_id", (object)user.PlayerID);
			}
			if (ip.Length > 0)
			{
				((AnalyticsEvent)analyticsEvent).AddEventCore("ip", (object)ip);
			}
			((AnalyticsEvent)analyticsEvent).AddEventCore("app_version", (object)appVersion);
			((AnalyticsEvent)analyticsEvent).AddEventCore("session_id", (object)sessionId);
			string text = Guid.NewGuid().ToString() + clock.GetTime().ConvertToUnixTimestamp() + deviceId;
			((AnalyticsEvent)analyticsEvent).AddEventCore("insert_id", (object)text);
			analyticUserProperties.AddUserPropertiesToAnalyticEvent(analyticsEvent);
		}
	}

	public void TrySendEventQueue()
	{
		timeSinceLastSend++;
		if (eventQueue.Count != 0)
		{
			for (int i = 0; i < eventQueue.Count; i++)
			{
				SendEvents(urlBase, (AnalyticsEvent)(object)eventQueue.Dequeue());
			}
		}
	}

	private async void SendEvents(string url, AnalyticsEvent _event)
	{
		string json = _event.ToJSON();
		byte[] bytes = Encoding.UTF8.GetBytes(json);
		string text = JsonConvert.SerializeObject(new AmplitudeRequestData(apiKey, bytes));
		UnityWebRequest www = UnityWebRequest.Put(url, text);
		try
		{
			www.method = "POST";
			www.SetRequestHeader("Content-Type", "application/json");
			www.SetRequestHeader("Accept", "application/json");
			await www.SendWebRequest();
			if (!www.isNetworkError && !www.isHttpError && DebugMessages)
			{
				DebugEvent(_event, json);
			}
		}
		finally
		{
			((IDisposable)www)?.Dispose();
		}
	}

	private void DebugEvents(List<AnalyticsEvent> events, string json)
	{
		foreach (AnalyticsEvent @event in events)
		{
			DebugEvent(@event, json);
		}
	}

	private void DebugEvent(AnalyticsEvent events, string json)
	{
		_ = DebugMessages;
	}

	private void BatchRequest()
	{
		if (timeSinceLastSend >= maxSecondsPerBatch || eventQueue.Count >= maxEventsPerBatch)
		{
			List<AnalyticsEvent> list = new List<AnalyticsEvent>();
			int num = Mathf.Min(maxEventsPerBatch, eventQueue.Count);
			for (int i = 0; i < num; i++)
			{
				list.Add((AnalyticsEvent)(object)eventQueue.Dequeue());
			}
			timeSinceLastSend = 0;
		}
	}

	private string JSONFromEvents(List<AnalyticsEvent> events)
	{
		string text = "";
		foreach (AnalyticsEvent @event in events)
		{
			text = text + @event.ToJSON() + ",";
		}
		return text.Substring(0, text.Length - 1);
	}
}
