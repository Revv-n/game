namespace GreenT.HornyScapes.Analytics;

public class BaseMarketingAnalytic : BaseAnalytic
{
	protected const string ID_KEY = "subid=";

	private IUrlReader webglSiteReader;

	public BaseMarketingAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, IUrlReader webglSiteReader)
		: base(amplitude)
	{
		this.webglSiteReader = webglSiteReader;
	}

	public string ReadIDFFromURL()
	{
		string text = webglSiteReader.ReadURL();
		int num = text.IndexOf("subid=") + "subid=".Length;
		return text.Substring(num, text.Length - num);
	}
}
