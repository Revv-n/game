using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Bank.UI;

public class PendingLoader : MonoBehaviour
{
	private Vector3 rotate = new Vector3(0f, 0f, 360f);

	private Tween rotateTween;

	private void OnEnable()
	{
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
		rotateTween = base.transform.DORotate(rotate, 2f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
	}

	private void OnDisable()
	{
		rotateTween.Kill();
	}
}
