namespace StripClub.Model;

public class MergeItemLocker : Locker
{
	private readonly int _mergeItemID;

	public MergeItemLocker(int id)
	{
		_mergeItemID = id;
	}

	public void Set(int id)
	{
		if (!base.IsOpen.Value)
		{
			isOpen.Value = id == _mergeItemID;
		}
	}

	public override void Initialize()
	{
	}
}
