namespace GreenT.Model.Collections;

public interface ICollectionManager<T> : ICollectionSetter<T>, ICollectionAdder<T>
{
	void RemoveItems(params T[] items);
}
