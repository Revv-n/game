using GreenT.UI;
using Zenject;

namespace GreenT.HornyScapes.Collection;

public class FromTypeWindowOpener : WindowOpener
{
	public ReturnToType OpenedFromType;

	protected ReturnButtonStrategy returnButtonStrategy;

	[Inject]
	private void Constructor(ReturnButtonStrategy returnButtonStrategy)
	{
		this.returnButtonStrategy = returnButtonStrategy;
	}

	public override void Click()
	{
		returnButtonStrategy.Set(OpenedFromType);
		base.Click();
	}

	public override void OpenOnly()
	{
		returnButtonStrategy.Set(OpenedFromType);
		base.OpenOnly();
	}
}
