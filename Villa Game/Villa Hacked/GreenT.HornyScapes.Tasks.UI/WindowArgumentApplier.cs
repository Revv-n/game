using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Messenger.UI;
using GreenT.UI;
using StripClub.Messenger;

namespace GreenT.HornyScapes.Tasks.UI;

public class WindowArgumentApplier
{
	private readonly IMessengerManager _messengerManager;

	public WindowArgumentApplier(IMessengerManager messengerManager)
	{
		_messengerManager = messengerManager;
	}

	public void SetArgs(IGoal goal, IEnumerable<IWindow> windowOpenerWindows)
	{
		if (ValidateInput(goal.ActionButtonType))
		{
			Dictionary<Type, IWindow> openedWindowsByType = GetOpenedWindowsByType(windowOpenerWindows);
			Apply(goal, openedWindowsByType);
		}
	}

	private static bool ValidateInput(ActionButtonType goalActionButtonType)
	{
		return goalActionButtonType != ActionButtonType.None;
	}

	private Dictionary<Type, IWindow> GetOpenedWindowsByType(IEnumerable<IWindow> windowOpenerWindows)
	{
		return windowOpenerWindows.ToDictionary((IWindow window) => window.GetType(), (IWindow window) => window);
	}

	private void Apply(IGoal goal, Dictionary<Type, IWindow> windowsByType)
	{
		if (goal.ActionButtonType == ActionButtonType.ToChat && windowsByType.ContainsKey(typeof(MessengerWindow)))
		{
			SetChatArgs(goal, windowsByType[typeof(MessengerWindow)]);
		}
	}

	private void SetChatArgs(IGoal goal, IWindow window)
	{
		if (!(window is MessengerWindow messengerWindow))
		{
			return;
		}
		IObjective objective = goal.Objectives.First();
		IConversationObjective conversationObjective = objective as IConversationObjective;
		if (conversationObjective != null)
		{
			Conversation conversation = _messengerManager.GetConversations().FirstOrDefault((Conversation x) => x.ID.Equals(conversationObjective.ConversationId));
			if (conversation != null)
			{
				messengerWindow.RequestShowConversationOnOpenWindow(conversation);
			}
		}
	}
}
