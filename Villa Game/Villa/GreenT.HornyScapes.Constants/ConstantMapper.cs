using System;
using GreenT.HornyScapes.Data;
using Newtonsoft.Json;

namespace GreenT.HornyScapes.Constants;

[Serializable]
[Mapper]
public class ConstantMapper
{
	[JsonProperty("constant_name")]
	public string Name { get; private set; }

	[JsonProperty("value")]
	public string Value { get; private set; }

	[JsonProperty("type")]
	public string Type { get; private set; }
}
