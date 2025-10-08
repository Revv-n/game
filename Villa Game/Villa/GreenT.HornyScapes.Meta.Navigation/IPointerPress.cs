using System;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.Navigation;

public interface IPointerPress
{
	IObservable<Vector2> OnPointerDown();

	IObservable<Vector2> OnPointerUp();

	IObservable<Vector2> OnPointerClick();
}
