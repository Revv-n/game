using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.StarShop.Story;

public class ArtLoadController : IInitializable, IDisposable
{
	private CompositeDisposable disposables = new CompositeDisposable();

	private ILoader<int, Sprite> spriteLoader;

	private StarShopArtManager artManager;

	[Inject]
	public void Init(ILoader<int, Sprite> spriteLoader, StarShopArtManager artManager)
	{
		this.spriteLoader = spriteLoader;
		this.artManager = artManager;
	}

	public void Initialize()
	{
		disposables.Clear();
		StarShopArt starShopArt = artManager.Collection.LastOrDefault((StarShopArt _art) => _art.Locker.IsOpen.Value);
		IObservable<StarShopArt> observable = Observable.Merge<StarShopArt>(from _art in artManager.Collection
			where !_art.Locker.IsOpen.Value
			select Observable.Select<bool, StarShopArt>(Observable.First<bool>((IObservable<bool>)_art.Locker.IsOpen, (Func<bool, bool>)((bool x) => x)), (Func<bool, StarShopArt>)((bool _) => _art)));
		if (starShopArt != null)
		{
			observable = Observable.StartWith<StarShopArt>(observable, starShopArt);
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Sprite>(Observable.SelectMany<StarShopArt, Sprite>(observable, (Func<StarShopArt, IObservable<Sprite>>)((StarShopArt _art) => spriteLoader.Load(_art.ID))), (Action<Sprite>)artManager.SetArt), (ICollection<IDisposable>)disposables);
	}

	public void Dispose()
	{
		disposables.Dispose();
	}
}
