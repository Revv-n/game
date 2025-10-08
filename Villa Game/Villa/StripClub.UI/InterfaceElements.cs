using System;

namespace StripClub.UI;

[Flags]
public enum InterfaceElements
{
	All = int.MinValue,
	Settings = 1,
	Inventory = 2,
	Messenger = 4,
	Shop = 8,
	Collections = 0x10,
	Registration = 0x20,
	Login = 0x40,
	LevelUp = 0x80,
	Merge = 0x100,
	Background = 0x200,
	Rewards = 0x400,
	Resources = 0x800,
	TapToContinue = 0x1000,
	MainLevelWindow = 0x2000,
	Core = 0x4000,
	MergeInventory = 0x8000,
	Task = 0x10000,
	StarShop = 0x20000,
	StarBar = 0x40000
}
