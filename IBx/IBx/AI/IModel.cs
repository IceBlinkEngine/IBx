using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBx;

namespace IBx.AI
{
    public interface IModel
    {
        void InvokeAI(GameView gv, ScreenCombat sc, Creature attacker);
    }
}
