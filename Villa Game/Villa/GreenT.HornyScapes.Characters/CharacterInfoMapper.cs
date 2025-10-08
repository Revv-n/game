using System;
using GreenT.HornyScapes.Card;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.Characters;

[Serializable]
[Mapper]
public class CharacterInfoMapper : CardMapper
{
	public ContentType type;

	public LoadType load_type;

	public int[] spawner_id;

	public int[] nude_level;

	public int promote_pattern_id;

	public UnlockType[] preload_type;

	public string[] preload_value;

	public int order_number;

	public int id_relationship;
}
