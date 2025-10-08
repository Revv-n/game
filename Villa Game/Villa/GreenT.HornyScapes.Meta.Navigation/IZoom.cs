using System;

namespace GreenT.HornyScapes.Meta.Navigation;

public interface IZoom
{
	bool IsZooming { get; }

	float ZoomDelta { get; }

	IObservable<float> OnZoom();
}
