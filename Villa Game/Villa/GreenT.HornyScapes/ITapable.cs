using System;

namespace GreenT.HornyScapes;

public interface ITapable<out T>
{
	IObservable<T> OnTap();
}
