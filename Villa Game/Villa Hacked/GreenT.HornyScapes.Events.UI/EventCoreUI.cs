using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;
using GreenT.HornyScapes.MergeCore;
using GreenT.Model.Reactive;
using GreenT.Types;
using GreenT.UI;
using StripClub.Model;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events.UI;

public class EventCoreUI : Window
{
	[SerializeField]
	private EventBattlePassButtonSubscription _eventBattlePassButtonSubscription;

	private CompositeDisposable disposables = new CompositeDisposable();

	private EventEnergyModeTempService eventEnergyModeTempService;

	private ReactiveCollection<CurrencyType> visibleCurrenciesManager;

	private CurrencyType[] prevWindowsVisibleCurrencies;

	private BattlePassSettingsProvider _battlePassSettingsProvider;

	private CalendarQueue _calendarQueue;

	[SerializeField]
	private AnimationSetOpenCloseController starters;

	private GameItemController gameItemController => Controller<GameItemController>.Instance;

	private SelectionController selectionController => Controller<SelectionController>.Instance;

	[field: SerializeField]
	public CurrencySpriteAttacher EventCurrencySpriteAttacher { get; private set; }

	[Inject]
	public void Init(ReactiveCollection<CurrencyType> manager, EventEnergyModeTempService eventEnergyModeTempService, BattlePassSettingsProvider battlePassSettingsProvider, CalendarQueue calendarQueue)
	{
		visibleCurrenciesManager = manager;
		this.eventEnergyModeTempService = eventEnergyModeTempService;
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_calendarQueue = calendarQueue;
	}

	public override void Open()
	{
		disposables.Clear();
		selectionController.ClearSelection();
		gameItemController.OpenField(ContentType.Event);
		if (!IsOpened)
		{
			prevWindowsVisibleCurrencies = visibleCurrenciesManager.ToArray();
			visibleCurrenciesManager.SetItems(eventEnergyModeTempService.IsSeparateEventEnergyMode ? CurrencyType.EventEnergy : CurrencyType.Energy, CurrencyType.Event, CurrencyType.Hard);
		}
		base.Open();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(starters.Open(), (Action)starters.InitClosers), (Action<AnimationSetOpenCloseController>)delegate
		{
			starters.InitClosers();
		}), (ICollection<IDisposable>)disposables);
		_eventBattlePassButtonSubscription.CheckBattlePass();
	}

	public override void Close()
	{
		visibleCurrenciesManager.SetItems(prevWindowsVisibleCurrencies);
		disposables.Clear();
		base.Close();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationSetOpenCloseController>(Observable.DoOnCancel<AnimationSetOpenCloseController>(starters.Close(), (Action)base.Close), (Action<AnimationSetOpenCloseController>)delegate
		{
			base.Close();
		}), (ICollection<IDisposable>)disposables);
	}

	private void OnDisable()
	{
		disposables.Clear();
	}
}
