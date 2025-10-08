using GreenT.HornyScapes.ToolTips;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public class MinieventLootboxToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	protected MiniEventTaskLootboxRewardItemView _sourceView;

	private Rarity _storedRarity;

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
		base.Settings.KeyText = localizationKey + _sourceView.Rarity.ToString().ToLower();
		_storedRarity = _sourceView.Rarity;
	}
}
