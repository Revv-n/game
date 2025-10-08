using System;
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
		MonetizationSubsystem monetizationSubsystem = subsystem;
		stream = ((monetizationSubsystem != null) ? ObservableExtensions.Subscribe<string>(monetizationSubsystem.OnFailed, (Action<string>)delegate
		{
			SetFailedView();
		}) : null);
		CloseButton.onClick.AddListener(subsystem.Clear);
		AbortButton.onClick.AddListener(SetFailedView);
	}
}
