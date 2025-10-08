namespace StripClub.Model.Data;

public interface IBundlesLoader<in KParam, out TResult> : ILoader<KParam, TResult>
{
	void ReleaseBundle(KParam param);
}
