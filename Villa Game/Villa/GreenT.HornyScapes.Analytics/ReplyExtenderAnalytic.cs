using GreenT.Data;
using GreenT.HornyScapes.Messenger.UI;
using StripClub.Messenger;
using StripClub.Model.Shop;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class ReplyExtenderAnalytic : AnalyticWindow<ReplyExtenderWindow>
{
	private const string ANALYTIC_REFRESH_EVENT = "messenger_answers_refresh";

	private const string ANALYTIC_INCREASE_EVENT = "messenger_answers_increase";

	private CompositeDisposable updateStream = new CompositeDisposable();

	private SavableVariable<int> refillCount;

	private ISaver saver;

	[Inject]
	private void InnerInit(ISaver saver, IMessengerManager messengerManager)
	{
		this.saver = saver;
		refillCount = new SavableVariable<int>("refillCount", 0);
		this.saver.Add(refillCount);
	}

	private void Start()
	{
		window.OnUpdateReplyExtensionCount.Subscribe(SendIncreaseEvent).AddTo(updateStream);
		window.OnRefill.Subscribe(delegate(int price)
		{
			SendRefreshEvent(price);
		}).AddTo(updateStream);
	}

	private void SendIncreaseEvent(int count)
	{
		Price<int> replyExtensionPrice = window.GetReplyExtensionPrice();
		AmplitudePropertiesEvent amplitudePropertiesEvent = new AmplitudePropertiesEvent();
		amplitudePropertiesEvent.AddProperty("messenger_answers_increase", $"{count}:{replyExtensionPrice.Value}");
		amplitude.AddProperty(amplitudePropertiesEvent);
	}

	private void SendRefreshEvent(int price)
	{
		refillCount.Value++;
		AmplitudePropertiesEvent amplitudePropertiesEvent = new AmplitudePropertiesEvent();
		amplitudePropertiesEvent.AddProperty("messenger_answers_refresh", $"{refillCount.Value}:{price}");
		amplitude.AddProperty(amplitudePropertiesEvent);
	}
}
