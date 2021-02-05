using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBx.AI
{
    static class ModelFactory
    {
        public static IModel createModel(string modelType)
        {
            return new ScriptedAttacker();
            switch (modelType)
            {
                case "BasicAttacker":
                    return new BasicAttacker();
                case "GeneralCaster":
                    return new GeneralCaster();
                case "ScriptedAttacker":
                    return new ScriptedAttacker();
                default:
                    return new BasicAttacker();
            }
        }
    }
}
