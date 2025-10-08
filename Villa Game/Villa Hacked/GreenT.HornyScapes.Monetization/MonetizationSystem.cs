using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Monetization;

public abstract class MonetizationSystem<TData> : IIAPController<TData>, IInitializable, IDisposable where TData : new()
{
	protected CompositeDisposable logStream = new CompositeDisposable();

	protected Subject<Product> onPressButton = new Subject<Product>();

	protected Subject<CheckoutData> onOpenForm = new Subject<CheckoutData>();

	protected Subject<TData> onSucceeded = new Subject<TData>();

	protected Subject<string> onFailed = new Subject<string>();

	public virtual IObservable<Product> OnPressButton => Observable.AsObservable<Product>((IObservable<Product>)onPressButton);

	public virtual IObservable<CheckoutData> OnOpenForm => Observable.AsObservable<CheckoutData>((IObservable<CheckoutData>)onOpenForm);

	public virtual IObservable<TData> OnSucceeded => Observable.AsObservable<TData>((IObservable<TData>)onSucceeded);

	public virtual IObservable<string> OnFailed => Observable.AsObservable<string>((IObservable<string>)onFailed);

	public virtual void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(OnFailed, (Action<string>)delegate
		{
		}), (ICollection<IDisposable>)logStream);
	}

	public void BuyProduct(string lotId, int monetizationId, string price)
	{
		Product product = new Product(lotId, monetizationId, price);
		BuyProduct(product);
	}

	protected virtual void BuyProduct(Product product)
	{
		onPressButton?.OnNext(product);
	}

	public virtual void Dispose()
	{
		onSucceeded.OnCompleted();
		onSucceeded.Dispose();
		onFailed.OnCompleted();
		onFailed.Dispose();
	}
}
