using SQLite;
using System;

namespace FilipinoFolkloreApp.Models
{
    public class Bugtong
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public List<string> Choices { get; set; } = new();
        public int RewardStars { get; set; } = 10;
        public int? MedalId { get; set; } // Nullable - only some bugtongs have medals
        public bool IsAvailable { get; set; } = true;
    }

    /// <summary>
    /// Database-tracked entity for bugtong progress
    /// </summary>
    public class BugtongMonitored
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string BugtongId { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public bool IsRewardClaimed { get; set; } = false;
        public DateTime? CompletedDate { get; set; }
    }
}
