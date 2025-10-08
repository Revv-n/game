namespace GreenT.Model.Collections;

public interface ICollectionSetter<T>
{
	void SetItems(params T[] items);
}
