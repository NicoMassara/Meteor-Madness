
namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class MeteorSkinController : FlyingObjectSkinController
    {
        protected override SkinData GetSkinData(SkinType skinType)
        {
            return SkinManager.Instance.GetMeteorData(skinType);
        }
    }
}