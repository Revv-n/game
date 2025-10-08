using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using GreenT.HornyScapes;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace UniversalAmplitude;

public class Amplitude : MonoBehaviour
{
	public const string URL_BASE = "https://api.amplitude.com/2/httpapi";

	[Tooltip("Amplitude Web API Key")]
	public string APIKey;

	[Tooltip("Additional version info appended to Initialize method property")]
	public string APIVersionAdd;

	[Tooltip("Max events allowed per web request")]
	[Range(0f, 10f)]
	public int MaxEventsPerBatch = 5;

	[Tooltip("Max seconds to wait before sending event, even if the batch is not full")]
	public int MaxSecondsPerBatch = 10;

	[Tooltip("Record IP Address for events")]
	public bool RecordIP;

	[Tooltip("Initialize Analytics data on startup")]
	public bool InitializeOnAwake = true;

	[Tooltip("Send Debug.Log Info messages, does not disable error messages")]
	public bool DebugMessages = true;

	public static Amplitude instance;

	public static bool WasInitialized;

	private Queue<AmplitudeEvent> eventQueue = new Queue<AmplitudeEvent>();

	[NonSerialized]
	public string user_id = "";

	[NonSerialized]
	public string device_id = "";

	[NonSerialized]
	public string app_version = "0";

	[NonSerialized]
	public string platform = "";

	[NonSerialized]
	public long sessionid = -1L;

	[NonSerialized]
	public string ip = "";

	private int timeSinceLastSend;

	[Inject]
	private static DeviceSystem deviceSystem;

	private void Awake()
	{
		instance = this;
		if (InitializeOnAwake && APIKey.Length > 8)
		{
			Initialize(Application.version);
		}
	}

	private void OnEnable()
	{
		InvokeRepeating("CheckEventQueue", 0.25f, 1f);
	}

	public static void Initialize(string AppVersion)
	{
		if (instance == null || WasInitialized)
		{
			return;
		}
		instance.device_id = deviceSystem.GetId();
		if (instance.user_id == null)
		{
			instance.user_id = "";
		}
		instance.app_version = AppVersion + instance.APIVersionAdd;
		instance.sessionid = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		instance.platform = "windows";
		if (instance.RecordIP)
		{
			instance.ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First((IPAddress f) => f.AddressFamily == AddressFamily.InterNetwork).ToString();
		}
		WasInitialized = true;
		SendEvent(new AmplitudeEvent("launch"));
	}

	public static void SetUserID(string userID)
	{
		instance.user_id = userID;
	}

	public static void SendEvent(AmplitudeEvent analyticsEvent)
	{
		instance.eventQueue.Enqueue(analyticsEvent);
	}

	private void CheckEventQueue()
	{
		if (eventQueue.Count == 0)
		{
			return;
		}
		timeSinceLastSend++;
		if (timeSinceLastSend >= instance.MaxSecondsPerBatch || eventQueue.Count >= MaxEventsPerBatch)
		{
			List<AmplitudeEvent> list = new List<AmplitudeEvent>();
			int num = Mathf.Min(MaxEventsPerBatch, eventQueue.Count);
			for (int i = 0; i < num; i++)
			{
				list.Add(eventQueue.Dequeue());
			}
			SendEvents(list);
			timeSinceLastSend = 0;
		}
	}

	private async void SendEvents(List<AmplitudeEvent> events)
	{
		string bodyData = JSONFromEvents(events);
		using UnityWebRequest www = UnityWebRequest.Put("https://api.amplitude.com/2/httpapi", bodyData);
		www.method = "POST";
		www.SetRequestHeader("Content-Type", "application/json");
		www.SetRequestHeader("Accept", "application/json");
		await www.SendWebRequest();
		if (!www.isNetworkError && !www.isHttpError)
		{
			_ = DebugMessages;
		}
	}

	private string JSONFromEvents(List<AmplitudeEvent> events)
	{
		string text = "";
		foreach (AmplitudeEvent @event in events)
		{
			text = text + @event.ToJSON() + ",";
		}
		text = text.Substring(0, text.Length - 1);
		return "{\"api_key\":\"" + APIKey + "\", \"events\": [" + text + "]}";
	}

	private void OnDisable()
	{
		CancelInvoke();
	}
}
