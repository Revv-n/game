using Newtonsoft.Json;

namespace StripClub.Model.Shop;

public class LocalizedPrice
{
	public string id;

	public decimal US;

	public decimal RU;

	public decimal ES;

	public decimal PT;

	public decimal DE;

	public decimal FR;

	public decimal IT;

	public decimal PL;

	public decimal BR;

	public decimal IN;

	public decimal MX;

	public decimal HK;

	public decimal TW;

	public decimal CA;

	public decimal UA;

	public decimal AU;

	public decimal VN;

	public decimal TH;

	public decimal JP;

	public decimal KZ;

	public decimal nutaku;

	public decimal erolabs;

	[JsonProperty("is_giving_points")]
	public bool IsGivingPoints;

	[JsonProperty("points_qty")]
	public int PointsQty;
}
