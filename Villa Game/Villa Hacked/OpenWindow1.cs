using DG.Tweening;
using UnityEngine;

public class OpenWindow1 : MonoBehaviour
{
	public Transform[] items;

	public float time;

	private void Start()
	{
		Sequence sequence = DOTween.Sequence();
		Transform[] array = items;
		foreach (Transform target in array)
		{
			sequence.Join(target.DOScale(1.3f, time));
			sequence.Append(target.DOScale(1f, time));
		}
		sequence.SetLoops(-1, LoopType.Yoyo);
	}

	private void Update()
	{
	}
}
