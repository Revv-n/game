using System.Collections.Generic;

namespace StripClub.Model.Character;

public interface IPrivateInfoBase
{
	IEnumerable<CharacterConfiguration> GetEverybodiesInfo(params IComparer<CharacterConfiguration>[] comparer);
}
