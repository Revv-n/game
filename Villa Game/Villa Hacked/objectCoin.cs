using DG.Tweening;
using UnityEngine;

public class objectCoin : MonoBehaviour
{
	public Transform tr;

	public SpriteRenderer sr;

	public float time;

	public float size;

	public float max_size;

	private void Start()
	{
		Sequence s = DOTween.Sequence();
		s.Join(tr.DOScale(max_size, time));
		s.Join(DOTweenModuleSprite.DOFade(sr, 1f, time));
		s.Append(tr.DOScale(size, time));
	}

	private void Update()
	{
	}
}
