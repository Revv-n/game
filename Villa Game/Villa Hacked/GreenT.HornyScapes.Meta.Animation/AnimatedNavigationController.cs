using GreenT.HornyScapes.Meta.Navigation;
using GreenT.HornyScapes.UI.Meta;
using GreenT.Utilities;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.Animation;

public class AnimatedNavigationController : HouseNavigationController
{
	[SerializeField]
	private CameraAnimation animation;

	public override void SnapTo(Bounds bounds)
	{
		animation.Camera = base.Camera;
		animation.MovementBounds = base.MovementBounds;
		Bounds bounds2 = BoundsExtension.Intersection(bounds, base.MovementBounds);
		animation.Position = CalculateCameraPosition(bounds2);
		animation.ZoomOrthoSize = CalculateOrthoSize(bounds2);
		animation.Stop();
		animation.Init();
		animation.Play();
	}
}
