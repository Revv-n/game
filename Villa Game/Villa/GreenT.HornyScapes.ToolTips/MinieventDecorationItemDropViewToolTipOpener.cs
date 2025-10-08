using GreenT.HornyScapes.MiniEvents;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public sealed class MinieventDecorationItemDropViewToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	private MiniEventTaskDecorationRewardItemView _sourceView;

	private int _storedId;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (string.IsNullOrEmpty(localizationKey))
		{
			localizationKey = "ui.hint.decoration.";
		}
	}

	protected override void SetSettings()
	{
		if (_storedId != _sourceView.DecorationID)
		{
			base.Settings.KeyText = localizationKey + _sourceView.DecorationID;
			_storedId = _sourceView.DecorationID;
		}
	}
}
