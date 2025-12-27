
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Interfaces.Skins;
using MeteorMadness.Managers.Cosmetics;

namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class CometSkinController : FlyingObjectSkinController<ICometSkinData>
    {
        protected override ICometSkinData GetSkinData(SkinType skinType)
        {
            return SkinManager.Instance.GetCometData(skinType);
        }
    }
}