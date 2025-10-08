using GreenT.HornyScapes.Events.Content;
using GreenT.Types;

namespace GreenT.HornyScapes.MergeStore;

public class MergeStoreService
{
	private readonly SectionCluster _cluster;

	private readonly ContentSelectorGroup _contentSelectorGroup;

	private ContentType Current => _contentSelectorGroup.Current;

	public MergeStoreService(SectionCluster cluster, ContentSelectorGroup contentSelectorGroup)
	{
		_cluster = cluster;
		_contentSelectorGroup = contentSelectorGroup;
	}

	public StorePreset GetSections()
	{
		return _cluster.GetPreset(Current);
	}
}
