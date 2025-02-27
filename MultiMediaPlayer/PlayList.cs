﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMediaPlayer
{
    class PlayList
    { 

        public String NamePlayList { get; set; }

        public String[] MediaList {  get; set;}

        public int TotalMedia { 
            get {
                if (MediaList == null) return 0;
                else
                     return MediaList.Length; }
        }

        public void RemoveAt(int pos)
        {
            List<String> temp = new List<string>(MediaList);
            temp.RemoveAt(pos);
            MediaList = temp.ToArray();
        }

    }
}
