using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using UniRx;

namespace GreenT.HornyScapes.MergeField;

public class MergeFieldInitializer : IStructureInitializer<IEnumerable<MergeFieldMapper>>, IInitializerState
{
	private readonly IManager<MergeFieldMapper> _manager;

	public IReadOnlyReactiveProperty<bool> IsInitialized { get; } = (IReadOnlyReactiveProperty<bool>)(object)new ReactiveProperty<bool>(true);


	public IReadOnlyReactiveProperty<bool> IsRequiredInitialized { get; } = (IReadOnlyReactiveProperty<bool>)(object)new ReactiveProperty<bool>(false);


	public MergeFieldInitializer(IManager<MergeFieldMapper> manager)
	{
		_manager = manager;
	}

	public IObservable<bool> Initialize(IEnumerable<MergeFieldMapper> mappers)
	{
		_manager.AddRange(mappers);
		return Observable.Return(true);
	}
}
