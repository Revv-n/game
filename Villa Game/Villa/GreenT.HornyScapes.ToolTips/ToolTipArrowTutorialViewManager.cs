using StripClub.UI;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipArrowTutorialViewManager : ViewManager<ToolTipArrowTutorialView>
{
	public ToolTipArrowTutorialView GetLast()
	{
		return views.FindLast((ToolTipArrowTutorialView _view) => _view.IsActive());
	}

	public ToolTipArrowTutorialView GetViewWithStep(int id)
	{
		return views.Find((ToolTipArrowTutorialView _view) => _view.Source.StepID == id);
	}
}
