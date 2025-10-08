using DG.Tweening;
using UnityEngine;

public class CleanUp : MonoBehaviour
{
	public Transform item1;

	public Transform item2;

	public Transform item3;

	public Transform item4;

	public Transform fog1;

	public Transform fog2;

	public Transform fog3;

	public Transform fog4;

	public Transform fog5;

	public Transform fog6;

	public Transform fog7;

	public SpriteRenderer itemsr1;

	public SpriteRenderer itemsr2;

	public SpriteRenderer itemsr3;

	public SpriteRenderer itemsr4;

	public SpriteRenderer fogsr1;

	public SpriteRenderer fogsr2;

	public SpriteRenderer fogsr3;

	public SpriteRenderer fogsr4;

	public SpriteRenderer fogsr5;

	public SpriteRenderer fogsr6;

	public SpriteRenderer fogsr7;

	public float time;

	public float time2;

	private void Start()
	{
		Sequence s = DOTween.Sequence();
		s.Join(fog2.DOScale(1.8f, time));
		s.Append(fog1.DOScale(1.3f, time));
		s.Join(fog2.DOScale(2f, time));
		s.Join(fog4.DOScale(1.8f, time));
		s.Append(fog3.DOScale(1.4f, time));
		s.Join(fog1.DOScale(1.5f, time));
		s.Join(fog5.DOScale(1.2f, time));
		s.Join(fog4.DOScale(2f, time));
		s.Join(fog6.DOScale(1.2f, time));
		s.Join(fog7.DOScale(0.8f, time));
		s.Append(fog3.DOScale(1.8f, time));
		s.Join(fog5.DOScale(1.4f, time));
		s.Join(fog6.DOScale(1.4f, time));
		s.Join(fog7.DOScale(1f, time));
		s.Join(DOTweenModuleSprite.DOFade(itemsr2, 0f, time2));
		s.Join(DOTweenModuleSprite.DOFade(itemsr1, 0f, time2));
		s.Join(DOTweenModuleSprite.DOFade(itemsr3, 0f, time2));
		s.Join(DOTweenModuleSprite.DOFade(itemsr4, 0f, time2));
		s.Append(DOTweenModuleSprite.DOFade(fogsr1, 0f, time2));
		s.Join(DOTweenModuleSprite.DOFade(fogsr2, 0f, time2));
		s.Join(DOTweenModuleSprite.DOFade(fogsr3, 0f, time2));
		s.Join(DOTweenModuleSprite.DOFade(fogsr4, 0f, time2));
		s.Join(DOTweenModuleSprite.DOFade(fogsr5, 0f, time2));
		s.Join(DOTweenModuleSprite.DOFade(fogsr6, 0f, time2));
		s.Join(DOTweenModuleSprite.DOFade(fogsr7, 0f, time2));
	}

	private void Update()
	{
	}
}
