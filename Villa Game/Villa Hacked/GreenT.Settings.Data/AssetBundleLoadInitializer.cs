using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.Settings.Data;

public class AssetBundleLoadInitializer : StructureInitializerViaArray<AssetBundleLoadMapper, AssetBundleLoadProcessor>
{
	public AssetBundleLoadInitializer(IManager<AssetBundleLoadProcessor> manager, IFactory<AssetBundleLoadMapper, AssetBundleLoadProcessor> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
