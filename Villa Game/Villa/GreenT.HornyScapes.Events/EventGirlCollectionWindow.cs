using GreenT.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public class EventGirlCollectionWindow : Window
{
	[SerializeField]
	private RectTransform _rectTransform;

	private EventGirlCollectionWindowState _state;

	private IEventGirlCollectionPositionStrategy _currentStrategy;

	private DefaultEventGirlCollectionPositionStrategy _defaultStrategy;

	private BattlePassEventGirlCollectionPositionStrategy _battlePassStrategy;

	public override void Open()
	{
		CheckStrategy();
		ApplyStrategy();
		base.Open();
	}

	public override void Close()
	{
		base.Close();
		_state = EventGirlCollectionWindowState.Default;
	}

	public void UpdateState(EventGirlCollectionWindowState state)
	{
		_state = state;
		CheckStrategy();
		ApplyStrategy();
	}

	protected override void Awake()
	{
		base.Awake();
		_state = EventGirlCollectionWindowState.Default;
		_defaultStrategy = new DefaultEventGirlCollectionPositionStrategy(_rectTransform);
		_battlePassStrategy = new BattlePassEventGirlCollectionPositionStrategy(_rectTransform);
		_currentStrategy = _defaultStrategy;
	}

	private void CheckStrategy()
	{
		IEventGirlCollectionPositionStrategy currentStrategy;
		if (_state != EventGirlCollectionWindowState.WithBattlePass)
		{
			IEventGirlCollectionPositionStrategy defaultStrategy = _defaultStrategy;
			currentStrategy = defaultStrategy;
		}
		else
		{
			IEventGirlCollectionPositionStrategy defaultStrategy = _battlePassStrategy;
			currentStrategy = defaultStrategy;
		}
		_currentStrategy = currentStrategy;
	}

	private void ApplyStrategy()
	{
		_currentStrategy.UpdatePosition();
	}
}
