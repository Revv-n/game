using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.Events;

public class BattlePassCalendarDispenser : ICalendarDispenser
{
	private readonly GameSettings _gameSettings;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	protected BattlePassCalendarDispenser(GameSettings gameSettings, BattlePassSettingsProvider battlePassSettingsProvider)
	{
		_gameSettings = gameSettings;
		_battlePassSettingsProvider = battlePassSettingsProvider;
	}

	public void Set(int balanceId)
	{
		BattlePass battlePass = _battlePassSettingsProvider.GetBattlePass(balanceId);
		_gameSettings.CurrencySettings[CurrencyType.BP, default(CompositeIdentificator)].SetSprite(battlePass.CurrentViewSettings.Currency);
		_gameSettings.CurrencySettings[CurrencyType.BP, default(CompositeIdentificator)].SetAlternativeSprite(battlePass.CurrentViewSettings.Currency);
		_gameSettings.CurrencySettings[CurrencyType.BP, default(CompositeIdentificator)].SetLocalization(battlePass.Bundle.CurrencyKeyLoc);
		_gameSettings.RewPlaceholder[RewType.Level].Set(battlePass.Bundle);
	}
}
