using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Merge;

public class UnboundWindowPopItem : MonoBehaviour
{
	[SerializeField]
	private int priority;

	[SerializeField]
	private float scale = 1.3f;

	[SerializeField]
	private float scaleTime = 0.18f;

	[SerializeField]
	private float color = 1f;

	[SerializeField]
	private float colorTime = 0.18f;

	public bool IsIgnored { get; set; }

	public int Priority
	{
		get
		{
			return priority;
		}
		set
		{
			priority = value;
		}
	}

	public float SizeTime => scaleTime;

	public Tween DoPop(bool isShow)
	{
		base.transform.SetScale((!isShow) ? 1 : 0);
		Sequence sequence = DOTween.Sequence();
		sequence.Append(base.transform.DOScale(scale, scaleTime).SetEase(Ease.OutSine));
		_ = (bool)GetComponent<Image>();
		sequence.Append(base.transform.DOScale(isShow ? 1 : 0, 0.18f).SetEase(Ease.InSine));
		return sequence;
	}
}
