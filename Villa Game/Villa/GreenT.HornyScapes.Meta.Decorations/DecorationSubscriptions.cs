using System;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.DisplayStrategy;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
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
		IConnectableObservable<Decoration> connectableObservable = _decorationManager.OnUpdate.Publish();
		IObservable<Decoration> source = connectableObservable.Where((Decoration decor) => decor.State == EntityStatus.Rewarded);
		IObservable<Decoration> source2 = source.Where((Decoration decor) => decor.DisplayCondition.IsOpen.Value).Do(delegate(Decoration decoration)
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
		IObservable<Unit> observable = source2.SelectMany(rewardsWindow.OnClose.Take(1));
		source2.SelectMany(rewardsWindow.OnCloseWithLootbox.Take(1)).AsUnitObservable().Merge(observable)
			.Subscribe(delegate
			{
				try
				{
					_startFlow.SnapToObjects();
				}
				catch (Exception exception)
				{
					exception.LogException();
				}
			})
			.AddTo(_disposables);
		(from _item in connectableObservable.Where((Decoration _) => _startFlow.IsLaunched).SelectMany((Decoration decoration) => from obj in _startFlow.OnSnapedTo
				where decoration.HouseObjectID.Equals(obj.Config.ID)
				select obj into _
				select decoration).Merge(connectableObservable.Where((Decoration _) => !_startFlow.IsLaunched))
			where (_item.State == EntityStatus.Rewarded) & _item.DisplayCondition.IsOpen.Value
			select _item).Subscribe(BuildDecoration, delegate(Exception ex)
		{
			ex.LogException();
		}).AddTo(_disposables);
		IConnectableObservable<Decoration> connectableObservable2 = (from decor in _decorationManager.Collection.ToObservable()
			where !decor.DisplayCondition.IsOpen.Value && decor.State == EntityStatus.Rewarded
			select decor).Merge(source.Where((Decoration decor) => !decor.DisplayCondition.IsOpen.Value)).SelectMany((Func<Decoration, IObservable<Decoration>>)EmitUnlockedDecoration).Publish();
		IObservable<Decoration> observable2 = connectableObservable2.Where((Decoration decor) => !_house.IsObjectSet(decor.HouseObjectID)).SelectMany((Func<Decoration, IObservable<Decoration>>)EmitDecorationOnRoomCreated);
		IObservable<IGameRoomObject<BaseObjectConfig>> source3 = (from decor in connectableObservable2.Where((Decoration decor) => _house.IsObjectSet(decor.HouseObjectID)).Merge(observable2)
			select _house.GetRoomObject(decor.HouseObjectID)).Share();
		IObservable<IGameRoomObject<BaseObjectConfig>> observable3 = source3.Where((IGameRoomObject<BaseObjectConfig> _) => !_startFlow.IsLaunched).Merge(source3.Where((IGameRoomObject<BaseObjectConfig> _) => _startFlow.IsLaunched));
		observable3.Subscribe(delegate(IGameRoomObject<BaseObjectConfig> roomObject)
		{
			_signalBus.TryFire(new IndicatorSignals.PushRequest(status: true, FilteredIndicatorType.Decoration));
			_startFlow.AddObjectToFlow(roomObject);
		}).AddTo(_disposables);
		(from _pair in observable3.CombineLatest(_displayService.OnIndicatorPush(FilteredIndicatorType.Decoration), (IGameRoomObject<BaseObjectConfig> _object, bool _indicator) => (_object: _object, _indicator: _indicator))
			where _pair._indicator
			select _pair).Subscribe(delegate
		{
			_signalBus.TryFire(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.Decoration));
			_startFlow.TryShowFlow();
		}, delegate(Exception ex)
		{
			throw ex.LogException();
		}).AddTo(_disposables);
		connectableObservable.Connect().AddTo(_disposables);
		connectableObservable2.Connect().AddTo(_disposables);
	}

	private void BuildDecoration(Decoration decoration)
	{
		_builder.BuildRoomObject(decoration.HouseObjectID);
	}

	private IObservable<Decoration> EmitUnlockedDecoration(Decoration decoration)
	{
		return from x in decoration.DisplayCondition.IsOpen.Skip(1)
			where x
			select x into _
			select decoration;
	}

	private IObservable<Decoration> EmitDecorationOnRoomCreated(Decoration decoration)
	{
		return from roomObject in _house.OnNew.SelectMany((Room room) => room.RoomObjects)
			where roomObject.Config.ID == decoration.HouseObjectID
			select roomObject into _
			select decoration;
	}

	public void Dispose()
	{
		_disposables?.Dispose();
	}
}
