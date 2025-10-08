using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCurrencyIconAnimator : MonoBehaviour
{
	[SerializeField]
	private Image currencyIcon;

	private Tween pingTween;

	public void Ping()
	{
		pingTween?.Kill();
		Sequence s = DOTween.Sequence();
		s.Append(currencyIcon.transform.DOScale(0.6f, 0.15f));
		s.Append(currencyIcon.transform.DOScale(1.3f, 0.2f).SetEase(Ease.InOutSine));
		s.Append(currencyIcon.transform.DOScale(1f, 0.2f).SetEase(Ease.InOutSine));
		pingTween = s;
	}

	public void PingAlert()
	{
		pingTween?.Kill();
		Sequence s = DOTween.Sequence();
		s.Append(currencyIcon.transform.DOScale(1.4f, 0.2f).SetEase(Ease.OutSine));
		s.Append(currencyIcon.transform.DOScale(1.25f, 0.15f).SetEase(Ease.InOutSine));
		s.Append(currencyIcon.transform.DOScale(1.4f, 0.15f).SetEase(Ease.InOutSine));
		s.Append(currencyIcon.transform.DOScale(1f, 0.15f).SetEase(Ease.InSine));
		pingTween = s;
	}
}
