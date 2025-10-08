using System;
using System.Collections.Generic;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class RegionPriceResolver : BasePriceResolver, IDisposable
{
	private string _steamRegion;

	private readonly SteamRegionRequest _regionRequest;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	public RegionPriceResolver(SteamRegionRequest regionRequest, LotManager lotManager, LocalizedPriceManager localizedPriceManager)
		: base(lotManager, localizedPriceManager)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_regionRequest = regionRequest;
	}

	public void OnAuth()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SteamUserResponse>(_regionRequest.Post(), (Action<SteamUserResponse>)delegate(SteamUserResponse response)
		{
			_steamRegion = response.data.country;
		}), (ICollection<IDisposable>)_disposables);
	}

	public override void Initialize()
	{
		Setup(_steamRegion);
	}

	public void Dispose()
	{
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Dispose();
		}
	}
}
