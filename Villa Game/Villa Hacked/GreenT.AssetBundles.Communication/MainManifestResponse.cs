using System.Collections.Generic;
using GreenT.HornyScapes;

namespace GreenT.AssetBundles.Communication;

public class MainManifestResponse : BaseResponse
{
	public List<BuildMainInfo> info = new List<BuildMainInfo>();
}
