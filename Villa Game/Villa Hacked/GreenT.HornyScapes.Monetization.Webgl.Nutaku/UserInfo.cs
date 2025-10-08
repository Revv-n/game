namespace GreenT.HornyScapes.Monetization.Webgl.Nutaku;

internal class UserInfo
{
	public string PlayerId;

	public string NutakuId;

	public string Grade;

	public string Nickname;

	public override string ToString()
	{
		return "UserInfo:\n\tPlayerId: '" + PlayerId + "'\n\tNutakuId: '" + NutakuId + "'\n\tGrade: '" + Grade + "'\n\tNickname: '" + Nickname + "'";
	}
}
