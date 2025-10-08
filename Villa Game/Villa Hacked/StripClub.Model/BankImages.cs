using System;
using UnityEngine;

namespace StripClub.Model;

[Serializable]
public class BankImages
{
	[field: SerializeField]
	public Sprite Small { get; internal set; }

	[field: SerializeField]
	public Sprite Big { get; internal set; }
}
