using System;
using DG.Tweening;
using UnityEngine;

namespace Merge.MotionDesign.Tweeners;

public class TweenerGIFly : MonoBehaviour
{
	[Serializable]
	public class GIFlyParams
	{
		public float time;

		public float extremumFactor;

		public float extremumDistance;

		public float amplitudeScale;

		public float minimizeScale;

		public float afterFallBonusTime;

		public float normalizeTime;

		public Ease flyCurve;
	}

	[SerializeField]
	private GIFlyParams giFlyParams;

	public Tween CreateFlyTween(GameItem gi, Vector3 a, Vector3 b)
	{
		float time = giFlyParams.time;
		gi.transform.position = a;
		gi.IconRenderer.sortingOrder += 100;
		Sequence sequence = DOTween.Sequence();
		Sequence sequence2 = DOTween.Sequence();
		sequence2.Append(gi.transform.DOScale(new Vector3(1f, giFlyParams.amplitudeScale, 1f), giFlyParams.time * giFlyParams.extremumDistance).SetEase(Ease.InOutSine));
		sequence2.Append(gi.transform.DOScaleY(giFlyParams.minimizeScale, time - time * giFlyParams.extremumDistance + giFlyParams.afterFallBonusTime).SetEase(Ease.InOutSine));
		sequence2.AppendCallback(delegate
		{
			TweenerMaster.DropFogCreator.CreateFog(b);
		});
		sequence2.Append(gi.transform.DOScale(1f, giFlyParams.normalizeTime).SetEase(Ease.InOutSine));
		sequence.Append(EasyBezierTweener.DoBezier(gi.transform, a, b, time, giFlyParams.extremumFactor, giFlyParams.extremumDistance, 1).SetEase(giFlyParams.flyCurve));
		sequence.Join(sequence2);
		sequence.onComplete = (TweenCallback)Delegate.Combine(sequence.onComplete, new TweenCallback(AtComplete));
		return sequence;
		void AtComplete()
		{
			gi.IconRenderer.sortingOrder -= 100;
		}
	}
}
