using UnityEngine;

namespace GreenT.HornyScapes.Meta.Navigation;

public sealed class PCNavigation : AbstractNavigationMonoBehaviour
{
	private const float DRAG_TRASH_HOLD = 1f;

	private Vector3 previousPosition;

	private Vector3 shift;

	private bool isPointerPressed;

	private void Update()
	{
		if (!Input.mousePresent)
		{
			Debug.LogError("Mouse didn't connected");
			return;
		}
		EvaluateDragShift();
		EvaluateZoomDelta();
	}

	private void EvaluateDragShift()
	{
		Vector3 mousePosition = Input.mousePosition;
		if (!base.IsDragging && isPointerPressed && Mathf.Abs((previousPosition - mousePosition).sqrMagnitude) > 1f)
		{
			base.IsDragging = true;
		}
		if (base.IsDragging)
		{
			shift = previousPosition - mousePosition;
			Drag(shift);
		}
		if (Input.GetMouseButtonDown(0))
		{
			isPointerPressed = true;
			PointerDown(mousePosition);
		}
		if (Input.GetMouseButtonUp(0))
		{
			isPointerPressed = false;
			PointerUp(mousePosition);
			if (base.IsDragging)
			{
				base.IsDragging = false;
			}
			else
			{
				PointerClick(mousePosition);
			}
		}
		if (isPointerPressed)
		{
			previousPosition = mousePosition;
		}
	}

	private void EvaluateZoomDelta()
	{
		if (Input.mouseScrollDelta.y != 0f)
		{
			float y = Input.mouseScrollDelta.y;
			Zoom(y);
		}
	}
}
