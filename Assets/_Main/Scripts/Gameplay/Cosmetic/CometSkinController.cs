using MeteorMadness.Contracts;
using MeteorMadness.Managers.Cosmetics;

namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class CometSkinController : FlyingObjectSkinController<CometSkinData>
    {
        protected override CometSkinData GetSkinData(SkinType skinType) 
            => SkinManager.Instance.GetCometData(skinType);
    }
}