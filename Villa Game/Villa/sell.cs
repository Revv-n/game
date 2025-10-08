using DG.Tweening;
using UnityEngine;

public class sell : MonoBehaviour
{
	public Transform tr;

	public SpriteRenderer sr;

	public float time;

	public float max_size;

	private void Start()
	{
		Sequence s = DOTween.Sequence();
		s.Join(tr.DOScale(max_size, time));
		s.Append(tr.DOScale(0f, time));
	}

	private void Update()
	{
	}
}
