namespace GreenT.Model.Collections;

public interface ICollectionAdder<T>
{
	void AddItems(params T[] items);
}
