using System;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.Navigation;

public interface IDrag
{
	bool IsDragging { get; }

	Vector2 DragShift { get; }

	IObservable<Vector2> OnDrag();
}
