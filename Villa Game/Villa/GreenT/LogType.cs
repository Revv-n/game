using System;

namespace GreenT;

[Flags]
public enum LogType
{
	Error = 2,
	Warning = 4,
	Data = 8,
	Net = 0x10,
	Game = 0x20,
	PlayerStats = 0x40,
	BundleLoad = 0x100,
	Analytic = 0x400,
	Messenger = 0x800,
	Lootbox = 0x1000,
	Payments = 0x4000,
	Events = 0x8000,
	Patterns = 0x10000,
	Tutorial = 0x20000,
	SaveEvent = 0x40000,
	RoomObject = 0x400000,
	Sellout = 0x800000,
	ScreenIndicator = 0x1000000,
	Banner = 0x2000000,
	Save = 0x4000000,
	Dates = 0x8000000,
	Develop = 0x40000000
}
