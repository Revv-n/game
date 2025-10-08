using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.HornyScapes.Data;

public class CombinedStructureInitializer : IStructureInitializer, IInitializerState, IDisposable
{
	protected ReadOnlyReactiveProperty<bool> isInitialized;

	protected IReadOnlyReactiveProperty<bool> isRequiredInitialized;

	public IReadOnlyReactiveProperty<bool> IsInitialized => (IReadOnlyReactiveProperty<bool>)(object)isInitialized;

	public IReadOnlyReactiveProperty<bool> IsRequiredInitialized => isRequiredInitialized;

	public IEnumerable<IStructureInitializer> StructureInitializers { get; }

	public CombinedStructureInitializer(IEnumerable<IStructureInitializer> structureInitializers)
	{
		isInitialized = ReactivePropertyExtensions.ToReadOnlyReactiveProperty<bool>(Observable.Select<IList<bool>, bool>(Observable.CombineLatest<bool>((IEnumerable<IObservable<bool>>)structureInitializers.Select((IStructureInitializer _initializer) => _initializer.IsInitialized)), (Func<IList<bool>, bool>)((IList<bool> _initStates) => _initStates.All((bool _isInited) => _isInited))), false);
		bool flag = structureInitializers.All((IStructureInitializer _initializer) => _initializer.IsRequiredInitialized.Value);
		isRequiredInitialized = (IReadOnlyReactiveProperty<bool>)(object)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<bool>(Observable.Select<IList<bool>, bool>(Observable.CombineLatest<bool>((IEnumerable<IObservable<bool>>)structureInitializers.Select((IStructureInitializer _initializer) => _initializer.IsRequiredInitialized)), (Func<IList<bool>, bool>)((IList<bool> _requiredStates) => _requiredStates.All((bool _isInited) => _isInited))), flag);
		StructureInitializers = structureInitializers;
	}

	public IObservable<bool> Initialize()
	{
		return Observable.Merge<bool>(StructureInitializers.Select((IStructureInitializer _initializer) => _initializer.Initialize()));
	}

	public void Dispose()
	{
		isInitialized.Dispose();
		(isRequiredInitialized as IDisposable).Dispose();
	}
}
