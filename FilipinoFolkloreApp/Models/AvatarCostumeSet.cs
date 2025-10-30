using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Models
{
    public class AvatarCostumeSet
    {
        [PrimaryKey]
        public int id { get; set; }
        public string avatarid { get; set; } = "";
        public bool avatarblueunlocked { get; set; } = false;
        public bool avatarblueredunlocked { get; set; } = false;
        public bool avatargreenunlocked { get; set; } = false;
        public bool avatarpinkunlocked { get; set; } = false;
        public bool avatarredunlocked { get; set; } = false;
        public bool avatarwhiteunlocked { get; set; } = false;
    }
}
