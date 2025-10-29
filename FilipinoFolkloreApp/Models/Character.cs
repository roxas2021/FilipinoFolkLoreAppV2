using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Models
{
    public class Character
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string name { get; set; } = "";
        public string avatar { get; set; } = "";
        public int points { get; set; }
        public int stars { get; set; } = 0;
    }
}
