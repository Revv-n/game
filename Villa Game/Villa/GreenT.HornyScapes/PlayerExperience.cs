using System;
using GreenT.Data;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.BattlePassSpace.Data;
using UniRx;

namespace GreenT.HornyScapes;

[Obsolete("Use BattlePasLevelInfoCase")]
public class PlayerExperience : ISavableState, IDisposable
{
	private readonly BattlePassProvider _battlePassProvider;

	private IDisposable _currencyStream;

	private IDisposable _xpStream;

	private const string Key = "PlayerExperience";

	public IReadOnlyReactiveProperty<int> Level => LevelInfo.LevelProperty;

	public IReadOnlyReactiveProperty<int> XP => LevelInfo.Points;

	private BattlePasLevelInfoCase LevelInfo => _battlePassProvider.CalendarChangeProperty.Value.battlePass.Data.LevelInfo;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	private PlayerExperience(BattlePassProvider battlePassProvider)
	{
		_battlePassProvider = battlePassProvider;
	}

	public string UniqueKey()
	{
		return "PlayerExperience";
	}

	public Memento SaveState()
	{
		return new PlayerExpMemento(this);
	}

	public void LoadState(Memento memento)
	{
	}

	public void Dispose()
	{
		_currencyStream?.Dispose();
		_xpStream?.Dispose();
	}
}
