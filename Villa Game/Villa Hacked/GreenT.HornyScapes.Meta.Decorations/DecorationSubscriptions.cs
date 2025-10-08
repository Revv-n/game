using System;
using System.Collections.Generic;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.HornyScapes.UI;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using StripClub.UI.Rewards;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Meta.Decorations;

public class DecorationSubscriptions : IDisposable
{
	private readonly StartFlow _startFlow;

	private readonly DecorationManager _decorationManager;

	private readonly IHouseBuilder _builder;

	private readonly RoomManager _house;

	private readonly IWindowsManager _windowsManager;

	private readonly IndicatorDisplayService _displayService;

	private readonly SignalBus _signalBus;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public DecorationSubscriptions(StartFlow startFlow, DecorationManager decorationManager, IHouseBuilder builder, RoomManager house, IndicatorDisplayService displayService, IWindowsManager windowsManager, SignalBus signalBus)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_house = house;
		_builder = builder;
		_signalBus = signalBus;
		_startFlow = startFlow;
		_displayService = displayService;
		_windowsManager = windowsManager;
		_decorationManager = decorationManager;
	}

	public void Activate(bool isGameActive)
	{
		_disposables.Clear();
		if (!isGameActive)
		{
			return;
		}
		RewardsWindow rewardsWindow = _windowsManager.Get<RewardsWindow>();
		IConnectableObservable<Decoration> val = Observable.Publish<Decoration>(_decorationManager.OnUpdate);
		IObservable<Decoration> observable = Observable.Where<Decoration>((IObservable<Decoration>)val, (Func<Decoration, bool>)((Decoration decor) => decor.State == EntityStatus.Rewarded));
		IObservable<Decoration> observable2 = Observable.Do<Decoration>(Observable.Where<Decoration>(observable, (Func<Decoration, bool>)((Decoration decor) => decor.DisplayCondition.IsOpen.Value)), (Action<Decoration>)delegate(Decoration decoration)
		{
			try
			{
				_startFlow.Launch(decoration);
			}
			catch (Exception exception2)
			{
				exception2.LogException();
			}
		});
		IObservable<Unit> observable3 = Observable.SelectMany<Decoration, Unit>(observable2, Observable.Take<Unit>(rewardsWindow.OnClose, 1));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.Merge<Unit>(Observable.AsUnitObservable<Lootbox>(Observable.SelectMany<Decoration, Lootbox>(observable2, Observable.Take<Lootbox>(rewardsWindow.OnCloseWithLootbox, 1))), new IObservable<Unit>[1] { observable3 }), (Action<Unit>)delegate
		{
			try
			{
				_startFlow.SnapToObjects();
			}
			catch (Exception exception)
			{
				exception.LogException();
			}
		}), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Decoration>(Observable.Where<Decoration>(Observable.Merge<Decoration>(Observable.SelectMany<Decoration, Decoration>(Observable.Where<Decoration>((IObservable<Decoration>)val, (Func<Decoration, bool>)((Decoration _) => _startFlow.IsLaunched)), (Func<Decoration, IObservable<Decoration>>)((Decoration decoration) => Observable.Select<IRoomObject<BaseObjectConfig>, Decoration>(Observable.Where<IRoomObject<BaseObjectConfig>>(_startFlow.OnSnapedTo, (Func<IRoomObject<BaseObjectConfig>, bool>)((IRoomObject<BaseObjectConfig> obj) => decoration.HouseObjectID.Equals(obj.Config.ID))), (Func<IRoomObject<BaseObjectConfig>, Decoration>)((IRoomObject<BaseObjectConfig> _) => decoration)))), new IObservable<Decoration>[1] { Observable.Where<Decoration>((IObservable<Decoration>)val, (Func<Decoration, bool>)((Decoration _) => !_startFlow.IsLaunched)) }), (Func<Decoration, bool>)((Decoration _item) => (_item.State == EntityStatus.Rewarded) & _item.DisplayCondition.IsOpen.Value)), (Action<Decoration>)BuildDecoration, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)_disposables);
		IConnectableObservable<Decoration> val2 = Observable.Publish<Decoration>(Observable.SelectMany<Decoration, Decoration>(Observable.Merge<Decoration>(Observable.Where<Decoration>(Observable.ToObservable<Decoration>(_decorationManager.Collection), (Func<Decoration, bool>)((Decoration decor) => !decor.DisplayCondition.IsOpen.Value && decor.State == EntityStatus.Rewarded)), new IObservable<Decoration>[1] { Observable.Where<Decoration>(observable, (Func<Decoration, bool>)((Decoration decor) => !decor.DisplayCondition.IsOpen.Value)) }), (Func<Decoration, IObservable<Decoration>>)EmitUnlockedDecoration));
		IObservable<Decoration> observable4 = Observable.SelectMany<Decoration, Decoration>(Observable.Where<Decoration>((IObservable<Decoration>)val2, (Func<Decoration, bool>)((Decoration decor) => !_house.IsObjectSet(decor.HouseObjectID))), (Func<Decoration, IObservable<Decoration>>)EmitDecorationOnRoomCreated);
		IObservable<IGameRoomObject<BaseObjectConfig>> observable5 = Observable.Share<IGameRoomObject<BaseObjectConfig>>(Observable.Select<Decoration, IGameRoomObject<BaseObjectConfig>>(Observable.Merge<Decoration>(Observable.Where<Decoration>((IObservable<Decoration>)val2, (Func<Decoration, bool>)((Decoration decor) => _house.IsObjectSet(decor.HouseObjectID))), new IObservable<Decoration>[1] { observable4 }), (Func<Decoration, IGameRoomObject<BaseObjectConfig>>)((Decoration decor) => _house.GetRoomObject(decor.HouseObjectID))));
		IObservable<IGameRoomObject<BaseObjectConfig>> observable6 = Observable.Merge<IGameRoomObject<BaseObjectConfig>>(Observable.Where<IGameRoomObject<BaseObjectConfig>>(observable5, (Func<IGameRoomObject<BaseObjectConfig>, bool>)((IGameRoomObject<BaseObjectConfig> _) => !_startFlow.IsLaunched)), new IObservable<IGameRoomObject<BaseObjectConfig>>[1] { Observable.Where<IGameRoomObject<BaseObjectConfig>>(observable5, (Func<IGameRoomObject<BaseObjectConfig>, bool>)((IGameRoomObject<BaseObjectConfig> _) => _startFlow.IsLaunched)) });
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<IGameRoomObject<BaseObjectConfig>>(observable6, (Action<IGameRoomObject<BaseObjectConfig>>)delegate(IGameRoomObject<BaseObjectConfig> roomObject)
		{
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.Decoration));
			_startFlow.AddObjectToFlow(roomObject);
		}), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(IGameRoomObject<BaseObjectConfig>, bool)>(Observable.Where<(IGameRoomObject<BaseObjectConfig>, bool)>(Observable.CombineLatest<IGameRoomObject<BaseObjectConfig>, bool, (IGameRoomObject<BaseObjectConfig>, bool)>(observable6, _displayService.OnIndicatorPush(FilteredIndicatorType.Decoration), (Func<IGameRoomObject<BaseObjectConfig>, bool, (IGameRoomObject<BaseObjectConfig>, bool)>)((IGameRoomObject<BaseObjectConfig> _object, bool _indicator) => (_object: _object, _indicator: _indicator))), (Func<(IGameRoomObject<BaseObjectConfig>, bool), bool>)(((IGameRoomObject<BaseObjectConfig> _object, bool _indicator) _pair) => _pair._indicator)), (Action<(IGameRoomObject<BaseObjectConfig>, bool)>)delegate
		{
			_signalBus.TryFire<IndicatorSignals.PushRequest>(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.Decoration));
			_startFlow.TryShowFlow();
		}, (Action<Exception>)delegate(Exception ex)
		{
			throw ex.LogException();
		}), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)_disposables);
		DisposableExtensions.AddTo<IDisposable>(val2.Connect(), (ICollection<IDisposable>)_disposables);
	}

	private void BuildDecoration(Decoration decoration)
	{
		_builder.BuildRoomObject(decoration.HouseObjectID);
	}

	private IObservable<Decoration> EmitUnlockedDecoration(Decoration decoration)
	{
		return Observable.Select<bool, Decoration>(Observable.Where<bool>(Observable.Skip<bool>((IObservable<bool>)decoration.DisplayCondition.IsOpen, 1), (Func<bool, bool>)((bool x) => x)), (Func<bool, Decoration>)((bool _) => decoration));
	}

	private IObservable<Decoration> EmitDecorationOnRoomCreated(Decoration decoration)
	{
		return Observable.Select<IRoomObject<BaseObjectConfig>, Decoration>(Observable.Where<IRoomObject<BaseObjectConfig>>(Observable.SelectMany<Room, IRoomObject<BaseObjectConfig>>(_house.OnNew, (Func<Room, IEnumerable<IRoomObject<BaseObjectConfig>>>)((Room room) => room.RoomObjects)), (Func<IRoomObject<BaseObjectConfig>, bool>)((IRoomObject<BaseObjectConfig> roomObject) => roomObject.Config.ID == decoration.HouseObjectID)), (Func<IRoomObject<BaseObjectConfig>, Decoration>)((IRoomObject<BaseObjectConfig> _) => decoration));
	}

	public void Dispose()
	{
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Dispose();
		}
	}
}
