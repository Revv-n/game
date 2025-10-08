namespace Merge;

public class AutoSpawnActionOperator : ModuleActionOperatorSimple
{
	public override GIModuleType Type => GIModuleType.AutoSpawn;

	public GIBox.AutoSpawn AutoSpawnBox { get; private set; }

	public override GIBox.Base GetBox()
	{
		return AutoSpawnBox;
	}

	public override void SetBox(GIBox.Base box)
	{
		AutoSpawnBox = box as GIBox.AutoSpawn;
		((IControlClocks)AutoSpawnBox).OnTimerTick += AtTimerTick;
		AutoSpawnBox = box as GIBox.AutoSpawn;
		block.Button.interactable = AutoSpawnBox.Data.TimerActive;
		block.SetButtonLabelText(AutoSpawnBox.SpeedUpPrice.ToString());
	}

	private void AtTimerTick(TimerStatus obj)
	{
		block.SetButtonLabelText(AutoSpawnBox.SpeedUpPrice.ToString());
	}
}
