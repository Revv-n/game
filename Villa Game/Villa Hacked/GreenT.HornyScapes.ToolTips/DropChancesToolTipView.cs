using StripClub.UI;
using StripClub.UI.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class DropChancesToolTipView : AnimatedToolTipView<DropChanceToolTipSettings>
{
	public class Manager : MonoViewManager<DropChanceToolTipSettings, DropChancesToolTipView>
	{
	}

	[SerializeField]
	private LocalizedTextMeshPro localizedTitle;

	[SerializeField]
	private RectTransform tailBody;

	[SerializeField]
	private RectTransform tailHead;

	private RarityChancesView chancesView;

	[Inject]
	private void Init(RarityChancesView chancesView)
	{
		this.chancesView = chancesView;
	}

	public override void Set(DropChanceToolTipSettings settings)
	{
		base.Set(settings);
		localizedTitle?.Init(settings.KeyText);
		InitTail(settings.TailSettings);
		chancesView.Set(settings.Chances);
	}

	private void InitTail(Tail settings)
	{
		RectTransform obj = ((settings.TailType == TailType.Body) ? tailBody : tailHead);
		obj.gameObject.SetActive(value: true);
		obj.anchoredPosition = settings.TailPosition;
		obj.localRotation = Quaternion.Euler(settings.TailRotation);
	}
}
