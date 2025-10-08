using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.ToolTips;

public class SimpleToolTipOpener<TSettings, TView> : AbstractToolTipOpener<TSettings, TView> where TSettings : ToolTipSettings where TView : MonoView<TSettings>
{
	public virtual void Open()
	{
		if (base.IsPlaying.Value)
		{
			view.Set(base.Settings);
			((Subject<_003F>)(object)open).OnNext(view);
		}
		else
		{
			base.IsPlaying.Value = true;
			view = viewManager.Display(base.Settings);
			((Subject<_003F>)(object)open).OnNext(view);
		}
	}

	public virtual void Close()
	{
		if ((bool)view)
		{
			((Subject<_003F>)(object)close).OnNext(view);
			base.IsPlaying.Value = false;
			view.Display(display: false);
			view = null;
		}
	}
}
