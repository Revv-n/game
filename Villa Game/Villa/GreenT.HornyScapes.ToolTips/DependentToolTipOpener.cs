using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.ToolTips;

public abstract class DependentToolTipOpener<TSettings, TView> : AbstractToolTipOpener<TSettings, TView>, IDependentToolTipOpener<TSettings> where TSettings : ToolTipSettings where TView : MonoView<TSettings>
{
	public ReactiveProperty<bool> ReadyToActivate { get; } = new ReactiveProperty<bool>();


	public virtual void Activate()
	{
		OpenToolTip(base.Settings);
		ReadyToActivate.Value = false;
	}

	public virtual void Deactivate()
	{
	}
}
