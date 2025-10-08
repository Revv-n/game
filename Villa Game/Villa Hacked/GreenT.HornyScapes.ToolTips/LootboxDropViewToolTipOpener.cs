using GreenT.HornyScapes.UI;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class LootboxDropViewToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	private LootboxDropView _sourceView;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (localizationKey == string.Empty)
		{
			localizationKey = "ui.hint.lootbox.";
		}
	}

	protected override void SetSettings()
	{
		base.Settings.KeyText = localizationKey + _sourceView.Lootbox.Rarity.ToString().ToLower();
	}
}
