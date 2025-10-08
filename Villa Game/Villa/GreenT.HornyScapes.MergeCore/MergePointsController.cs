using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Events;
using GreenT.Types;
using Merge;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.MergeCore;

public class MergePointsController
{
	private readonly MergePointsManager _pointsManager;

	private readonly MergePointsCollector _pointsCollector;

	private readonly MergePointsEventManager _eventManager;

	public readonly IsDroppedLogics IsDroppedLogics = new IsDroppedLogics();

	public MergePointsController(LockerFactory lockerFactory, MergePointsIconService iconService, CollectCurrencyServiceForFly currencyServiceForFly, IAmplitudeSender<AmplitudeEvent> amplitude, MiniEventSettingsProvider miniEventSettingsProvider, MiniEventMapperManager miniEventMapperManager)
	{
		_eventManager = new MergePointsEventManager();
		_pointsManager = new MergePointsManager(lockerFactory, iconService, IsDroppedLogics, _eventManager, amplitude, miniEventSettingsProvider, miniEventMapperManager);
		_pointsCollector = new MergePointsCollector(iconService, currencyServiceForFly);
	}

	public void TryRemoveEventPoints(EventStructureType eventType, CompositeIdentificator id = default(CompositeIdentificator))
	{
		_eventManager.TryRemoveEventPoints(eventType, id);
	}

	public void Init()
	{
		Controller<MergeController>.Instance.OnStartMerge += OnStartMerge;
		Controller<MergeController>.Instance.OnMerge += ProcessItemAfterMerge;
		Controller<GameItemController>.Instance.StartFieldCreat.Subscribe(delegate
		{
			SubscribeToItemCreated();
		});
		Controller<GameItemController>.Instance.EndFieldCreat.Subscribe(delegate
		{
			UnsubscribeFromItemCreated();
		});
		Controller<GameItemController>.Instance.OnItemCreated += ProcessBubbleCreation;
		Controller<GameItemController>.Instance.OnItemTakenFromSomethere += ProcessItemFromSource;
	}

	private void SubscribeToItemCreated()
	{
		Controller<GameItemController>.Instance.OnItemCreated += ProcessNewField;
	}

	private void UnsubscribeFromItemCreated()
	{
		Controller<GameItemController>.Instance.OnItemCreated -= ProcessNewField;
	}

	private void OnStartMerge(GameItem from, GameItem to)
	{
		_pointsCollector.CollectMergePoints(from);
		_pointsCollector.CollectMergePoints(to);
	}

	private void ProcessBubbleCreation(GameItem item)
	{
		if (!IsInvalidItem(item) && item.HasBox(GIModuleType.Bubble) && HaveMergePoints(item, ignoreChance: true))
		{
			_pointsManager.InitializeMergePoints(item, MergePointsCreatType.Bubble);
		}
	}

	private void ProcessItemFromSource(GameItem item)
	{
		if (!IsInvalidItem(item) && _pointsManager.ValidateMergePointsModule(item))
		{
			_pointsManager.InitializeMergePoints(item, MergePointsCreatType.Skip);
		}
	}

	private void ProcessNewField(GameItem item)
	{
		if (!IsInvalidItem(item) && _pointsManager.ValidateMergePointsModule(item))
		{
			_pointsManager.InitializeMergePoints(item, MergePointsCreatType.Skip);
		}
	}

	private void ProcessItemAfterMerge(GameItem item)
	{
		if (!IsInvalidItem(item) && HaveMergePoints(item, ignoreChance: false))
		{
			_pointsManager.InitializeMergePoints(item, MergePointsCreatType.Merge);
		}
	}

	private bool HaveMergePoints(GameItem item, bool ignoreChance)
	{
		if (!_pointsManager.ValidateMergePointsModule(item))
		{
			return _pointsManager.TryAddMergePointsModule(item, ignoreChance);
		}
		return true;
	}

	private bool IsInvalidItem(GameItem item)
	{
		return item == null;
	}
}
