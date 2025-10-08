using DG.Tweening;
using UnityEngine;

public class BubbleObject : MonoBehaviour
{
	public Transform tr;

	public SpriteRenderer sr;

	public float time;

	public float size;

	public float alpha;

	private void Start()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Join(tr.DOScale(size, time));
		sequence.Join(sr.DOFade(alpha, time));
		sequence.SetLoops(-1, LoopType.Yoyo);
	}

	private void Update()
	{
	}
}
