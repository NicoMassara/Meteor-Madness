
using MeteorMadness.GlobalValues;

namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class CometSkinController : FlyingObjectSkinController
    {
        protected override SkinData GetSkinData(SkinType skinType)
        {
            return null;
            //return SkinManager.Instance.GetCometData(skinType);
        }
    }
}