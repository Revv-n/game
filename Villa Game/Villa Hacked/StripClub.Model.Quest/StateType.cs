using System;

namespace StripClub.Model.Quest;

[Flags]
public enum StateType
{
	None = 0,
	Active = 1,
	Complete = 2,
	Rewarded = 4,
	Locked = 8
}
