
using MeteorMadness.GlobalValues;

namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class MeteorSkinController : FlyingObjectSkinController
    {
        protected override SkinData GetSkinData(SkinType skinType)
        {
            return null;
            //return SkinManager.Instance.GetMeteorData(skinType);
        }
    }
}