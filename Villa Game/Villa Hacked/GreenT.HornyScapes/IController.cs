using System;

namespace GreenT.HornyScapes;

public interface IController : IDisposable
{
	void Initialize();

	void RefreshSavables();
}
