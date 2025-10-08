namespace GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;

public sealed class IndicatorSignals
{
	public class PushRequest
	{
		public bool Status { get; }

		public FilteredIndicatorType Type { get; }

		public PushRequest(bool status, FilteredIndicatorType type)
		{
			Status = status;
			Type = type;
		}
	}
}
