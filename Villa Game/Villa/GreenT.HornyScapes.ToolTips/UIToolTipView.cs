using StripClub.UI;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class UIToolTipView : AnimatedToolTipView<ToolTipUISettings>
{
	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private TextMeshProUGUI title;

	[SerializeField]
	private LocalizedTextMeshPro localizedTextMeshProText;

	[SerializeField]
	private LocalizedTextMeshPro localizedTextMeshProName;

	[SerializeField]
	private RectTransform tailBody;

	[SerializeField]
	private RectTransform tailHead;

	public override void Set(ToolTipUISettings settings)
	{
		base.Set(settings);
		localizedTextMeshProText?.Init(settings.KeyText);
		localizedTextMeshProName?.Init(settings.KeyName);
		InitTail(settings.TailSettings);
	}

	public void SetArguments(params object[] arguments)
	{
		localizedTextMeshProText?.SetArguments(arguments);
	}

	private void InitTail(Tail settings)
	{
		tailBody.gameObject.SetActive(value: false);
		tailHead.gameObject.SetActive(value: false);
		RectTransform obj = ((settings.TailType == TailType.Body) ? tailBody : tailHead);
		obj.gameObject.SetActive(value: true);
		obj.anchoredPosition = settings.TailPosition;
		obj.localRotation = Quaternion.Euler(settings.TailRotation);
	}
}
