using System;
using System.Collections.Generic;
using System.Text;

namespace IBx
{
    public class IBxPreferences
    {
        public bool GoogleAnalyticsOn = true;
        public string UserID = "none";
        public string UserName = "";
        public int CustomWindowSizeWidth = 1024;
        public int CustomWindowSizeHeight = 768;

        public IBxPreferences()
        {

        }

        public void GenerateUniqueUserID()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            UserID = new String(stringChars);
        }
    }
}
