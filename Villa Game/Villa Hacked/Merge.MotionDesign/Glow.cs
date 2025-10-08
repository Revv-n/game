using DG.Tweening;
using UnityEngine;

namespace Merge.MotionDesign;

public class Glow : MonoBehaviour
{
	public Transform glow1;

	public Transform reward1;

	public Transform reward2;

	public Transform reward3;

	public Transform buttonPlay;

	public Transform target;

	public SpriteRenderer glow2sr;

	public SpriteRenderer glow1sr;

	public SpriteRenderer taskssr;

	public SpriteRenderer reward1sr;

	public SpriteRenderer reward2sr;

	public SpriteRenderer reward3sr;

	public float time;

	private void Start()
	{
		Sequence s = DOTween.Sequence();
		s.Join(DOTweenModuleSprite.DOFade(glow1sr, 1f, 0.1f));
		s.Append(glow1.DOScale(3.8f, time));
		s.Join(DOTweenModuleSprite.DOFade(glow2sr, 1f, time));
		s.Append(DOTweenModuleSprite.DOFade(glow1sr, 0f, time));
		s.Join(DOTweenModuleSprite.DOFade(taskssr, 0f, time));
		s.Append(DOTweenModuleSprite.DOFade(glow2sr, 0f, time));
		s.Append(reward1.DOScale(0.8f, time));
		Sequence sequence = DOTween.Sequence();
		sequence.Append(EasyBezierTweener.DoBezier(reward1, reward1.position, target.position, time));
		sequence.Join(reward1.DOScale(1.4f, time / 2f));
		sequence.Insert(time / 2f, reward1.DOScale(1f, time / 2f));
		s.Append(sequence);
		s.Append(DOTweenModuleSprite.DOFade(reward1sr, 0f, time));
		s.Join(reward2.DOScale(0.8f, time));
		s.Join(buttonPlay.DOScale(1.4f, time));
		s.Append(buttonPlay.DOScale(1f, time));
		Sequence sequence2 = DOTween.Sequence();
		sequence2.Join(EasyBezierTweener.DoBezier(reward2, reward2.position, target.position, time));
		sequence2.Join(reward2.DOScale(1.4f, time / 2f));
		sequence2.Insert(time / 2f, reward2.DOScale(1f, time / 2f));
		s.Join(sequence2);
		s.Append(DOTweenModuleSprite.DOFade(reward2sr, 0f, time));
		s.Join(reward3.DOScale(0.8f, time));
		s.Join(buttonPlay.DOScale(1.4f, time));
		s.Append(buttonPlay.DOScale(1f, time));
		Sequence sequence3 = DOTween.Sequence();
		sequence3.Join(EasyBezierTweener.DoBezier(reward3, reward3.position, target.position, time));
		sequence3.Join(reward3.DOScale(1.4f, time / 2f));
		sequence3.Insert(time / 2f, reward3.DOScale(1f, time / 2f));
		s.Join(sequence3);
		s.Append(DOTweenModuleSprite.DOFade(reward3sr, 0f, time));
		s.Join(buttonPlay.DOScale(1.4f, time));
		s.Append(buttonPlay.DOScale(1f, time));
	}

	private void Update()
	{
	}
}
