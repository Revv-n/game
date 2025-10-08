using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.Analytics;
using Merge;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class CollectController : Controller<CollectController>, IClickInteractionController, IModuleController, IMasterController, IGameExitListener
{
	private CollectCurrencyServiceForFly _collectCurrencyService;

	private Tween _tweenSpeedUp;

	private ModuleConfigs.Collect.SpeedUpParams _speedUpParams;

	private GameItemController Field => Controller<GameItemController>.Instance;

	GIModuleType IModuleController.ModuleType => GIModuleType.Collect;

	public event Action<GameItem> OnCollect;

	[Inject]
	private void InnerInit(CollectCurrencyServiceForFly collectCurrencyService)
	{
		_collectCurrencyService = collectCurrencyService;
	}

	ClickResult IClickInteractionController.Interact(GameItem gameItem)
	{
		ModuleConfigs.Collect module = gameItem.Config.GetModule<ModuleConfigs.Collect>();
		if (module.CollectableType == CollectableType.Currency)
		{
			ModuleConfigs.Collect.CurrencyParams currencyParams = module.Parametres as ModuleConfigs.Collect.CurrencyParams;
			_collectCurrencyService.Collect(gameItem, currencyParams.CurrencyType, currencyParams.Amount, CurrencyAmplitudeAnalytic.SourceType.MergeCollect, currencyParams.CompositeIdentificator);
		}
		else
		{
			Debug.Log($"Not impimented realization: {module.CollectableType}");
		}
		this.OnCollect?.Invoke(gameItem);
		Field.RemoveItem(gameItem);
		Merge.Sounds.Play("collect");
		return ClickResult.Deselect;
	}

	void IMasterController.LinkControllers(IList<BaseController> controllers)
	{
		controllers.OfType<ITimeBoostListener>().ToList();
	}

	void IGameExitListener.BeforeExit()
	{
		_tweenSpeedUp?.Kill(complete: true);
	}
}
