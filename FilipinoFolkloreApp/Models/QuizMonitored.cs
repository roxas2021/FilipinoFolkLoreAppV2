using SQLite;
using System;

namespace FilipinoFolkloreApp.Models
{
    [Table("QuizMonitored")]
    public class QuizMonitored
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string StoryId { get; set; } = "";
        public int QuestionIndex { get; set; }
        public bool IsAnsweredCorrectly { get; set; } = false;
        public DateTime? AnsweredDate { get; set; }
    }
}
