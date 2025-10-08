namespace GreenT.HornyScapes.BannerSpace;

public class BannerCostData
{
	public readonly string BuyResource;

	public readonly int PriceX1;

	public readonly int PriceX10;

	public readonly string RebuyResource;

	public readonly int RebuyCost1x;

	public BannerCostData(string buyResource, int priceX1, int priceX10, string rebuyResource, int rebuyCost1x)
	{
		BuyResource = buyResource;
		PriceX1 = priceX1;
		PriceX10 = priceX10;
		RebuyResource = rebuyResource;
		RebuyCost1x = rebuyCost1x;
	}
}
