using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Models
{
    public class StoryMonitored
    {
        [PrimaryKey]
        public int StoryIndex { get; set; }

        public bool IsRewardClaimed { get; set; } = false;
        public bool IsPurchased { get; set; } = false;
        public bool NarratorEagleUnlocked { get; set; } = false;
        public bool NarratorMonkeyUnlocked { get; set; } = false;
    }
}
