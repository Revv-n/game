using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Characters;
using GreenT.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventStartView : EventView
{
	[SerializeField]
	private DropsContentView dropsContentView;

	[SerializeField]
	private Button _startButton;

	private IWindowsManager _windowsManager;

	[Inject]
	public void Construct(IWindowsManager windowsManager)
	{
		_windowsManager = windowsManager;
		SubscribeButtons();
	}

	public override void Set(CalendarModel calendarModel)
	{
		base.Set(calendarModel);
		Event @event = _eventSettingsProvider.GetEvent(calendarModel.BalanceId);
		if (@event != null)
		{
			background.sprite = @event.ViewSettings.StartWindowBackground;
			girlImage.sprite = @event.ViewSettings.StartGirl;
			DisplayContent(@event);
		}
	}

	protected override Sprite GetFocusGirl(ICharacter character)
	{
		return character.GetBundleData().CardImages.Last();
	}

	private void DisplayContent(Event eventSettings)
	{
		dropsContentView.Set(eventSettings.PreviewCards.ToList());
	}

	private void SubscribeButtons()
	{
		_startButton.onClick.AddListener(Clicked);
	}

	private void Clicked()
	{
		_windowsManager.Get<EventGirlCollectionWindow>().UpdateState(EventGirlCollectionWindowState.WithBattlePass);
	}
}
