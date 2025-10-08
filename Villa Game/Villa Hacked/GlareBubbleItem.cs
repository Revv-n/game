using DG.Tweening;
using UnityEngine;

public class GlareBubbleItem : MonoBehaviour
{
	public Transform tr;

	public SpriteRenderer sr;

	public float size;

	public float alpha;

	public float time;

	private void Start()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Join(tr.DOScale(size, time));
		sequence.Join(DOTweenModuleSprite.DOFade(sr, alpha, time));
		sequence.SetLoops(-1, LoopType.Yoyo);
	}

	private void Update()
	{
	}
}
