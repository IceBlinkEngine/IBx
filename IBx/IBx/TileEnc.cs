using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBx
{
    public class TileEnc 
    {
        //[JsonIgnore]
        public string Layer1Filename = "t_grass";
        //[JsonIgnore]
        public string Layer2Filename = "t_blank";
        //[JsonIgnore]
        public string Layer3Filename = "t_blank";
        //[JsonIgnore]
        public int Layer1Rotate = 0;
        //[JsonIgnore]
        public int Layer2Rotate = 0;
        //[JsonIgnore]
        public int Layer3Rotate = 0;
        //[JsonIgnore]
        public int Layer1Xshift = 0;
        //[JsonIgnore]
        public int Layer2Xshift = 0;
        //[JsonIgnore]
        public int Layer3Xshift = 0;
         //[JsonIgnore]
        public int Layer1Yshift = 0;
         //[JsonIgnore]
        public int Layer2Yshift = 0;
         //[JsonIgnore]
        public int Layer3Yshift = 0;
         /// <summary>
         /// 
         /// </summary>
//[JsonIgnore]
        public bool Layer1Mirror = false;
         //[JsonIgnore]
        public bool Layer2Mirror = false;
         //[JsonIgnore]
        public bool Layer3Mirror = false;
         //[JsonIgnore]
        public int Layer1Xscale = 0;
         //[JsonIgnore]
        public int Layer2Xscale = 0;
         //[JsonIgnore]
        public int Layer3Xscale = 0;
         //[JsonIgnore]
        public int Layer1Yscale = 0;
         //[JsonIgnore]
        public int Layer2Yscale = 0;
         //[JsonIgnore]
        public int Layer3Yscale = 0;
         //[JsonIgnore]
        public bool Walkable = true;
         //[JsonIgnore]
	    public bool LoSBlocked = false;

        public TileEnc()
	    {
	
	    }
    }
}
