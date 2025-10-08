using System;
using GreenT.HornyScapes.Meta.Decorations;
using StripClub.Model;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTaskDecorationRewardItemView : MiniEventTaskRewardItemView
{
	[SerializeField]
	private Image _icon;

	public int DecorationID { get; private set; }

	public override void Set(LinkedContent source)
	{
		base.Set(source);
		if (base.Source is DecorationLinkedContent decorationLinkedContent)
		{
			DecorationID = decorationLinkedContent.ID;
			_icon.sprite = decorationLinkedContent.GetIcon();
			return;
		}
		throw new Exception().SendException($"{GetType().Name}: You're trying to display contet of type: {base.Source.GetType()} inside {GetType().Name} ! ");
	}
}
