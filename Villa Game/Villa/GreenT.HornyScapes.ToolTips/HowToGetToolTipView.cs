using System.Linq;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class HowToGetToolTipView : AnimatedToolTipView<HowToGetToolTipSettings>
{
	public class Manager : MonoViewManager<HowToGetToolTipSettings, HowToGetToolTipView>
	{
	}

	[SerializeField]
	private LocalizedTextMeshPro localizedTitle;

	[SerializeField]
	private RectTransform tailBody;

	[SerializeField]
	private RectTransform tailHead;

	[Inject]
	private ItemGetView itemGetView;

	public override void Set(HowToGetToolTipSettings settings)
	{
		base.Set(settings);
		localizedTitle?.Init(settings.KeyText);
		InitTail(settings.TailSettings);
		itemGetView.Set(settings.HowToGet);
		if (settings.AdditionalWays.Any())
		{
			itemGetView.AddAdditionalWay(settings.AdditionalWays);
		}
	}

	private void InitTail(Tail settings)
	{
		RectTransform obj = ((settings.TailType == TailType.Body) ? tailBody : tailHead);
		obj.gameObject.SetActive(value: true);
		obj.anchoredPosition = settings.TailPosition;
		obj.localRotation = Quaternion.Euler(settings.TailRotation);
	}
}
