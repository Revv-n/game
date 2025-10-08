using Merge;
using StripClub.Model.Shop.UI;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class BigMergeItemViewToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	[Tooltip("Spawner")]
	private string additionalKey;

	[SerializeField]
	private MergeItemDropCardBigView sourceView;

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
		GIConfig source = sourceView.Source;
		if (source != null && storedId != source.UniqId)
		{
			bool flag = source.HasModule(GIModuleType.ClickSpawn);
			base.Settings.KeyText = (flag ? (additionalKey + source.Key.Collection.ToLower()) : localizationKey);
			storedId = source.UniqId;
		}
	}
}
