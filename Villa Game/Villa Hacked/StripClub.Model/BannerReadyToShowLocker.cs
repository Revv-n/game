namespace StripClub.Model;

public class BannerReadyToShowLocker : Locker
{
	public readonly int ID;

	public BannerReadyToShowLocker(int id)
	{
		ID = id;
	}

	public void Open(int id)
	{
		if (!base.IsOpen.Value && id == ID)
		{
			isOpen.Value = true;
		}
	}

	public void Close(int id)
	{
		if (base.IsOpen.Value && id == ID)
		{
			isOpen.Value = false;
		}
	}

	public override void Initialize()
	{
	}
}
