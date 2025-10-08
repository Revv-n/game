using StripClub.UI;

namespace GreenT.HornyScapes.ToolTips;

public class SimpleToolTipOpener<TSettings, TView> : AbstractToolTipOpener<TSettings, TView> where TSettings : ToolTipSettings where TView : MonoView<TSettings>
{
	public virtual void Open()
	{
		if (base.IsPlaying.Value)
		{
			view.Set(base.Settings);
			open.OnNext(view);
		}
		else
		{
			base.IsPlaying.Value = true;
			view = viewManager.Display(base.Settings);
			open.OnNext(view);
		}
	}

	public virtual void Close()
	{
		if ((bool)view)
		{
			close.OnNext(view);
			base.IsPlaying.Value = false;
			view.Display(display: false);
			view = null;
		}
	}
}
