using DG.Tweening;
using UnityEngine;

public class Objects : MonoBehaviour
{
	public Transform tr;

	public SpriteRenderer sr;

	public float time;

	public float alpha;

	public float size;

	private void Start()
	{
		Sequence s = DOTween.Sequence();
		s.Join(tr.DOScale(size, time));
		s.Join(sr.DOFade(alpha, time));
	}

	private void Update()
	{
	}
}
