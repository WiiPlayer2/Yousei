﻿using Yousei.Core;

namespace Yousei.Internal.Connectors.Control
{
    internal class ControlConnection : SimpleConnection
    {
        public ControlConnection()
        {
            AddAction<IfAction>("if");
            AddAction<ForEachAction>("foreach");
            AddAction<WhileAction>("while");
            AddAction<SwitchAction>("switch");
        }
    }
}