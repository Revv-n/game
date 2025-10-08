using GreenT.HornyScapes.Saves;

namespace GreenT.Data;

public interface ISaveSerializer
{
	void Serialize(SavedData state);
}
