namespace GreenT.HornyScapes.Analytics;

public class EditorUrlReader : IUrlReader
{
	private string url;

	private const string referrer = "https://unity_editor.com";

	public EditorUrlReader(string url)
	{
		this.url = url;
	}

	public string ReadURL()
	{
		return url;
	}

	public string ReadReferrer()
	{
		return "https://unity_editor.com";
	}
}
