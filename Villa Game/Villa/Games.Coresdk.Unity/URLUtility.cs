namespace Games.Coresdk.Unity;

public class URLUtility
{
	private const string DEEP_LINK_SCHEME = "coresdk";

	private const string DEEP_LINK_HOST = "coresdk.games";

	public static string GetDeepLinkURL(string packageName = null, string lastPath = null)
	{
		if (string.IsNullOrEmpty(lastPath))
		{
			return string.Format("{0}://{1}/{2}", "coresdk", "coresdk.games", packageName);
		}
		return string.Format("{0}://{1}/{2}/{3}", "coresdk", "coresdk.games", packageName, lastPath);
	}

	public static string GetAuthorizationToken(string url)
	{
		return TokenCollection.Parse(url).GetValue("token");
	}
}
