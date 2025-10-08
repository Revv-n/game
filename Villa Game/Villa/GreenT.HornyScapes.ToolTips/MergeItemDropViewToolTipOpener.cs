using GreenT.HornyScapes.Bank.UI;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class MergeItemDropViewToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	[Tooltip("Spawner")]
	private string additionalKey;

	[SerializeField]
	private MergeItemDropView sourceView;

	private int storedId;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (string.IsNullOrEmpty(localizationKey))
		{
			localizationKey = "ui.hint.item";
		}
		if (string.IsNullOrEmpty(additionalKey))
		{
			additionalKey = "ui.hint.spawner.";
		}
	}

	protected override void SetSettings()
	{
		if (storedId != sourceView.Source.UniqId)
		{
			bool flag = sourceView.Source.HasModule(GIModuleType.ClickSpawn);
			base.Settings.KeyText = (flag ? (additionalKey + sourceView.Source.Key.Collection.ToLower()) : localizationKey);
			storedId = sourceView.Source.UniqId;
		}
	}
}
