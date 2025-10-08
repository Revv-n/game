using System;
using System.Collections.Generic;
using GreenT.Net;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics;

public class AmplitudeIdentificationRequest : IDisposable
{
	public const string URL_IDENTIFIER = "https://api2.amplitude.com/identify?api_key={0}&identification={1}";

	private readonly string deviceId;

	private readonly string apiKey;

	private readonly User user;

	private readonly int maxEventsPerBatch;

	private readonly int maxSecondsPerBatch;

	private IDisposable sendPropStream;

	private Queue<AmplitudePropertiesEvent> propertiesQueue = new Queue<AmplitudePropertiesEvent>();

	private int timeSinceLastSend;

	public AmplitudeIdentificationRequest(string deviceId, string apiKey, User user, int maxEventsPerBatch = 5, int maxSecondsPerBatch = 10)
	{
		this.deviceId = deviceId;
		this.apiKey = apiKey;
		this.user = user;
		this.maxSecondsPerBatch = maxSecondsPerBatch;
		this.maxEventsPerBatch = maxEventsPerBatch;
	}

	public void AddToQueueProperty(AmplitudePropertiesEvent analyticsEvent)
	{
		analyticsEvent.SetDeviceId(deviceId);
		analyticsEvent.AddProperty("playerId", (object)user.PlayerID);
		propertiesQueue.Enqueue(analyticsEvent);
	}

	public bool TrySendPropertiesQueue()
	{
		if (propertiesQueue.Count == 0)
		{
			return false;
		}
		timeSinceLastSend++;
		if (timeSinceLastSend < maxSecondsPerBatch && propertiesQueue.Count < maxEventsPerBatch)
		{
			return false;
		}
		List<AnalyticsEvent> list = new List<AnalyticsEvent>();
		int num = Mathf.Min(maxEventsPerBatch, propertiesQueue.Count);
		for (int i = 0; i < num; i++)
		{
			list.Add((AnalyticsEvent)(object)propertiesQueue.Dequeue());
		}
		SendProp(list);
		return true;
	}

	private void SendProp(List<AnalyticsEvent> events)
	{
		string arg = events[0].ToJSON();
		string url = $"https://api2.amplitude.com/identify?api_key={apiKey}&identification={arg}";
		sendPropStream?.Dispose();
		sendPropStream = ObservableExtensions.Subscribe<string>(HttpRequestExecutor.GetRequest(url), (Action<string>)delegate
		{
		});
	}

	public void Dispose()
	{
		sendPropStream?.Dispose();
	}
}
