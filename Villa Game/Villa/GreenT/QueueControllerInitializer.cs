using System;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Lockers;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.HornyScapes.StarShop;
using UniRx;
using Zenject;

namespace GreenT;

public class QueueControllerInitializer : IInitializable, IDisposable
{
	private readonly GameStarter _gameStarter;

	private readonly LockerController _lockerController;

	private readonly CalendarFlowRule _calendarFlowRule;

	private readonly StarShopController _starShopController;

	private readonly DecorationController _decorationController;

	private IDisposable _onGameActiveStream;

	public QueueControllerInitializer(GameStarter gameStarter, StarShopController starShopController, CalendarFlowRule calendarFlowRule, LockerController lockerController, DecorationController decorationController)
	{
		_gameStarter = gameStarter;
		_starShopController = starShopController;
		_calendarFlowRule = calendarFlowRule;
		_lockerController = lockerController;
		_decorationController = decorationController;
	}

	public void Initialize()
	{
		_onGameActiveStream?.Dispose();
		_onGameActiveStream = _gameStarter.IsGameActive.Where((bool x) => x).Subscribe(InitializeControllers);
	}

	private void InitializeControllers(bool state)
	{
		_lockerController.Initialize(state);
		_starShopController.Initialize(state);
		_decorationController.Initialize(state);
		_calendarFlowRule.Initialize();
	}

	public void Dispose()
	{
		_onGameActiveStream?.Dispose();
	}
}
