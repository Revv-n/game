using GreenT.HornyScapes.Monetization.Android.Harem;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Harem;

public class HaremPopup : MonetizationPopup
{
	[Inject]
	private SignalBus _signalBus;

	protected override void Awake()
	{
		base.Awake();
		SupportButton.onClick.AddListener(supportUrlOpener.OpenUrl);
		AbortButton.onClick.AddListener(Close);
		AbortButton.onClick.AddListener(AbortToJs);
	}

	private void AbortToJs()
	{
		_signalBus.Fire<CancelPaymentSignal>();
	}
}
