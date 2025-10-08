using GreenT.HornyScapes.ToolTips;
using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public class MinieventCurrencyToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	protected MiniEventTaskCurrencyRewardItemView _sourceView;

	private CurrencyType _storedCurrencyType = CurrencyType.None;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (localizationKey == string.Empty)
		{
			localizationKey = "ui.hint.resource.";
		}
	}

	protected override void SetSettings()
	{
		if (_storedCurrencyType != _sourceView.CurrencyType)
		{
			base.Settings.KeyText = localizationKey + _sourceView.CurrencyType.ToString().ToLower() + $".{_sourceView.CompositeIdentificator[0]}";
			_storedCurrencyType = _sourceView.CurrencyType;
		}
	}
}
