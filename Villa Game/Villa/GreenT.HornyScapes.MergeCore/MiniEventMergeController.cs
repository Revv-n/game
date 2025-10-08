using System;
using System.Collections.Generic;
using GreenT.HornyScapes.MiniEvents;
using Merge;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public abstract class MiniEventMergeController<T> : Controller<T>, IDisposable where T : Controller<T>
{
	protected MiniEventMergeItemDispenser _mergeItemDispenser;

	protected CompositeDisposable _trackStream = new CompositeDisposable();

	[Inject]
	private void Construct(MiniEventMergeItemDispenser miniEventMergeItemDispenser)
	{
		_mergeItemDispenser = miniEventMergeItemDispenser;
	}

	public abstract void RestoreItems(IEnumerable<GIData> items);

	protected bool IsMiniEventCurrencyItem(GIConfig config)
	{
		ModuleConfigs.Collect module = config.GetModule<ModuleConfigs.Collect>();
		if (module == null)
		{
			return false;
		}
		if (!module.TryGetCurrencyParams(out var currencyParams))
		{
			return false;
		}
		if (currencyParams.CurrencyType != CurrencyType.MiniEvent)
		{
			return false;
		}
		return true;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Dispose();
	}

	public void Dispose()
	{
		_trackStream.Dispose();
	}
}
