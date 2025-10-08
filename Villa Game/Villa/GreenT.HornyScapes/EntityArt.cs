using StripClub.Model;

namespace GreenT.HornyScapes;

public abstract class EntityArt
{
	public int ID { get; }

	public CompositeLocker Locker { get; }

	public EntityArt(int id, ILocker[] locker)
	{
		ID = id;
		Locker = new CompositeLocker(locker);
	}

	public override string ToString()
	{
		return base.ToString() + " ID: " + ID;
	}
}
