using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(window.OnUpdateReplyExtensionCount, (Action<int>)SendIncreaseEvent), (ICollection<IDisposable>)updateStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(window.OnRefill, (Action<int>)delegate(int price)
		{
			SendRefreshEvent(price);
		}), (ICollection<IDisposable>)updateStream);
	}

	private void SendIncreaseEvent(int count)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		Price<int> replyExtensionPrice = window.GetReplyExtensionPrice();
		AmplitudePropertiesEvent val = new AmplitudePropertiesEvent();
		val.AddProperty("messenger_answers_increase", (object)$"{count}:{replyExtensionPrice.Value}");
		amplitude.AddProperty(val);
	}

	private void SendRefreshEvent(int price)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		refillCount.Value++;
		AmplitudePropertiesEvent val = new AmplitudePropertiesEvent();
		val.AddProperty("messenger_answers_refresh", (object)$"{refillCount.Value}:{price}");
		amplitude.AddProperty(val);
	}
}
