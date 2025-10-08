using System;
using GreenT.HornyScapes.Saves;

namespace GreenT.Data;

public interface ISaveLoader
{
	IObservable<SavedData> LoadSave();
}
