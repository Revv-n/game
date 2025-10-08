using System;
using GreenT.HornyScapes.Content;
using Merge;
using StripClub.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTaskMergeRewardItemView : MiniEventTaskRewardItemView
{
	[SerializeField]
	private TMP_Text _count;

	[SerializeField]
	private Image _icon;

	public GIConfig GameItemConfig { get; private set; }

	public override void Set(LinkedContent source)
	{
		base.Set(source);
		if (base.Source is MergeItemLinkedContent mergeItemLinkedContent)
		{
			GameItemConfig = mergeItemLinkedContent.GameItemConfig;
			_icon.sprite = mergeItemLinkedContent.GetIcon();
			_count.text = $"{mergeItemLinkedContent.Quantity}";
			return;
		}
		throw new Exception().SendException($"{GetType().Name}: You're trying to display contet of type: {base.Source.GetType()} inside {GetType().Name} ! ");
	}
}
