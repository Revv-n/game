using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GlowTweenMeta : MonoBehaviour, IPoolReturner
{
	[SerializeField]
	private Image glow1sr;

	[SerializeField]
	private Image glow2sr;

	[SerializeField]
	private float time;

	public float Time => time;

	public Action ReturnInPool { get; set; }

	public Tween DoEffect()
	{
		Sequence sequence = DOTween.Sequence();
		base.gameObject.SetActive(value: true);
		sequence.Join(glow1sr.DOFade(1f, 0.1f));
		sequence.Append(glow2sr.transform.DOScale(1.5f, Time));
		sequence.Join(glow2sr.DOFade(1f, Time));
		sequence.Append(glow1sr.DOFade(0f, Time));
		sequence.Append(glow2sr.DOFade(0f, Time));
		sequence.Join(glow2sr.transform.DOScale(0.5f, Time));
		sequence.Pause();
		return sequence;
	}
}
