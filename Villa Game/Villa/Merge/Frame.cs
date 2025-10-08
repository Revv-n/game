using DG.Tweening;
using UnityEngine;

namespace Merge;

public class Frame : MonoBehaviour
{
	public Transform tr;

	public SpriteRenderer sr;

	public float size;

	public float alpha;

	public float time;

	public Tween SizeTween { get; private set; }

	private void OnEnable()
	{
		tr.localScale = Vector3.one;
		sr.SetAlpha(1f);
		Sequence sequence = DOTween.Sequence();
		sequence.Join(tr.DOScale(size, time));
		sequence.Join(sr.DOFade(alpha, time));
		sequence.SetLoops(-1, LoopType.Yoyo);
		SizeTween = sequence;
	}

	private void OnDisable()
	{
		SizeTween?.Kill();
	}
}
