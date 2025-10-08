namespace StripClub.Model;

public interface ICollectionSetter<in T>
{
	void Add(params T[] obj);
}
