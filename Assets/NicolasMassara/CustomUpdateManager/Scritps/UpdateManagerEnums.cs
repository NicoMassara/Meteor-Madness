namespace NicolasMassara.CustomUpdateManager
{
    public enum UpdateGroup
    {
        Always,
        Gameplay,
        UI,
        Inputs,
        Camera,
        Effects,
        Shield,
        Earth,
        Ability
    }

    public enum TickGroup
    {
        EveryFrame,
        HalfTarget,
        QuarterTarget,
        EightTarget,
        SixteenthTarget,
        ThirtySecondTarget,
        SixtyFourthTarget,
        EverySecond
    }
}