using GreenT.HornyScapes.Events;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class LastChanceEventButtonView : LastChanceButtonView
{
	private LastChanceEventBundleProvider _eventBundlesProvider;

	[Inject]
	private void Init(LastChanceEventBundleProvider eventBundlesProvider)
	{
		_eventBundlesProvider = eventBundlesProvider;
	}

	protected override void Activate()
	{
		EventBundleData eventBundleData = _eventBundlesProvider.TryGet(_currentLastChance.EventId);
		if (eventBundleData != null)
		{
			_icon.sprite = eventBundleData.ButtonSp;
		}
	}
}
