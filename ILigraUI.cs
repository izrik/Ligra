﻿using System;
using System.Collections.Generic;
using MetaphysicsIndustries.Ligra.RenderItems;

namespace MetaphysicsIndustries.Ligra
{
    public interface ILigraUI
    {
        void AddRenderItem(RenderItem item);
        void RemoveRenderItem(RenderItem item);
        IList<RenderItem> RenderItems { get; }
        Vector2 ClientSize { get; }

        void OpenPlotProperties(GraphItem item);

        IList<string> History { get; }
        int CurrentHistoryIndex { get; set; }
    }
}
