using System;

namespace GreenT.HornyScapes.Monetization;

public interface IIAPController<TData>
{
	IObservable<Product> OnPressButton { get; }

	IObservable<CheckoutData> OnOpenForm { get; }

	IObservable<TData> OnSucceeded { get; }

	IObservable<string> OnFailed { get; }
}
