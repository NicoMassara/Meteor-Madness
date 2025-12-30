using MeteorMadness.Contracts;
using MeteorMadness.Managers.Cosmetics;
using UnityEngine;

namespace _Main.Scripts.Cosmetics.SkinControllers
{
    public class MeteorSkinController : FlyingObjectSkinController<MeteorSkinData>
    {
        protected override MeteorSkinData GetSkinData(SkinType skinType) 
            => SkinManager.Instance.GetMeteorData(skinType);
    }
}