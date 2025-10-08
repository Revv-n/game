using DG.Tweening;
using UnityEngine;

public class TapItem : MonoBehaviour
{
	public Transform item;

	public SpriteRenderer itemsr;

	public float time;

	public float scale;

	public float max_size_time;

	public float min_scale;

	public float max_scale;

	public float min_size_time;

	private void Start()
	{
		Sequence s = DOTween.Sequence();
		s.Join(item.DOScale(min_scale, min_size_time));
		s.Append(item.DOScale(max_scale, max_size_time));
		s.Append(item.DOScale(scale, min_size_time));
		s.Append(item.DOScale(max_scale, max_size_time));
		s.Append(item.DOScale(scale, max_size_time));
	}

	private void Update()
	{
	}
}
