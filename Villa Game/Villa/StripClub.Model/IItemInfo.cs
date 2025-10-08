using System;
using UnityEngine;

namespace StripClub.Model;

public interface IItemInfo
{
	Guid Guid { get; }

	string LocalizationKey { get; }

	Sprite Icon { get; }
}
