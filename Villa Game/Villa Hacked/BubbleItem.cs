using DG.Tweening;
using UnityEngine;

public class BubbleItem : MonoBehaviour
{
	public Transform tr;

	public SpriteRenderer sr;

	public GameObject bubble;

	public float size;

	public float alpha;

	public float time;

	private void Start()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Join(tr.DOScale(size, time));
		sequence.Join(DOTweenModuleSprite.DOFade(sr, alpha, time));
		sequence.Join(bubble.transform.DORotate(new Vector3(0f, 0f, -55f), 1f));
		sequence.SetLoops(-1, LoopType.Yoyo);
	}

	private void Update()
	{
	}
}
