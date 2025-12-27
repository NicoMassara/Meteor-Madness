
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Interfaces.Skins;
using MeteorMadness.Managers.Cosmetics;

namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class MeteorSkinController : FlyingObjectSkinController<IMeteorSkinData>
    {
        protected override IMeteorSkinData GetSkinData(SkinType skinType)
        {
            return SkinManager.Instance.GetMeteorData(skinType);
        }
    }
}