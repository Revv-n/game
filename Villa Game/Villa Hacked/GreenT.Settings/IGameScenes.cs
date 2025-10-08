using System.Collections.Generic;

namespace GreenT.Settings;

public interface IGameScenes
{
	IEnumerable<SerializedScene> LevelScenesLoadOrder { get; }
}
