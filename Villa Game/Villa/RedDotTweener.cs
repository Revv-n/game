using System.Collections;
using DG.Tweening;
using UnityEngine;

public class RedDotTweener : MonoBehaviour
{
	public Transform dot;

	public Transform a;

	public Transform b;

	public float time;

	public float time2;

	public float initDelay;

	public float jumpDelay;

	private Tween tween;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(initDelay);
		StartTween();
	}

	private void StartTween()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Join(dot.DOScale(new Vector3(1.2f, 0.5f), time2));
		sequence.Append(dot.DOMove(a.position, time));
		sequence.Join(dot.DOScale(new Vector3(0.6f, 1f), time));
		sequence.Append(dot.DOScale(new Vector3(0.8f, 1f), time));
		sequence.Append(dot.DOMove(b.position, time));
		sequence.Join(dot.DOScale(new Vector3(0.6f, 1f), time));
		sequence.Append(dot.DOScale(new Vector3(1.2f, 0.5f), time2));
		sequence.Append(dot.DOScale(1f, time));
		sequence.AppendInterval(jumpDelay);
		sequence.SetLoops(-1, LoopType.Restart);
		tween = sequence;
	}

	private void OnEnable()
	{
		tween?.Play();
	}

	private void OnDisable()
	{
		tween?.Pause();
	}

	private void OnDestroy()
	{
		tween.Kill();
	}
}
