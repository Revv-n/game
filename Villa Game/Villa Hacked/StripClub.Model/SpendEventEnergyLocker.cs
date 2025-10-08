namespace StripClub.Model;

public class SpendEventEnergyLocker : Locker
{
	public readonly string EventID;

	private readonly int _targetValue;

	public SpendEventEnergyLocker(string eventID, int targetValue)
	{
		EventID = eventID;
		_targetValue = targetValue;
	}

	public override void Initialize()
	{
	}

	public void Set(int currentValue)
	{
		if (!isOpen.Value)
		{
			isOpen.Value = currentValue >= _targetValue;
		}
	}

	public void Reset()
	{
		isOpen.Value = false;
	}
}
