using System.Collections.Generic;
using System.Linq;
using GreenT.Types;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopRootViewControllerService : IViewController, IContentable
{
	private readonly ActivitiesShopManager _activitiesShopManager;

	private readonly MiniEventLotBoughtHandler _miniEventLotBoughtHandler;

	private readonly MiniEventShopRouletteSummonViewController _rouletteSummonViewController;

	private readonly Dictionary<ShopTabType, IShopViewController> _viewControllers;

	private readonly Dictionary<ShopTabType, IContentable> _contentables;

	private IShopViewController _lastViewController;

	public MiniEventShopRootViewControllerService(MiniEventLotBoughtHandler miniEventLotBoughtHandler, ActivitiesShopManager activitiesShopManager, MiniEventShopBundlesViewController miniEventShopBundlesViewController, MiniEventShopRouletteSummonViewController miniEventShopRouletteSummonViewController, MiniEventShopOfferViewController miniEventShopOfferViewController, MiniEventShopSummonViewController miniEventShopSummonViewController, MiniEventShop8SlotsViewController miniEventShop8SlotsViewController, MiniEventShopDoubleOfferViewController miniEventShopDoubleOfferViewController, MiniEventShopChainBundlesViewController miniEventShopChainBundlesViewController)
	{
		_viewControllers = new Dictionary<ShopTabType, IShopViewController>
		{
			{
				ShopTabType.BundlesList,
				miniEventShopBundlesViewController
			},
			{
				ShopTabType.Offer,
				miniEventShopOfferViewController
			},
			{
				ShopTabType.Summon,
				miniEventShopSummonViewController
			},
			{
				ShopTabType.EightSlots,
				miniEventShop8SlotsViewController
			},
			{
				ShopTabType.DoubleOffer,
				miniEventShopDoubleOfferViewController
			},
			{
				ShopTabType.ChainBundles,
				miniEventShopChainBundlesViewController
			}
		};
		_contentables = new Dictionary<ShopTabType, IContentable>
		{
			{
				ShopTabType.BundlesList,
				miniEventShopBundlesViewController
			},
			{
				ShopTabType.Roulette,
				miniEventShopRouletteSummonViewController
			},
			{
				ShopTabType.Summon,
				miniEventShopSummonViewController
			},
			{
				ShopTabType.Offer,
				miniEventShopOfferViewController
			},
			{
				ShopTabType.EightSlots,
				miniEventShop8SlotsViewController
			},
			{
				ShopTabType.DoubleOffer,
				miniEventShopDoubleOfferViewController
			},
			{
				ShopTabType.ChainBundles,
				miniEventShopChainBundlesViewController
			}
		};
		_activitiesShopManager = activitiesShopManager;
		_miniEventLotBoughtHandler = miniEventLotBoughtHandler;
		_rouletteSummonViewController = miniEventShopRouletteSummonViewController;
	}

	public void Show(CompositeIdentificator identificator, bool isMultiTabbed)
	{
		ActivityShopMapper activityShopMapper = _activitiesShopManager.Collection.FirstOrDefault((ActivityShopMapper activityShop) => activityShop.bank_tab_id == identificator[0]);
		if (!_viewControllers.TryGetValue(activityShopMapper.tab_type, out _lastViewController) && activityShopMapper.tab_type == ShopTabType.Roulette)
		{
			_rouletteSummonViewController.Show(identificator, isMultiTabbed);
			return;
		}
		_lastViewController.Show(identificator, isMultiTabbed);
		_miniEventLotBoughtHandler.SetupLots(_lastViewController.CurrentLots);
	}

	public void HideAll()
	{
		if (_lastViewController != null)
		{
			_lastViewController.HideAll();
		}
		if (_rouletteSummonViewController != null)
		{
			_rouletteSummonViewController.HideAll();
		}
	}

	public bool HasAnyContentAvailable(CompositeIdentificator identificator)
	{
		ActivityShopMapper activityShopMapper = _activitiesShopManager.Collection.FirstOrDefault((ActivityShopMapper activityShop) => activityShop.bank_tab_id == identificator[0]);
		_contentables.TryGetValue(activityShopMapper.tab_type, out var value);
		return value.HasAnyContentAvailable(identificator);
	}
}
