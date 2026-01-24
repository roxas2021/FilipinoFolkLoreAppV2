    using FilipinoFolkloreApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace FilipinoFolkloreApp.Services
{
    public static class BugtongService
    {
        public static List<Bugtong> Bugtongs { get; private set; } = new();

        static BugtongService()
        {
            InitializeBugtongs();
        }

        private static void InitializeBugtongs()
        {
            Bugtongs = new List<Bugtong>
            {
                new Bugtong
                {
                    Id = "bugtong_1",
                    Name = "Bahay ni Tandang",
                    Prompt = "Bahay ni Tandang, walang pintuan.",
                    Answer = "ITLOG",
                    Choices = new List<string> { "ITLOG", "BAHAY", "PINTO" },
                    RewardStars = 10,
                    MedalId = 16
                },
                new Bugtong
                {
                    Id = "bugtong_2",
                    Name = "Isang Butil ng Palay",
                    Prompt = "Isang butil ng palay, sakop ang buong bahay.",
                    Answer = "ILAW",
                    Choices = new List<string> { "ILAW", "PALAY", "KANDILA" },
                    RewardStars = 10,
                    MedalId = 17
                },
                new Bugtong
                {
                    Id = "bugtong_3",
                    Name = "Dalawang Balon",
                    Prompt = "Dalawang balon, hindi malingon.",
                    Answer = "MATA",
                    Choices = new List<string> { "MATA", "BALON", "ILONG" },
                    RewardStars = 10,
                    MedalId = 18 // First bugtong medal
                },
                new Bugtong
                {
                    Id = "bugtong_4",
                    Name = "Kung Kailan Pinatay",
                    Prompt = "Kung kailan pinatay, saka pa lumakad.",
                    Answer = "GULONG",
                    Choices = new List<string> { "GULONG", "KOTSE", "BISIKLETA" },
                    RewardStars = 15,
                    MedalId = 19
                },
                new Bugtong
                {
                    Id = "bugtong_5",
                    Name = "Limang Puno ng Niyog",
                    Prompt = "Limang puno ng niyog, isa lang ang bunga.",
                    Answer = "KAMAY",
                    Choices = new List<string> { "KAMAY", "NIYOG", "DALIRI" },
                    RewardStars = 15,
                    MedalId = 20 // Second bugtong medal
                },
            };
        }

        public static Bugtong? GetBugtong(string bugtongId)
        {
            return Bugtongs.FirstOrDefault(b => b.Id == bugtongId);
        }

        public static List<string> ShuffleChoices(List<string> choices)
        {
            var random = new Random();
            return choices.OrderBy(x => random.Next()).ToList();
        }
    }
}
