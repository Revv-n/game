using UnityEngine;
using UnityEngine.EventSystems;

namespace GreenT.HornyScapes.ToolTips;

public class BankInfoToolTipOpener : ToolTipUIOpener<TailedToolTipSettings, BankInfoToolTip>
{
	[SerializeField]
	protected string localizationKey;

	public override void OnPointerClick(PointerEventData eventData)
	{
		SetSettings();
		base.OnPointerClick(eventData);
	}

	protected virtual void SetSettings()
	{
		base.Settings.KeyText = localizationKey;
	}
}
