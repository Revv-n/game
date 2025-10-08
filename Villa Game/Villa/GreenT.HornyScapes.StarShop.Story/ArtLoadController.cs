using System;
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
		IObservable<StarShopArt> source = (from _art in artManager.Collection
			where !_art.Locker.IsOpen.Value
			select from _ in _art.Locker.IsOpen.First((bool x) => x)
				select _art).Merge();
		if (starShopArt != null)
		{
			source = source.StartWith(starShopArt);
		}
		source.SelectMany((StarShopArt _art) => spriteLoader.Load(_art.ID)).Subscribe(artManager.SetArt).AddTo(disposables);
	}

	public void Dispose()
	{
		disposables.Dispose();
	}
}
