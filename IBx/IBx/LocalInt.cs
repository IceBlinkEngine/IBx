using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBx
{
    public class LocalInt 
    {
	    public string Key = "";
	    public int Value = 0;
    	
	    public LocalInt()
	    {
		
	    }
	
	    public LocalInt DeepCopy()
        {
		    LocalInt copy = new LocalInt();
		    copy.Key = this.Key;
		    copy.Value = this.Value;
		    return copy;
        }
    }
}
