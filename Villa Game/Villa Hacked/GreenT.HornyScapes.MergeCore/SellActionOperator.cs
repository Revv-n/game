using System;
using Merge;

namespace GreenT.HornyScapes.MergeCore;

public class SellActionOperator : ModuleActionOperatorSimple
{
	[Serializable]
	public struct CountInfo
	{
		public int count;

		public int treshold;
	}

	public override GIModuleType Type => GIModuleType.Sell;

	public GIBox.Sell SellBox { get; private set; }

	public override GIBox.Base GetBox()
	{
		return SellBox;
	}

	protected override void Init()
	{
		Controller<SellController>.Instance.OnSellImportant += AtSellImportant;
	}

	public override void SetBox(GIBox.Base box)
	{
		SellBox = box as GIBox.Sell;
		block.SetButtonLabelText(SellBox.Config.Price.ToString());
		base.IsActive = true;
	}

	private void AtSellImportant(GameItem gi)
	{
		if (gi.TryGetBox<GIBox.Sell>(out var _))
		{
			Controller<SelectionController>.Instance.ClearSelection();
		}
	}
}
