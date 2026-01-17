using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Models
{
    public class Medals
    {
        [PrimaryKey]
        public int MedalId { get; set; }
        public string MedalName { get; set; } = "";
        public string MedalDescription { get; set; } = "";
        public string MedalImagePath { get; set; } = "";
        public bool isUnlocked { get; set; } = false;
        public DateTime TimeStamp = DateTime.Now;
    }
}
