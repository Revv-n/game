using UnityEngine;

namespace GreenT.HornyScapes.Meta.Navigation;

public sealed class TouchScreenNavigation : AbstractNavigationMonoBehaviour, INavigation, IDrag, IZoom, IPointerPress
{
	private const float MULTIPLIER = 0.005f;

	private const float DRAG_TRASH_HOLD = 100f;

	public void Update()
	{
		switch (Input.touchCount)
		{
		case 1:
			EvaluatePress();
			EvaluateDragShift();
			break;
		case 2:
			EvaluateZoomDelta();
			break;
		default:
			base.IsDragging = false;
			base.IsZooming = false;
			break;
		}
	}

	private void EvaluatePress()
	{
		Touch touch = Input.GetTouch(0);
		Vector2 position = touch.position;
		if (touch.phase == TouchPhase.Began)
		{
			PointerDown(position);
		}
		if (touch.phase == TouchPhase.Ended)
		{
			PointerUp(position);
			PointerClick(position);
		}
	}

	private void EvaluateZoomDelta()
	{
		Touch touch = Input.GetTouch(0);
		Touch touch2 = Input.GetTouch(1);
		if (touch.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
		{
			Vector2 obj = ((touch.radius > touch.deltaPosition.magnitude) ? touch.position : (touch.position - touch.deltaPosition));
			Vector2 vector = ((touch2.radius > touch2.deltaPosition.magnitude) ? touch2.position : (touch2.position - touch2.deltaPosition));
			float magnitude = (obj - vector).magnitude;
			float num = (touch.position - touch2.position).magnitude - magnitude;
			base.IsZooming = Mathf.Abs(num) > Mathf.Min(touch.radius, touch2.radius);
			if (base.IsZooming)
			{
				base.ZoomDelta = num * 0.005f;
				Zoom(base.ZoomDelta);
			}
		}
	}

	private void EvaluateDragShift()
	{
		base.DragShift = Input.GetTouch(0).deltaPosition;
		base.IsDragging = base.DragShift.sqrMagnitude > 100f;
		if (base.IsDragging)
		{
			Drag(-base.DragShift);
		}
	}
}
