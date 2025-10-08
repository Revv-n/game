using GreenT.HornyScapes.ToolTips;

namespace GreenT.HornyScapes;

public class BattlePassLevelDropViewToolTipOpener : DropViewToolTipOpener
{
	protected override void OnValidate()
	{
		base.OnValidate();
		if (localizationKey == string.Empty)
		{
			localizationKey = "ui.hint.resource.bp_level";
		}
	}

	protected override void SetSettings()
	{
		base.Settings.KeyText = localizationKey;
	}
}
