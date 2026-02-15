using FilipinoFolkloreApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace FilipinoFolkloreApp.Services
{
    public static class BugtongService
    {
        public static List<Bugtong> Bugtongs { get; private set; } = new();
        public static List<BugtongMilestone> Milestones { get; private set; } = new();

        static BugtongService()
        {
            InitializeBugtongs();
            InitializeMilestones();
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
                    RewardStars = 10
                },
                new Bugtong
                {
                    Id = "bugtong_2",
                    Name = "Dalawang Kuko",
                    Prompt = "May dalawang kuko, sa dagat nakatira,\nLakad ko'y pasulong at pabalik, laging abala.",
                    Answer = "ALIMANGO",
                    Choices = new List<string> { "ALIMANGO", "GUNTING", "PAA" },
                    RewardStars = 10
                },
                new Bugtong
                {
                    Id = "bugtong_3",
                    Name = "Pitong Kulay",
                    Prompt = "Lumilitaw ako pagkatapos ng ulan,\nPitong kulay ang saya, sa langit sumisikat.",
                    Answer = "BAHAGHARI",
                    Choices = new List<string> { "BAHAGHARI", "KULOG", "KIDLAT" },
                    RewardStars = 10
                },
                new Bugtong
                {
                    Id = "bugtong_4",
                    Name = "Sumisikat sa Umaga",
                    Prompt = "Ako'y sumisikat tuwing umaga,\nNagbibigay liwanag at init sa lahat ng dako.",
                    Answer = "ARAW",
                    Choices = new List<string> { "ARAW", "ULAN", "BUWAN" },
                    RewardStars = 15
                },
                new Bugtong
                {
                    Id = "bugtong_5",
                    Name = "Mula sa Ulap",
                    Prompt = "Mula sa ulap ako'y bumabagsak,\nHalaman at bulaklak ay ako'y ikinagagalak.",
                    Answer = "ULAN",
                    Choices = new List<string> { "ULAN", "HANGIN", "APOY" },
                    RewardStars = 15
                },
                new Bugtong
                {
                    Id = "bugtong_6",
                    Name = "May Katawan, Walang Mukha",
                    Prompt = "May katawan, walang mukha,\nGinagamit sa pag-upo lagi na lang siya.",
                    Answer = "UPUAN",
                    Choices = new List<string> { "UPUAN", "MESA", "KAMA" },
                    RewardStars = 10
                },
                new Bugtong
                {
                    Id = "bugtong_7",
                    Name = "Bilog at Maliwanag",
                    Prompt = "Bilog ako, maliwanag,\nSa gabi ay nagbibigay-liwanag.",
                    Answer = "BUWAN",
                    Choices = new List<string> { "BUWAN", "ARAW", "ULAP" },
                    RewardStars = 10
                },
                new Bugtong
                {
                    Id = "bugtong_8",
                    Name = "May Gulong",
                    Prompt = "May gulong pero di sasakyan,\nPinapaikot para maglaro ang bataan.",
                    Answer = "YOYO",
                    Choices = new List<string> { "YOYO", "BOLA", "BISIKLETA" },
                    RewardStars = 15
                },
                new Bugtong
                {
                    Id = "bugtong_9",
                    Name = "May Pakpak",
                    Prompt = "May pakpak pero di lumilipad,\nSa tubig lang siya naglalakad.",
                    Answer = "PATO",
                    Choices = new List<string> { "PATO", "ISDA", "MANOK" },
                    RewardStars = 15
                },
                new Bugtong
                {
                    Id = "bugtong_10",
                    Name = "Buto't Balat",
                    Prompt = "Buto't balat,\nLumilipad.",
                    Answer = "SARANGGOLA",
                    Choices = new List<string> { "SARANGGOLA", "IBON", "EROPLANO" },
                    RewardStars = 20
                },
                new Bugtong
                {
                    Id = "bugtong_11",
                    Name = "Lumalaki Pag Binugbog",
                    Prompt = "Lumalaki kapag binubugbog,\nSa hangin sumisigaw.",
                    Answer = "TAMBOL",
                    Choices = new List<string> { "TAMBOL", "BOLA", "KAHON" },
                    RewardStars = 10
                },
                new Bugtong
                {
                    Id = "bugtong_12",
                    Name = "Naghihintay sa Sulok",
                    Prompt = "Naghihintay sa sulok,\nNangingisay sa gitna.",
                    Answer = "WALIS",
                    Choices = new List<string> { "WALIS", "PALO", "SIKO" },
                    RewardStars = 10
                },
                new Bugtong
                {
                    Id = "bugtong_13",
                    Name = "Tahimik sa Umaga",
                    Prompt = "Tahimik kung umaga,\nMaingay kung gabi.",
                    Answer = "KULIGLIG",
                    Choices = new List<string> { "KULIGLIG", "ASO", "IBON" },
                    RewardStars = 15
                },
                new Bugtong
                {
                    Id = "bugtong_14",
                    Name = "Isang Bungkos",
                    Prompt = "Isang bungkos na bulaklak,\nSa langit nakabitin.",
                    Answer = "BITUIN",
                    Choices = new List<string> { "BITUIN", "ULAP", "ILAW" },
                    RewardStars = 15
                },
                new Bugtong
                {
                    Id = "bugtong_15",
                    Name = "Butas sa Gitna",
                    Prompt = "May butas sa gitna,\nPinag-uusapan ng bayan.",
                    Answer = "SINGSING",
                    Choices = new List<string> { "SINGSING", "BARYA", "BILOG" },
                    RewardStars = 20
                },
            };
        }

        private static void InitializeMilestones()
        {
            Milestones = new List<BugtongMilestone>
            {
                new BugtongMilestone
                {
                    RequiredCorrect = 10,
                    RewardStars = 50,
                    MedalId = 17,
                    Description = "Nakumpleto ang 10 bugtong!"
                },
                new BugtongMilestone
                {
                    RequiredCorrect = 13,
                    RewardStars = 75,
                    MedalId = 18,
                    Description = "Nakumpleto ang 13 bugtong!"
                },
                new BugtongMilestone
                {
                    RequiredCorrect = 15,
                    RewardStars = 100,
                    MedalId = 19,
                    Description = "Nakumpleto ang lahat ng bugtong!"
                }
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

        public static BugtongMilestone? GetAchievedMilestone(int correctCount)
        {
            return Milestones
                .Where(m => m.RequiredCorrect == correctCount)
                .FirstOrDefault();
        }
    }
}
