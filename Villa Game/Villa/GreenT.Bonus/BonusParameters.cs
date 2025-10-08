namespace GreenT.Bonus;

public class BonusParameters
{
	private string localizationKey = "content.bonus.";

	public int UniqParentID { get; }

	public BonusType BonusType { get; }

	public string NameKey { get; }

	private object _value { get; }

	public BonusParameters(int uniqParentID, string name, BonusType bonusType, object value)
	{
		UniqParentID = uniqParentID;
		BonusType = bonusType;
		NameKey = localizationKey + name;
		_value = value;
	}

	public T GetValue<T>()
	{
		return (T)_value;
	}
}
