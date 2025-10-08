using GreenT.HornyScapes.Lootboxes;

namespace GreenT.Extensions;

public static class ExtensionMethods
{
	public static bool CheckRewardsLength(int lenght, RewType[] rew_type, string config_type_name, string[] rew_id, string config_id_name, int[] rew_qty, string config_qty_name)
	{
		CheckProperty(lenght, rew_type.Length, config_type_name, out var condition);
		CheckProperty(lenght, rew_id.Length, config_id_name, out condition);
		CheckProperty(lenght, rew_qty.Length, config_qty_name, out condition);
		return condition;
	}

	public static void CheckProperty(int referenceLenght, int lenght, string errorProperty, out bool condition)
	{
		condition = referenceLenght == lenght;
		_ = condition;
	}
}
