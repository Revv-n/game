using System.Linq;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Events;
using Merge;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.MergeCore;

public class MergePointsManager
{
	private readonly LockerFactory _lockerFactory;

	private readonly MergePointsIconService _iconService;

	private readonly IsDroppedLogics _isDroppedLogics;

	private readonly MergePointsEventManager _eventManager;

	private readonly IAmplitudeSender<AmplitudeEvent> _amplitude;

	private readonly MiniEventSettingsProvider _miniEventSettingsProvider;

	private readonly MiniEventMapperManager _miniEventMapperManager;

	public MergePointsManager(LockerFactory lockerFactory, MergePointsIconService iconService, IsDroppedLogics isDroppedLogics, MergePointsEventManager eventManager, IAmplitudeSender<AmplitudeEvent> amplitude, MiniEventSettingsProvider miniEventSettingsProvider, MiniEventMapperManager miniEventMapperManager)
	{
		_lockerFactory = lockerFactory;
		_iconService = iconService;
		_isDroppedLogics = isDroppedLogics;
		_eventManager = eventManager;
		_amplitude = amplitude;
		_miniEventSettingsProvider = miniEventSettingsProvider;
		_miniEventMapperManager = miniEventMapperManager;
	}

	public void InitializeMergePoints(GameItem item, MergePointsCreatType creatType)
	{
		ModuleConfigs.MergePoints.MergePointsCreateData mergePointsData = GetMergePointsData(item);
		if (mergePointsData == null)
		{
			RemoveMergePoints(item);
			return;
		}
		SendAmplitude(item, creatType, mergePointsData);
		CreateDisposableStream(mergePointsData, item);
		_iconService.SetIcon(item, mergePointsData.CurrencySelector);
	}

	private void SendAmplitude(GameItem item, MergePointsCreatType creatType, ModuleConfigs.MergePoints.MergePointsCreateData data)
	{
		if (creatType != 0)
		{
			MergePointsAmplitudeEvent analyticsEvent = new MergePointsAmplitudeEvent(item, data.PointsQty, data.CurrencySelector, creatType);
			_amplitude.AddEvent(analyticsEvent);
		}
	}

	public bool TryAddMergePointsModule(GameItem item, bool ignoreChance)
	{
		if (!item.Config.TryGetModule<ModuleConfigs.MergePoints>(out var result))
		{
			return false;
		}
		if (!TryGetActivePointsData(result, out var data))
		{
			return false;
		}
		if (!ShouldAddPoints(ignoreChance, data.PointsChance, result))
		{
			return false;
		}
		ModuleDatas.MergePoints item2 = new ModuleDatas.MergePoints(data.CurrencySelector.Identificator, data.PointsQty, data.CurrencySelector.Currency);
		item.Data.Modules.Add(item2);
		return true;
	}

	private bool TryGetActivePointsData(ModuleConfigs.MergePoints mergePointsConfig, out ModuleConfigs.MergePoints.MergePointsCreateData data)
	{
		InitAllLockers(mergePointsConfig);
		data = mergePointsConfig.CreateData.FirstOrDefault((ModuleConfigs.MergePoints.MergePointsCreateData x) => x.Locker.IsOpen.Value);
		return data != null;
	}

	private bool TryGetMiniEventAvailableProperty(ModuleConfigs.MergePoints mergePointsConfig, out ReactiveProperty<bool> property)
	{
		ModuleConfigs.MergePoints.MergePointsCreateData creatData = mergePointsConfig.CreateData.FirstOrDefault((ModuleConfigs.MergePoints.MergePointsCreateData x) => x.LockerTypes.Any((UnlockType unlockType) => unlockType == UnlockType.MinieventInProgress) && x.Locker.IsOpen.Value);
		return TryGetMiniEventAvailableProperty(creatData, out property);
	}

	private bool TryGetMiniEventAvailableProperty(ModuleConfigs.MergePoints.MergePointsCreateData creatData, out ReactiveProperty<bool> property)
	{
		property = null;
		if (creatData == null)
		{
			return false;
		}
		for (int i = 0; i < creatData.LockerTypes.Length; i++)
		{
			if (creatData.LockerTypes[i] == UnlockType.MinieventInProgress)
			{
				string bundleId = creatData.LockerValue[i];
				if (_miniEventMapperManager.TryGetIdForBundleInfo(bundleId, out var id))
				{
					MiniEvent @event = _miniEventSettingsProvider.GetEvent(id);
					property = @event.IsAnyContentAvailable;
					return true;
				}
			}
		}
		return false;
	}

	public ModuleConfigs.MergePoints.MergePointsCreateData GetMergePointsData(GameItem item)
	{
		if (!item.Config.TryGetModule<ModuleConfigs.MergePoints>(out var result))
		{
			return null;
		}
		if (!TryGetActivePointsData(result, out var data))
		{
			return null;
		}
		return data;
	}

	private void CreateDisposableStream(ModuleConfigs.MergePoints.MergePointsCreateData data, GameItem item)
	{
		InitLocker(data);
		CompositeDisposable disposable = new CompositeDisposable();
		if (TryGetMiniEventAvailableProperty(data, out var property) && property.Value)
		{
			property.Where((bool x) => !x).Take(1).Subscribe(delegate
			{
				disposable.Dispose();
				RemoveMergePoints(item);
			})
				.AddTo(disposable);
		}
		item.OnRemovingObservable.Take(1).Subscribe(delegate
		{
			disposable.Dispose();
		}).AddTo(disposable);
		data.Locker.IsOpen.Where((bool isOpen) => !isOpen).Take(1).Subscribe(delegate
		{
			disposable.Dispose();
			RemoveMergePoints(item);
		})
			.AddTo(disposable);
	}

	private void InitLocker(ModuleConfigs.MergePoints.MergePointsCreateData data)
	{
		if (data.Locker == null)
		{
			CompositeLocker locker = LockerFactory.CreateFromParamsArray(data.LockerTypes, data.LockerValue, _lockerFactory, LockerSourceType.MergeMiniEvent);
			data.SetLocker(locker);
		}
	}

	private void InitAllLockers(ModuleConfigs.MergePoints mergePointsConfig)
	{
		ModuleConfigs.MergePoints.MergePointsCreateData[] createData = mergePointsConfig.CreateData;
		foreach (ModuleConfigs.MergePoints.MergePointsCreateData data in createData)
		{
			InitLocker(data);
		}
	}

	private bool ShouldAddPoints(bool ignoreChance, int pointsChance, ModuleConfigs.MergePoints mergePointsConfig)
	{
		if (TryGetMiniEventAvailableProperty(mergePointsConfig, out var property) && !property.Value)
		{
			return false;
		}
		if (!ignoreChance)
		{
			return _isDroppedLogics.IsDropped(pointsChance);
		}
		return true;
	}

	public static void RemoveMergePoints(GameItem item)
	{
		item.RemoveModule(GIModuleType.MergePoints);
		item.MergePointsCase.Deactivate();
	}

	public bool ValidateMergePointsModule(GameItem item)
	{
		if (!TryGetMergePointsModule(item, out var mergePoints))
		{
			return false;
		}
		if (!item.Config.TryGetModule<ModuleConfigs.MergePoints>(out var result))
		{
			return false;
		}
		InitAllLockers(result);
		ModuleConfigs.MergePoints.MergePointsCreateData mergePointsCreateData = result.CreateData.FirstOrDefault((ModuleConfigs.MergePoints.MergePointsCreateData x) => x.Locker.IsOpen.Value);
		if (mergePointsCreateData != null)
		{
			if (!IsSameCurrency(mergePointsCreateData, mergePoints))
			{
				RemoveMergePoints(item);
				return false;
			}
			if (IsSameCurrency(mergePointsCreateData, mergePoints))
			{
				return true;
			}
		}
		else
		{
			ModuleConfigs.MergePoints.MergePointsCreateData mergePointsCreateData2 = result.CreateData.FirstOrDefault((ModuleConfigs.MergePoints.MergePointsCreateData x) => IsSameCurrency(x, mergePoints));
			if (mergePointsCreateData2 != null)
			{
				SetupCurrencyActivationWatcher(item, result, mergePointsCreateData2);
			}
		}
		RemoveMergePoints(item);
		return false;
	}

	private void SetupCurrencyActivationWatcher(GameItem item, ModuleConfigs.MergePoints pointsConfig, ModuleConfigs.MergePoints.MergePointsCreateData targetData)
	{
		CompositeDisposable disposable = new CompositeDisposable();
		targetData.Locker.IsOpen.Where((bool isOpen) => isOpen).Take(1).Subscribe(delegate
		{
			TryAddMergePointsModule(item, ignoreChance: true);
			InitializeMergePoints(item, MergePointsCreatType.Skip);
			disposable.Dispose();
		})
			.AddTo(disposable);
		_eventManager.OnEventEnd.Where((MergePointsEventManager.EventInfo info) => IsSameCurrency(targetData, info)).Take(1).Subscribe(delegate
		{
			disposable.Dispose();
		})
			.AddTo(disposable);
		ModuleConfigs.MergePoints.MergePointsCreateData[] createData = pointsConfig.CreateData;
		foreach (ModuleConfigs.MergePoints.MergePointsCreateData mergePointsCreateData in createData)
		{
			if (mergePointsCreateData != targetData)
			{
				mergePointsCreateData.Locker.IsOpen.Where((bool isOpen) => isOpen).Take(1).Subscribe(delegate
				{
					disposable.Dispose();
				})
					.AddTo(disposable);
			}
		}
		item.OnRemovingObservable.Take(1).Subscribe(delegate
		{
			disposable.Dispose();
		}).AddTo(disposable);
	}

	public static bool TryGetMergePointsModule(GameItem item, out ModuleDatas.MergePoints mergePoints)
	{
		return item.Data.TryGetModule<ModuleDatas.MergePoints>(out mergePoints);
	}

	private static bool IsSameCurrency(ModuleConfigs.MergePoints.MergePointsCreateData targetData, MergePointsEventManager.EventInfo x)
	{
		if (x.CurrencyType == targetData.CurrencySelector.Currency)
		{
			return x.ID == targetData.CurrencySelector.Identificator;
		}
		return false;
	}

	private static bool IsSameCurrency(ModuleConfigs.MergePoints.MergePointsCreateData data, ModuleDatas.MergePoints mergePoints)
	{
		if (data.CurrencySelector.Currency == mergePoints.CurrencyType)
		{
			return data.CurrencySelector.Identificator == mergePoints.Identificator;
		}
		return false;
	}
}
