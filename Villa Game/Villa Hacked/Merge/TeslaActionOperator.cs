namespace Merge;

public class TeslaActionOperator : ModuleActionOperatorSimple
{
	public override GIModuleType Type => GIModuleType.Tesla;

	public GIBox.Tesla TeslaBox { get; private set; }

	public override GIBox.Base GetBox()
	{
		return TeslaBox;
	}

	public override void SetBox(GIBox.Base box)
	{
		if (TeslaBox != null)
		{
			TeslaBox.OnTimerTick -= AtTimerTick;
			TeslaBox.OnTimerComplete -= AtTimerComplete;
		}
		TeslaBox = box as GIBox.Tesla;
		block.Button.interactable = !TeslaBox.Data.Activated;
		block.SetButtonLabelText(Seconds.ToLabeledString(TeslaBox.Config.LifeTime));
		block.MainLabel.SetActive(!TeslaBox.Data.Activated);
		TeslaBox.OnTimerComplete += AtTimerComplete;
		TeslaBox.OnTimerTick += AtTimerTick;
	}

	protected override void AtButtonClick()
	{
		base.AtButtonClick();
		block.Button.interactable = false;
	}

	private void AtTimerTick(TimerStatus timerInfo)
	{
		block.SetButtonLabelText(Seconds.ToLabeledString(timerInfo.TimeLeft, 1));
	}

	private void AtTimerComplete(IControlClocks sender)
	{
		Deactivate();
	}

	public override void Deactivate()
	{
		base.Deactivate();
		if (TeslaBox != null)
		{
			TeslaBox.OnTimerTick -= AtTimerTick;
			TeslaBox.OnTimerComplete -= AtTimerComplete;
		}
		TeslaBox = null;
	}
}
