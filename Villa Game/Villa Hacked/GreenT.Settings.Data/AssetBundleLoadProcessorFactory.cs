using System;
using System.Collections.Generic;
using StripClub.Model;
using Zenject;

namespace GreenT.Settings.Data;

public class AssetBundleLoadProcessorFactory : IFactory<AssetBundleLoadMapper, AssetBundleLoadProcessor>, IFactory
{
	private readonly LockerFactory _lockerFactory;

	private readonly DiContainer _container;

	private AssetBundleLoadProcessorFactory(LockerFactory lockerFactory, DiContainer container)
	{
		_lockerFactory = lockerFactory;
		_container = container;
	}

	public AssetBundleLoadProcessor Create(AssetBundleLoadMapper mapper)
	{
		ILocker locker = CreateLockers(mapper);
		AssetBundleLoadProcessor assetBundleLoadProcessor = _container.Instantiate<AssetBundleLoadProcessor>((IEnumerable<object>)new object[2] { mapper.bundle, locker });
		assetBundleLoadProcessor.Initialize();
		return assetBundleLoadProcessor;
	}

	private ILocker CreateLockers(AssetBundleLoadMapper mapper)
	{
		LockerExtension lockerExtension = new LockerExtension(_lockerFactory);
		ILocker[] array = null;
		try
		{
			array = lockerExtension.Create(mapper.unlock_type, mapper.unlock_value, LockerSourceType.Calendar);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException(GetType().Name + ": bundle = " + mapper.bundle + " has error in load lockers");
		}
		return new CompositeLocker(array);
	}
}
