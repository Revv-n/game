using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Webgl.Epocha;

public class EpochaPopup : MonetizationPopup
{
	protected MonetizationSubsystem subsystem;

	[Inject]
	private void InnerInit(MonetizationSubsystem subsystem)
	{
		this.subsystem = subsystem;
	}

	protected override void Awake()
	{
		base.Awake();
		SupportButton.onClick.AddListener(supportUrlOpener.OpenUrl);
		AbortButton.onClick.AddListener(subsystem.AbortPayment);
		stream = subsystem?.OnFailed.Subscribe(delegate
		{
			SetFailedView();
		});
		CloseButton.onClick.AddListener(subsystem.Clear);
		AbortButton.onClick.AddListener(SetFailedView);
	}
}
