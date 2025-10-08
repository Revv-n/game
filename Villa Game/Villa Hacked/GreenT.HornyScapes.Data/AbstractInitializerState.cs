using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.HornyScapes.Data;

public abstract class AbstractInitializerState : IInitializerState, IDisposable
{
	protected ReactiveProperty<bool> isInitialized;

	protected IReadOnlyReactiveProperty<bool> isRequiredInitialized;

	public IReadOnlyReactiveProperty<bool> IsInitialized => (IReadOnlyReactiveProperty<bool>)(object)isInitialized;

	public IReadOnlyReactiveProperty<bool> IsRequiredInitialized => isRequiredInitialized;

	public AbstractInitializerState(IEnumerable<IStructureInitializer> others = null)
	{
		isInitialized = new ReactiveProperty<bool>();
		if (others != null && others.Any())
		{
			isRequiredInitialized = (IReadOnlyReactiveProperty<bool>)(object)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<bool>(Observable.Select<IList<bool>, bool>(Observable.CombineLatest<bool>((IEnumerable<IObservable<bool>>)others.Select((IStructureInitializer _other) => _other.IsInitialized)), (Func<IList<bool>, bool>)((IList<bool> _others) => _others.All((bool _isInitialized) => _isInitialized))), false);
		}
		else
		{
			isRequiredInitialized = (IReadOnlyReactiveProperty<bool>)(object)new ReactiveProperty<bool>(true);
		}
	}

	public virtual void Dispose()
	{
		isInitialized.Dispose();
		(isRequiredInitialized as IDisposable).Dispose();
	}
}
