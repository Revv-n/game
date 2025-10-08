using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.BattlePassRewardCards;
using GreenT.HornyScapes.Monetization;
using GreenT.Localizations;
using GreenT.UI;
using StripClub.Model.Shop;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;

public class EventBattlePassPurchaseView : MonoView
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

	private BattlePassMapperProvider _mapperProvider;

	private DiContainer _container;

	private ContentStorageProvider _storageProvider;

	private CalendarModel _eventCalendarModel;

	private BattlePassPurchaseViewAdapter _viewAdapter;

	[Inject]
	public void Constructor(LotManager lotManager, Purchaser purchaser, LocalizationService localizationService, BattlePassMapperProvider mapperProvider, DiContainer container, ContentStorageProvider storageProvider)
	{
		_purchaser = purchaser;
		_localizationService = localizationService;
		_lotManager = lotManager;
		_mapperProvider = mapperProvider;
		_container = container;
		_storageProvider = storageProvider;
	}

	public void Set(CalendarModel eventCalendarModel, BattlePass battlePass)
	{
		if (_eventCalendarModel != eventCalendarModel)
		{
			_eventCalendarModel = eventCalendarModel;
			_ = battlePass.Bundle;
			int[] array = _mapperProvider.GetEventMapper(battlePass.ID).any_lot_bought.Select(int.Parse).ToArray();
			BattlePassPurchaseViewAdapter prefab = ((array.Length == 1) ? _singleAdapterPrefab : _doubleAdapterPrefab);
			_viewAdapter = _container.InstantiatePrefabForComponent<BattlePassPurchaseViewAdapter>(prefab, _adaptersContainer);
			_viewAdapter.Init(_lotManager, _purchaser, _localizationService, _windowOpener, _storageProvider, battlePass, array);
		}
	}
}
