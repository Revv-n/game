namespace GreenT.HornyScapes.BannerSpace;

public class BannerInfoData
{
	public int Id;

	public string Source;

	public int BankTabId;

	public string BannerGroup;

	public string ContentSource;

	public string BackgroundName;

	public BannerInfoData(int id, string source, int bankTabId, string bannerGroup, string contentSource, string backgroundName)
	{
		Id = id;
		Source = source;
		BankTabId = bankTabId;
		BannerGroup = bannerGroup;
		ContentSource = contentSource;
		BackgroundName = backgroundName;
	}
}
