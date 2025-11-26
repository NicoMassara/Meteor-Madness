
namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class CometSkinController : FlyingObjectSkinController
    {
        protected override SkinData GetSkinData(SkinType skinType)
        {
            return SkinManager.Instance.GetCometData(skinType);
        }
    }
}