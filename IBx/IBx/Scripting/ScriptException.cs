using System;

namespace IBx.Scripting
{
    class ScriptException : Exception
    {
        public ScriptException(Exception ex) : base("Exception in script", ex)
        {
        }
    }
}
