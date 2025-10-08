using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.Model.Quest;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTasksRootViewController : BaseTabVariantsIEnumerableViewController<MiniEventTask, MiniEventTaskItemView, MiniEventTasksRootView, MiniEventSingleTasksRootView>
{
	public MiniEventTasksRootViewController(IManager<MiniEventTask> manager, IViewManager<IEnumerable<MiniEventTask>, MiniEventTasksRootView> viewManager, IViewManager<IEnumerable<MiniEventTask>, MiniEventSingleTasksRootView> singleViewManager)
		: base(manager, viewManager, singleViewManager)
	{
	}

	protected override IEnumerable<MiniEventTask> GetSources(CompositeIdentificator identificator)
	{
		return _manager.Collection.Where((MiniEventTask t) => t.Identificator == identificator && t.State != StateType.Rewarded);
	}
}
