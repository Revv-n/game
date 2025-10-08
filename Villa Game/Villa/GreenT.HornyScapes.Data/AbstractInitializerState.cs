using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.HornyScapes.Data;

public abstract class AbstractInitializerState : IInitializerState, IDisposable
{
	protected ReactiveProperty<bool> isInitialized;

	protected IReadOnlyReactiveProperty<bool> isRequiredInitialized;

	public IReadOnlyReactiveProperty<bool> IsInitialized => isInitialized;

	public IReadOnlyReactiveProperty<bool> IsRequiredInitialized => isRequiredInitialized;

	public AbstractInitializerState(IEnumerable<IStructureInitializer> others = null)
	{
		isInitialized = new ReactiveProperty<bool>();
		if (others != null && others.Any())
		{
			isRequiredInitialized = (from _others in others.Select((IStructureInitializer _other) => _other.IsInitialized).CombineLatest()
				select _others.All((bool _isInitialized) => _isInitialized)).ToReadOnlyReactiveProperty(initialValue: false);
		}
		else
		{
			isRequiredInitialized = new ReactiveProperty<bool>(initialValue: true);
		}
	}

	public virtual void Dispose()
	{
		isInitialized.Dispose();
		(isRequiredInitialized as IDisposable).Dispose();
	}
}
