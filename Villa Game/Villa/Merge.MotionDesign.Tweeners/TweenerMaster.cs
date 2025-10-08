using UnityEngine;

namespace Merge.MotionDesign.Tweeners;

public class TweenerMaster : Controller<TweenerMaster>
{
	[SerializeField]
	private TweenerGIFly giFlyTweener;

	[SerializeField]
	private FogCreator dropFogCreator;

	public static TweenerGIFly TweenerGIFly => Controller<TweenerMaster>.Instance.giFlyTweener;

	public static FogCreator DropFogCreator => Controller<TweenerMaster>.Instance.dropFogCreator;
}
