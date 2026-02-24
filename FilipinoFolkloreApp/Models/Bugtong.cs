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
        public bool IsAvailable { get; set; } = true;
    }

    public class BugtongMonitored
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string BugtongId { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public bool IsRewardClaimed { get; set; } = false;
        public DateTime? CompletedDate { get; set; }
    }

    
    public class BugtongMilestone
    {
        public int RequiredCorrect { get; set; }
        public int RewardStars { get; set; }
        public int MedalId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
