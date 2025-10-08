namespace GreenT.HornyScapes.Analytics;

public class AmplitudeRequestData
{
	public string api_key;

	public byte[] @event;

	public AmplitudeRequestData(string apiKey, byte[] encodeJson)
	{
		api_key = apiKey;
		@event = encodeJson;
	}
}
