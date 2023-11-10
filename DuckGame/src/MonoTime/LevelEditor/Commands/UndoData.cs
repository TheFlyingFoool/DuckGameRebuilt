using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class UndoData
    {
        public List<UndoData> actions = new List<UndoData>();
        public Action apply;
        public Action revert;
    }
}
