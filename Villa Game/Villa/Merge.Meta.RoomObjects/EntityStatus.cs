using System;

namespace Merge.Meta.RoomObjects;

[Flags]
public enum EntityStatus
{
	Blocked = 1,
	InProgress = 2,
	Complete = 4,
	Rewarded = 8
}
