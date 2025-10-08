using GreenT.HornyScapes.Collection;
using GreenT.HornyScapes.UI;
using GreenT.UI;
using StripClub.Model.Cards;
using StripClub.UI.Collections.Promote;

namespace GreenT.HornyScapes.Messenger.UI;

public class PromoteOpener : IOpener<ICard>
{
	private readonly IWindowsManager windowsManager;

	private readonly ReturnButtonStrategy returnButtonStrategy;

	public PromoteOpener(IWindowsManager windowsManager, ReturnButtonStrategy returnButtonStrategy)
	{
		this.windowsManager = windowsManager;
		this.returnButtonStrategy = returnButtonStrategy;
	}

	public void Open(ICard card)
	{
		returnButtonStrategy.Set(ReturnToType.Meta);
		PromoteWindow promoteWindow = windowsManager.Get<PromoteWindow>();
		promoteWindow.Set(card);
		promoteWindow.Open();
	}
}
