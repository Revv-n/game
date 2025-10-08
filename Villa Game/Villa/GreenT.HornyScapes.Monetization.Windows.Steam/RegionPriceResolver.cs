using System;
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
		_regionRequest = regionRequest;
	}

	public void OnAuth()
	{
		_regionRequest.Post().Subscribe(delegate(SteamUserResponse response)
		{
			_steamRegion = response.data.country;
		}).AddTo(_disposables);
	}

	public override void Initialize()
	{
		Setup(_steamRegion);
	}

	public void Dispose()
	{
		_disposables?.Dispose();
	}
}
