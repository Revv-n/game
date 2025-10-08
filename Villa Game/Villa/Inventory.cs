using DG.Tweening;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public Transform tr;

	public SpriteRenderer sr;

	public float size;

	public float alpha;

	public float time;

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
