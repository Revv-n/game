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
			analyticsEvent.AddEventCore("platform", platform);
			analyticsEvent.AddEventCore("device_id", deviceId);
			string playerID = user.PlayerID;
			if (playerID != null && playerID.Length > 0)
			{
				analyticsEvent.AddEventCore("user_id", user.PlayerID);
			}
			if (ip.Length > 0)
			{
				analyticsEvent.AddEventCore("ip", ip);
			}
			analyticsEvent.AddEventCore("app_version", appVersion);
			analyticsEvent.AddEventCore("session_id", sessionId);
			string value = Guid.NewGuid().ToString() + clock.GetTime().ConvertToUnixTimestamp() + deviceId;
			analyticsEvent.AddEventCore("insert_id", value);
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
				SendEvents(urlBase, eventQueue.Dequeue());
			}
		}
	}

	private async void SendEvents(string url, AnalyticsEvent _event)
	{
		string json = _event.ToJSON();
		byte[] bytes = Encoding.UTF8.GetBytes(json);
		string bodyData = JsonConvert.SerializeObject(new AmplitudeRequestData(apiKey, bytes));
		using UnityWebRequest www = UnityWebRequest.Put(url, bodyData);
		www.method = "POST";
		www.SetRequestHeader("Content-Type", "application/json");
		www.SetRequestHeader("Accept", "application/json");
		await www.SendWebRequest();
		if (!www.isNetworkError && !www.isHttpError && DebugMessages)
		{
			DebugEvent(_event, json);
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
				list.Add(eventQueue.Dequeue());
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
