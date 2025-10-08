using System;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Monetization;
using GreenT.Localizations;
using GreenT.UI;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events.BattlePassRewardCards;

public class BattlePassPurchaseView : MonoView<CalendarModel>
{
	[SerializeField]
	private WindowOpener _windowOpener;

	[SerializeField]
	private Transform _adaptersContainer;

	[SerializeField]
	private BattlePassPurchaseViewAdapter _singleAdapterPrefab;

	[SerializeField]
	private BattlePassPurchaseViewAdapter _doubleAdapterPrefab;

	private Purchaser _purchaser;

	private LocalizationService _localizationService;

	private LotManager _lotManager;

	private BattlePassProvider _provider;

	private BattlePassStateService _stateService;

	private DiContainer _container;

	private ContentStorageProvider _storageProvider;

	private BattlePassPurchaseViewAdapter _viewAdapter;

	private IDisposable _providerStream;

	[Inject]
	public void Constructor(LotManager lotManager, Purchaser purchaser, LocalizationService localizationService, BattlePassProvider provider, BattlePassStateService stateService, DiContainer container, ContentStorageProvider storageProvider)
	{
		_purchaser = purchaser;
		_localizationService = localizationService;
		_lotManager = lotManager;
		_provider = provider;
		_stateService = stateService;
		_container = container;
		_storageProvider = storageProvider;
	}

	private void Awake()
	{
		Initialization();
	}

	private void Initialization()
	{
		if (_stateService.HaveActiveBattlePass())
		{
			ApplyData(_provider.CalendarChangeProperty.Value);
		}
		_providerStream = _provider.CalendarChangeProperty.Where(((CalendarModel calendar, BattlePass battlePass) tuple) => tuple.battlePass != null).Subscribe(ApplyData);
	}

	private void ApplyData((CalendarModel calendar, BattlePass battlePass) tuple)
	{
		if (tuple.calendar.EventMapper is BattlePassMapper battlePassMapper)
		{
			int[] array = battlePassMapper.any_lot_bought.Select(int.Parse).ToArray();
			BattlePassPurchaseViewAdapter prefab = ((array.Length == 1) ? _singleAdapterPrefab : _doubleAdapterPrefab);
			BattlePass item = tuple.battlePass;
			if (_viewAdapter != null)
			{
				UnityEngine.Object.Destroy(_viewAdapter.gameObject);
			}
			_viewAdapter = _container.InstantiatePrefabForComponent<BattlePassPurchaseViewAdapter>(prefab, _adaptersContainer);
			_viewAdapter.Init(_lotManager, _purchaser, _localizationService, _windowOpener, _storageProvider, item, array);
		}
	}

	private void OnDestroy()
	{
		_providerStream?.Dispose();
	}
}
