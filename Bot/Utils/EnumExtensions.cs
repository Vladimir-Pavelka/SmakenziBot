namespace SmakenziBot.Utils
{
    using System;
    using BuildOrder.Steps;

    public static class EnumExtensions
    {
        public static BaseType ToBaseType(this HatcheryType ht)
        {
            switch (ht)
            {
                case HatcheryType.NaturalExp: return BaseType.Natural;
                case HatcheryType.ThirdExp: return BaseType.Third;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}