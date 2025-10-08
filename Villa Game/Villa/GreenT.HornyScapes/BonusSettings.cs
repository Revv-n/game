using System;
using GreenT.HornyScapes.ToolTips;
using UnityEngine;

namespace GreenT.HornyScapes;

[Serializable]
public class BonusSettings
{
	[SerializeField]
	private Sprite bonusSprite;

	[SerializeField]
	private ToolTipUISettings bonusToolTipSettings;

	public Sprite BonusSprite => bonusSprite;

	public ToolTipUISettings BonusToolTipSettings => bonusToolTipSettings;
}
