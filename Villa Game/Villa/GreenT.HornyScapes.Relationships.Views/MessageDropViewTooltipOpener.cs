using GreenT.HornyScapes.ToolTips;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Views;

public class MessageDropViewTooltipOpener : DropViewToolTipOpener
{
	[SerializeField]
	private MessageDropView _messageDropView;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (localizationKey == string.Empty)
		{
			localizationKey = "hint.date_message";
		}
	}
}
