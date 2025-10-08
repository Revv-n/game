namespace GreenT.HornyScapes.Monetization.Webgl;

public class MonetizationRequestData
{
	private const string imageUrl = "";

	public readonly string ItemNameKey;

	public readonly string ItemDescriptionKey;

	public readonly string ImageNameKey;

	public MonetizationRequestData(string itemNameKey, string itemDescriptionKey, string imageNameKey)
	{
		ItemNameKey = itemNameKey;
		ItemDescriptionKey = itemDescriptionKey;
		ImageNameKey = string.Format("", imageNameKey);
	}
}
