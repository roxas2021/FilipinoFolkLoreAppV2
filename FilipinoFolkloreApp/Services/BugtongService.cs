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
                    Name = "Balat Matulis",
                    Prompt = "Balat ko'y matulis, loob ko'y matamis,\nMasarap sa pagkain, lalo na sa pista't merienda.",
                    Answer = "PINYA",
                    Choices = new List<string> { "PINYA", "SAGING", "PAKWAN" },
                    RewardStars = 10,
                    MedalId = 16
                },
                new Bugtong
                {
                    Id = "bugtong_2",
                    Name = "Dalawang Kuko",
                    Prompt = "May dalawang kuko, sa dagat nakatira,\nLakad ko'y pasulong at pabalik, laging abala.",
                    Answer = "ALIMANGO",
                    Choices = new List<string> { "ALIMANGO", "GUNTING", "PAA" },
                    RewardStars = 10,
                    MedalId = 17
                },
                new Bugtong
                {
                    Id = "bugtong_3",
                    Name = "Pitong Kulay",
                    Prompt = "Lumilitaw ako pagkatapos ng ulan,\nPitong kulay ang saya, sa langit sumisikat.",
                    Answer = "BAHAGHARI",
                    Choices = new List<string> { "BAHAGHARI", "KULOG", "KIDLAT" },
                    RewardStars = 10,
                    MedalId = 18
                },
                new Bugtong
                {
                    Id = "bugtong_4",
                    Name = "Sumisikat sa Umaga",
                    Prompt = "Ako'y sumisikat tuwing umaga,\nNagbibigay liwanag at init sa lahat ng dako.",
                    Answer = "ARAW",
                    Choices = new List<string> { "ARAW", "ULAN", "BUWAN" },
                    RewardStars = 15,
                    MedalId = 19
                },
                new Bugtong
                {
                    Id = "bugtong_5",
                    Name = "Mula sa Ulap",
                    Prompt = "Mula sa ulap ako'y bumabagsak,\nHalaman at bulaklak ay ako'y ikinagagalak.",
                    Answer = "ULAN",
                    Choices = new List<string> { "ULAN", "HANGIN", "APOY" },
                    RewardStars = 15,
                    MedalId = 20
                },
                new Bugtong
                {
                    Id = "bugtong_6",
                    Name = "May Katawan, Walang Mukha",
                    Prompt = "May katawan, walang mukha,\nGinagamit sa pag-upo lagi na lang siya.",
                    Answer = "UPUAN",
                    Choices = new List<string> { "UPUAN", "MESA", "KAMA" },
                    RewardStars = 10,
                    MedalId = null
                },
                new Bugtong
                {
                    Id = "bugtong_7",
                    Name = "Bilog at Maliwanag",
                    Prompt = "Bilog ako, maliwanag,\nSa gabi ay nagbibigay-liwanag.",
                    Answer = "BUWAN",
                    Choices = new List<string> { "BUWAN", "ARAW", "ULAP" },
                    RewardStars = 10,
                    MedalId = null
                },
                new Bugtong
                {
                    Id = "bugtong_8",
                    Name = "May Gulong",
                    Prompt = "May gulong pero di sasakyan,\nPinapaikot para maglaro ang bataan.",
                    Answer = "YOYO",
                    Choices = new List<string> { "YOYO", "BOLA", "BISIKLETA" },
                    RewardStars = 15,
                    MedalId = null
                },
                new Bugtong
                {
                    Id = "bugtong_9",
                    Name = "May Pakpak",
                    Prompt = "May pakpak pero di lumilipad,\nSa tubig lang siya naglalakad.",
                    Answer = "PATO",
                    Choices = new List<string> { "PATO", "ISDA", "MANOK" },
                    RewardStars = 15,
                    MedalId = null
                },
                new Bugtong
                {
                    Id = "bugtong_10",
                    Name = "Buto't Balat",
                    Prompt = "Buto't balat,\nLumilipad.",
                    Answer = "SARANGGOLA",
                    Choices = new List<string> { "SARANGGOLA", "IBON", "EROPLANO" },
                    RewardStars = 20,
                    MedalId = null
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
