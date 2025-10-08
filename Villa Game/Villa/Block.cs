using DG.Tweening;
using UnityEngine;

public class Block : MonoBehaviour
{
	public Transform tr;

	public SpriteRenderer sr;

	public float time;

	public float timeMax;

	private void Start()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Join(tr.DOScale(0.8f, timeMax));
		sequence.Append(tr.transform.DORotate(new Vector3(0f, 0f, -15f), time));
		sequence.Append(tr.transform.DORotate(new Vector3(0f, 0f, 15f), time));
		sequence.Append(tr.transform.DORotate(new Vector3(0f, 0f, -15f), time));
		sequence.Append(tr.transform.DORotate(new Vector3(0f, 0f, 0f), time));
		sequence.Append(tr.DOScale(0.7f, time));
		sequence.Append(tr.DOScale(0.7f, timeMax));
		sequence.SetLoops(-1, LoopType.Restart);
	}

	private void Update()
	{
	}
}
