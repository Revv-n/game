using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerConfigInitializer : LimitedContentConfigStructureInitializer<BannerMapper, CreateData, CreateDataManager>
{
	public BannerConfigInitializer(DataManagerCluster cluster, CreateDataFactory createDataFactory, IEnumerable<IStructureInitializer> others = null)
		: base((IDictionary<ContentType, CreateDataManager>)cluster, (IFactory<BannerMapper, CreateData>)createDataFactory, others)
	{
	}
}
