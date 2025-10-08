namespace StripClub.UI;

public interface IView
{
	int SiblingIndex { get; set; }

	void Display(bool isOn);

	bool IsActive();
}
public interface IView<in T> : IView
{
	void Set(T param);
}
