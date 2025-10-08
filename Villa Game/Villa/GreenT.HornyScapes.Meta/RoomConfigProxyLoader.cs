using System;
using GreenT.AssetBundles;
using GreenT.HornyScapes.Meta.RoomObjects;
using GreenT.Settings;
using GreenT.Settings.Data;
using Merge.Meta.RoomObjects;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Meta;

public class RoomConfigProxyLoader : AssetBundleLoaderByType<int, RoomConfig>
{
	private readonly DiContainer container;

	public RoomConfigProxyLoader(DiContainer container, IAssetBundlesLoader assetBundlesLoader, IProjectSettings projectSettings)
		: base(assetBundlesLoader, projectSettings, BundleType.Room)
	{
		this.container = container;
	}

	public override IObservable<RoomConfig> Load(int bundleName)
	{
		return base.Load(bundleName).Do(InjectDependencies);
	}

	private void InjectDependencies(RoomConfig obj)
	{
		try
		{
			container.Inject(obj);
			for (int i = 0; i != obj.ObjectConfigs.Count; i++)
			{
				BaseObjectConfig baseObjectConfig = obj.ObjectConfigs[i];
				try
				{
					if (baseObjectConfig != null)
					{
						container.Inject(baseObjectConfig);
						continue;
					}
					throw new NullReferenceException("Room object #" + i + " is equals null");
				}
				catch (Exception innerException)
				{
					throw new Exception("ObjectConfig id: " + baseObjectConfig.ID.ToString() + " inject exception", innerException);
				}
			}
		}
		catch (Exception innerException2)
		{
			throw new Exception("RoomConfig id: " + obj.RoomID + " inject exception", innerException2);
		}
	}
}
