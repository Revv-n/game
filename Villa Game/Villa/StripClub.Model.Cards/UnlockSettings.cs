namespace StripClub.Model.Cards;

public class UnlockSettings
{
	public UnlockType Type { get; }

	public int Value { get; }

	public UnlockSettings(UnlockType type, int value)
	{
		Type = type;
		Value = value;
	}
}
