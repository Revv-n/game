using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics;

public class Amplitude : IAmplitudeSender<AmplitudeEvent>, IAnalyticSender<AmplitudeEvent>, IDisposable
{
	private string urlBaseEventRequest;

	public readonly string APIKey;

	private int maxEventsPerBatch = 1;

	private int maxSecondsPerBatch = 10;

	private bool recordIP;

	private bool debugMessages = true;

	private float checkSendEvents = 0.25f;

	private IDisposable sendStream;

	private AmplitudeEventRequest eventRequest;

	private AmplitudeIdentificationRequest identificationRequest;

	private AnalyticUserProperties analyticUserProperties;

	private DeviceSystem deviceSystem;

	private readonly User user;

	private readonly TournamentPointsStorage tournamentPointsStorage;

	private readonly IClock clock;

	public string APIVersionAdd { get; private set; }

	public bool WasInitialized { get; private set; }

	public string DeviceId { get; private set; } = "";


	public string AppVersion { get; private set; } = "0";


	public string Platform { get; private set; } = "";


	public long SessionId { get; private set; } = -1L;


	public string IP { get; private set; } = "";


	public Amplitude(string apiKey, string urlBaseEventRequest, DeviceSystem deviceSystem, User user, IClock clock, TournamentPointsStorage tournamentPointsStorage, AnalyticUserProperties analyticUserProperties)
	{
		APIKey = apiKey;
		this.urlBaseEventRequest = urlBaseEventRequest;
		this.deviceSystem = deviceSystem;
		this.user = user;
		this.clock = clock;
		this.tournamentPointsStorage = tournamentPointsStorage;
		this.analyticUserProperties = analyticUserProperties;
		if (APIKey.Length > 8)
		{
			Initialize(Application.version);
		}
		else
		{
			Debug.LogError("Amplitude doesnt initialize");
		}
	}

	public void Initialize(string AppVersion)
	{
		if (!WasInitialized)
		{
			InitializeParams(AppVersion);
			CreateRequests();
			WasInitialized = true;
			StartRepeatRequest();
		}
	}

	public void AddEvent(AmplitudeEvent analyticsEvent)
	{
		eventRequest.AddSendEvent(analyticsEvent);
	}

	public void AddProperty(AmplitudePropertiesEvent analyticsEvent)
	{
		identificationRequest.AddToQueueProperty(analyticsEvent);
	}

	private void InitializeParams(string AppVersion)
	{
		DeviceId = deviceSystem.GetId();
		this.AppVersion = AppVersion + APIVersionAdd;
		SessionId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		SetIP();
		Platform = deviceSystem.GetPlatform();
	}

	private void CreateRequests()
	{
		eventRequest = new AmplitudeEventRequest(urlBaseEventRequest, DeviceId, APIKey, Platform, IP, AppVersion, SessionId, maxEventsPerBatch, maxSecondsPerBatch, user, clock, tournamentPointsStorage, analyticUserProperties);
		identificationRequest = new AmplitudeIdentificationRequest(DeviceId, APIKey, user, maxSecondsPerBatch, maxSecondsPerBatch);
	}

	private void StartRepeatRequest()
	{
		sendStream = ObservableExtensions.Subscribe<long>(Observable.Interval(TimeSpan.FromSeconds(checkSendEvents), Scheduler.MainThreadIgnoreTimeScale), (Action<long>)delegate
		{
			eventRequest.TrySendEventQueue();
			identificationRequest.TrySendPropertiesQueue();
		});
	}

	private void SetIP()
	{
		if (recordIP)
		{
			IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First((IPAddress f) => f.AddressFamily == AddressFamily.InterNetwork).ToString();
		}
	}

	public void Dispose()
	{
		sendStream?.Dispose();
	}
}
