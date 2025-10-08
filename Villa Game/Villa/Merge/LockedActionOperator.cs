namespace Merge;

public class LockedActionOperator : ModuleActionOperatorSimple
{
	public override GIModuleType Type => GIModuleType.Locked;

	public GIBox.Locked LockedBox { get; private set; }

	public override GIBox.Base GetBox()
	{
		return LockedBox;
	}

	public override void SetBox(GIBox.Base box)
	{
		LockedBox = box as GIBox.Locked;
		bool flag = !LockedBox.Data.BlocksMerge;
		bool flag2 = LockedBox.Data.UnlockPrice >= 0;
		base.gameObject.SetActive(flag && flag2);
		block.SetButtonLabelText(LockedBox.Data.UnlockPrice.ToString());
	}
}
