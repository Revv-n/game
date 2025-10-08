using GreenT.Types;
using GreenT.UI;
using StripClub.Model;

namespace GreenT.HornyScapes.Events;

public class EventCalendarDispenser : ICalendarDispenser
{
	private MergeBackWindow _mergeBackWindow;

	private readonly IWindowsManager _windowsManager;

	private readonly GameSettings _gameSettings;

	private readonly EventSettingsProvider _eventSettingsProvider;

	protected EventCalendarDispenser(IWindowsManager windowsManager, GameSettings gameSettings, EventSettingsProvider eventSettingsProvider)
	{
		_windowsManager = windowsManager;
		_gameSettings = gameSettings;
		_eventSettingsProvider = eventSettingsProvider;
	}

	public void Set(int balanceId)
	{
		EventBundleData bundle = _eventSettingsProvider.GetEvent(balanceId).Bundle;
		_gameSettings.CurrencySettings[CurrencyType.Event, default(CompositeIdentificator)].SetSprite(bundle.Currency);
		_gameSettings.CurrencySettings[CurrencyType.Event, default(CompositeIdentificator)].SetAlternativeSprite(bundle.AlternativeCurrency);
		_gameSettings.CurrencySettings[CurrencyType.Event, default(CompositeIdentificator)].SetLocalization(bundle.CurrencyKeyLoc);
		_gameSettings.CurrencySettings[CurrencyType.EventXP, default(CompositeIdentificator)].SetSprite(bundle.Target);
		_gameSettings.CurrencySettings[CurrencyType.EventXP, default(CompositeIdentificator)].SetAlternativeSprite(bundle.Target);
		_gameSettings.CurrencySettings[CurrencyType.EventXP, default(CompositeIdentificator)].SetLocalization(bundle.TargetKeyLoc);
		_mergeBackWindow = _windowsManager.Get<MergeBackWindow>();
		_mergeBackWindow.Set(bundle.MergeBackground);
	}
}
