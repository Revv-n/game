using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public class MergeStoreConfigInitializer : LimitedContentConfigStructureInitializer<MergeStoreMapper, CreateData, CreateDataManager>
{
	public MergeStoreConfigInitializer(DataManagerCluster cluster, CreateDataFactory createDataFactory, IEnumerable<IStructureInitializer> others = null)
		: base((IDictionary<ContentType, CreateDataManager>)cluster, (IFactory<MergeStoreMapper, CreateData>)createDataFactory, others)
	{
	}
}
