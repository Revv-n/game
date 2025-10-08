using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public class ScrollRectVelocityWithShadow : ScrollRectVelocityModifier
{
	[Tooltip("How many children fit without overlay view port")]
	[SerializeField]
	protected int childCountFit;

	[SerializeField]
	private Image shadow;

	protected override void Awake()
	{
		base.Awake();
		OnChildOverlay(Vector2.zero);
		ScrollRect.onValueChanged.AddListener(OnChildOverlay);
	}

	public bool IsContentOverlay()
	{
		return ScrollRect.content.childCount > childCountFit;
	}

	public void OnChildOverlay(Vector2 arg0)
	{
		if (!IsContentOverlay())
		{
			shadow.enabled = false;
		}
		else
		{
			shadow.enabled = ScrollRect.verticalNormalizedPosition > autoSnappingOffset;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ScrollRect.onValueChanged.RemoveAllListeners();
	}
}
