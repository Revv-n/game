using GreenT.HornyScapes.MiniEvents;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public sealed class MinieventMergeItemDropViewToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	[Tooltip("Spawner")]
	private string _additionalKey;

	[SerializeField]
	private MiniEventTaskMergeRewardItemView _sourceView;

	private int _storedId;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (string.IsNullOrEmpty(localizationKey))
		{
			localizationKey = "ui.hint.item";
		}
		if (string.IsNullOrEmpty(_additionalKey))
		{
			_additionalKey = "ui.hint.spawner.";
		}
	}

	protected override void SetSettings()
	{
		if (_storedId != _sourceView.GameItemConfig.UniqId)
		{
			bool flag = _sourceView.GameItemConfig.HasModule(GIModuleType.ClickSpawn);
			base.Settings.KeyText = (flag ? (_additionalKey + _sourceView.GameItemConfig.Key.Collection.ToLower()) : localizationKey);
			_storedId = _sourceView.GameItemConfig.UniqId;
		}
	}
}
