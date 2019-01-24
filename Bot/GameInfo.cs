namespace SmakenziBot
{
    using System.Collections.Generic;
    using NBWTA.Result;

    public class GameInfo
    {
        private readonly AnalyzedMap _analyzedMap;

        public GameInfo(AnalyzedMap analyzedMap)
        {
            _analyzedMap = analyzedMap;
        }

        public HashSet<MyBase> MyBases { get; } = new HashSet<MyBase>();
    }
}