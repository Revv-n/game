using System;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Events;
using GreenT.UI;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Bank.GoldenTickets.UI;

public class SubscriptionGoldenTicketView : MonoView
{
	[Header("Go to option:")]
	[SerializeField]
	private Button _goToButton;

	[SerializeField]
	private OpenSection _sectionOpener;

	[SerializeField]
	private MonoContentSetter _contentSetter;

	[SerializeField]
	private WindowOpener _windowOpener;

	public void Set(SubscriptionLot subscriptionLot)
	{
		_sectionOpener.Set(subscriptionLot.TabID);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_goToButton), (Action<Unit>)delegate
		{
			_contentSetter.Set();
			_windowOpener.OpenOnly();
			_sectionOpener.Open();
		}), (Component)this);
	}
}
