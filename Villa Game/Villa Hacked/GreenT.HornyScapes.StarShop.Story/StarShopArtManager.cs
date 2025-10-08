using System;
using GreenT.Model.Collections;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.StarShop.Story;

public class StarShopArtManager : SimpleManager<StarShopArt>
{
	private ReactiveProperty<Sprite> current = new ReactiveProperty<Sprite>((Sprite)null);

	public IReadOnlyReactiveProperty<Sprite> Current;

	public StarShopArtManager()
	{
		current = new ReactiveProperty<Sprite>((Sprite)null);
		Current = (IReadOnlyReactiveProperty<Sprite>)(object)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<Sprite>((IObservable<Sprite>)current);
	}

	internal void SetArt(Sprite art)
	{
		current.Value = art;
	}

	public override void Dispose()
	{
		base.Dispose();
		current.Dispose();
	}
}
