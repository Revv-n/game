using System;
using GreenT.Types;
using StripClub.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTaskCurrencyRewardItemView : MiniEventTaskRewardItemView
{
	[SerializeField]
	private TMP_Text _count;

	[SerializeField]
	private Image _icon;

	public CurrencyType CurrencyType { get; private set; }

	public CompositeIdentificator CompositeIdentificator { get; private set; }

	public override void Set(LinkedContent source)
	{
		base.Set(source);
		if (base.Source is CurrencyLinkedContent currencyLinkedContent)
		{
			switch (currencyLinkedContent.Currency)
			{
			case CurrencyType.Soft:
				_rewardIcon.Set(1);
				_backgroundFrame.Set(1);
				break;
			case CurrencyType.Hard:
				_rewardIcon.Set(2);
				_backgroundFrame.Set(2);
				break;
			case CurrencyType.Energy:
				_rewardIcon.Set(3);
				_backgroundFrame.Set(3);
				break;
			default:
				_icon.sprite = currencyLinkedContent.GetIcon();
				break;
			}
			CurrencyType = currencyLinkedContent.Currency;
			CompositeIdentificator = currencyLinkedContent.CompositeIdentificator;
			_count.text = $"{currencyLinkedContent.Quantity}";
			return;
		}
		throw new Exception().SendException($"{GetType().Name}: You're trying to display contet of type: {base.Source.GetType()} inside {GetType().Name} ! ");
	}
}
