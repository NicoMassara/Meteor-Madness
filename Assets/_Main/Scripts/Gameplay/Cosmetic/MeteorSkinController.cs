using MeteorMadness.Contracts;
using MeteorMadness.Managers.Cosmetics;

namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class MeteorSkinController : FlyingObjectSkinController<MeteorSkinData>
    {
        protected override MeteorSkinData GetSkinData(SkinType skinType)
        {
            return SkinManager.Instance.GetMeteorData(skinType);
        }
    }
}