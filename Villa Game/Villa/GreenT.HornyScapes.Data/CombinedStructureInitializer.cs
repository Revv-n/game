using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.HornyScapes.Data;

public class CombinedStructureInitializer : IStructureInitializer, IInitializerState, IDisposable
{
	protected ReadOnlyReactiveProperty<bool> isInitialized;

	protected IReadOnlyReactiveProperty<bool> isRequiredInitialized;

	public IReadOnlyReactiveProperty<bool> IsInitialized => isInitialized;

	public IReadOnlyReactiveProperty<bool> IsRequiredInitialized => isRequiredInitialized;

	public IEnumerable<IStructureInitializer> StructureInitializers { get; }

	public CombinedStructureInitializer(IEnumerable<IStructureInitializer> structureInitializers)
	{
		isInitialized = (from _initStates in structureInitializers.Select((IStructureInitializer _initializer) => _initializer.IsInitialized).CombineLatest()
			select _initStates.All((bool _isInited) => _isInited)).ToReadOnlyReactiveProperty(initialValue: false);
		bool initialValue = structureInitializers.All((IStructureInitializer _initializer) => _initializer.IsRequiredInitialized.Value);
		isRequiredInitialized = (from _requiredStates in structureInitializers.Select((IStructureInitializer _initializer) => _initializer.IsRequiredInitialized).CombineLatest()
			select _requiredStates.All((bool _isInited) => _isInited)).ToReadOnlyReactiveProperty(initialValue);
		StructureInitializers = structureInitializers;
	}

	public IObservable<bool> Initialize()
	{
		return StructureInitializers.Select((IStructureInitializer _initializer) => _initializer.Initialize()).Merge();
	}

	public void Dispose()
	{
		isInitialized.Dispose();
		(isRequiredInitialized as IDisposable).Dispose();
	}
}
