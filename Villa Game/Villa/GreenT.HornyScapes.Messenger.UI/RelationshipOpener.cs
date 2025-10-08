using GreenT.HornyScapes.Collection;
using GreenT.UI;
using StripClub.Model.Cards;
using StripClub.UI.Collections.Promote;

namespace GreenT.HornyScapes.Messenger.UI;

public sealed class RelationshipOpener
{
	private readonly IWindowsManager _windowsManager;

	private readonly ReturnButtonStrategy _returnButtonStrategy;

	public RelationshipOpener(IWindowsManager windowsManager, ReturnButtonStrategy returnButtonStrategy)
	{
		_windowsManager = windowsManager;
		_returnButtonStrategy = returnButtonStrategy;
	}

	public void Open(ICard card)
	{
		_returnButtonStrategy.Set(ReturnToType.Meta);
		PromoteWindow promoteWindow = _windowsManager.Get<PromoteWindow>();
		promoteWindow.Set(card);
		promoteWindow.ActivateTab(1);
		promoteWindow.Open();
	}
}
