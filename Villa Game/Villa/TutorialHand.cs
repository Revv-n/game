using DG.Tweening;
using UnityEngine;

public class TutorialHand : MonoBehaviour
{
	public Transform hand;

	public Transform ellipse;

	public Transform target;

	public SpriteRenderer handsr;

	public SpriteRenderer ellipsesr;

	public float time;

	public float timeFly;

	private void Start()
	{
		Sequence s = DOTween.Sequence();
		s.Join(handsr.DOFade(1f, 0.2f));
		s.Append(hand.DOScale(0.7f, time));
		s.Join(ellipsesr.DOFade(1f, 0.2f));
		s.Append(hand.DOMove(target.position, timeFly));
		s.Join(ellipse.DOMove(target.position, timeFly));
		s.Append(hand.DOScale(0.9f, time));
		s.Join(ellipsesr.DOFade(0f, 0.2f));
		s.Append(handsr.DOFade(0f, 0.2f));
	}

	private void Update()
	{
	}
}
