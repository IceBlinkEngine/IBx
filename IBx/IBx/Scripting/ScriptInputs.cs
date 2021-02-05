using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBx.Scripting
{
    class ScriptInputs : Dictionary<String, Object>
    {
        // This is meant to be the only method that scripts can call
        // Eventually, find a way to restrict the set methods
        public Object GetValue(string key)
        {
            return this[key];
        }
    }
}
