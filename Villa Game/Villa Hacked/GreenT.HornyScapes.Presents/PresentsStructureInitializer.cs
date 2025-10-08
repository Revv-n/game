using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Presents.Models;
using GreenT.HornyScapes.Presents.Services;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Presents;

public class PresentsStructureInitializer : StructureInitializerViaArray<PresentsMapper, Present>
{
	private readonly PresentsBundleLoadService _presentsBundleLoadService;

	public PresentsStructureInitializer(PresentsBundleLoadService presentsBundleLoadService, IManager<Present> manager, IFactory<PresentsMapper, Present> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
		_presentsBundleLoadService = presentsBundleLoadService;
	}

	public override IObservable<bool> Initialize(IEnumerable<PresentsMapper> mappers)
	{
		return Observable.Select<Unit, bool>(Observable.ContinueWith<bool, Unit>(base.Initialize(mappers), _presentsBundleLoadService.LoadBundles()), (Func<Unit, bool>)((Unit _) => isInitialized.Value));
	}
}
