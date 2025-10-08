namespace GreenT.HornyScapes.ToolTips;

public interface IToolTip<in T> where T : ToolTipSettings
{
	void Set(T settings);
}
