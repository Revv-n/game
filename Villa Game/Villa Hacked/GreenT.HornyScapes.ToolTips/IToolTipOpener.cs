namespace GreenT.HornyScapes.ToolTips;

public interface IToolTipOpener<TSettings> where TSettings : ToolTipSettings
{
	void OpenToolTip(TSettings view);
}
