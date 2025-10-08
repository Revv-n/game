using System;
using GreenT.Data;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.BattlePassSpace.Data;

public class BattlePasLevelInfoCase : IDisposable
{
	private readonly BattlePass _battlePass;

	private readonly ReactiveProperty<int> _levelProperty = new ReactiveProperty<int>();

	private SimpleCurrency _currency;

	private readonly ISaver _saver;

	private IDisposable _pointsSubscribe;

	public IReadOnlyReactiveProperty<int> LevelProperty => (IReadOnlyReactiveProperty<int>)(object)_levelProperty;

	public IReadOnlyReactiveProperty<int> Points => (IReadOnlyReactiveProperty<int>)(object)_currency.Count;

	public void AddPoints(int value)
	{
		ReactiveProperty<int> count = _currency.Count;
		count.Value += value;
	}

	public BattlePasLevelInfoCase(BattlePass battlePass, SimpleCurrency currency, ISaver saver)
	{
		_battlePass = battlePass;
		_currency = currency;
		_saver = saver;
		_pointsSubscribe = ObservableExtensions.Subscribe<int>((IObservable<int>)Points, (Action<int>)OnPointsChanged);
	}

	private void OnPointsChanged(int points)
	{
		int levelForPoints = _battlePass.GetLevelForPoints(points);
		if (levelForPoints != _levelProperty.Value)
		{
			_levelProperty.Value = levelForPoints;
		}
	}

	public void Reset()
	{
		_currency.Count.Value = 0;
		_levelProperty.Value = 0;
	}

	public void Delete()
	{
		_saver.Delete(_currency);
		Dispose();
	}

	public void Dispose()
	{
		_pointsSubscribe?.Dispose();
	}
}
