using System;
using MetaphysicsIndustries.Solus;
using System.Collections.Generic;

namespace MetaphysicsIndustries.Ligra
{
    public class LigraEnvironment : SolusEnvironment
    {
        public readonly List<RenderItem> RenderItems = new List<RenderItem>();
        public readonly List<string> History = new List<string>();
        public int CurrentHistoryIndex = -1;
    }
}
