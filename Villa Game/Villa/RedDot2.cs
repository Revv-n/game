using DG.Tweening;
using UnityEngine;

public class RedDot2 : MonoBehaviour
{
	public Transform dot;

	public Transform target;

	public Transform target2;

	public float time;

	public float time2;

	private void Start()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Join(dot.DOScale(new Vector3(1.2f, 0.3f), time2));
		sequence.Append(dot.DOMove(target.position, time));
		sequence.Join(dot.DOScale(new Vector3(0.6f, 1f), time));
		sequence.Append(dot.DOScale(new Vector3(0.8f, 1f), time));
		sequence.Append(dot.DOMove(target2.position, time));
		sequence.Join(dot.DOScale(new Vector3(0.6f, 1f), time));
		sequence.Append(dot.DOScale(new Vector3(1.2f, 0.3f), time2));
		sequence.Append(dot.DOScale(0.75f, time));
		sequence.SetLoops(-1, LoopType.Restart);
	}

	private void Update()
	{
	}
}
