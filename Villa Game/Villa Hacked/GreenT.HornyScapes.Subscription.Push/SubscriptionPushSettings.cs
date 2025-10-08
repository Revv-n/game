using System;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.Bank.Data;
using GreenT.Model.Collections;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Subscription.Push;

public class SubscriptionPushSettings : IBankSection
{
	public class Manager : SimpleManager<SubscriptionPushSettings>
	{
	}

	private readonly Subject<Unit> _onPushSubject = new Subject<Unit>();

	public int ID { get; }

	public LayoutType Layout { get; }

	public SubscriptionLot[] SubscriptionID { get; }

	public int PushTime { get; }

	public int? PushOffset { get; }

	public int GoTo { get; }

	public string LocalizationKey { get; }

	public ILocker Lock { get; }

	public SubscriptionPushSettings(SubscriptionPushMapper mapper, SubscriptionLot[] lots, CompositeLocker locker)
	{
		Lock = locker;
		ID = mapper.id;
		GoTo = mapper.go_to;
		SubscriptionID = lots;
		PushTime = mapper.push_time;
		PushOffset = mapper.push_offset;
		LocalizationKey = mapper.header_localization;
		Layout = SubscriptionID.Length switch
		{
			1 => LayoutType.OneItem, 
			2 => LayoutType.DoubleOffer, 
			_ => LayoutType.OneItem, 
		};
	}

	public void InvokeOnPushedEvent()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		_onPushSubject.OnNext(default(Unit));
	}

	public IObservable<Unit> OnPush()
	{
		return Observable.AsObservable<Unit>((IObservable<Unit>)_onPushSubject);
	}
}
