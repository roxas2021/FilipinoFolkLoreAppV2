using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Services
{
    public static class AlamatContent
    {
        public class Narrator
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string Avatar { get; set; } = "";   // e.g., "narrators/tarsier.png"
            public int PriceStars { get; set; } = 0;   // 0 = free
        }

        public class Slide
        {
            public string Background { get; set; } = "";         // e.g., "stories/juan/scene1.png"
            public List<string> Characters { get; set; } = new();// layered sprites (front-most last)
            public string? Subtitle { get; set; }                 // null/empty = no subtitle

            public Dictionary<string, string> AudioByNarrator { get; set; } = new();
        }

        public class QuizQuestion
        {
            public string Prompt { get; set; } = "";
            public List<string> ChoiceImages { get; set; } = new(); // 3 images
            public int CorrectIndex { get; set; } = 0;              // 0..2
            public int TimeLimitSec { get; set; } = 20;
        }

        public class Story
        {
            public string Id { get; set; } = "";
            public string Title { get; set; } = "";
            public int PriceStars { get; set; } = 0;         // Story #1 free; others priced
            public int RewardStars { get; set; } = 20;       // per-story override
            public string Thumb = "";
            public List<Slide> Slides { get; set; } = new();
            public List<QuizQuestion> Quiz { get; set; } = new(); // supports multiple later
        }

        // ------- EDIT THESE: narrators, stories, slides, quiz -------
        public static List<Narrator> Narrators { get; } = new()
        {
            new Narrator{ Id="tarsier", Name="Tarsier", Avatar="elements/tarsier.png", PriceStars=0 },
            new Narrator{ Id="eagle",   Name="Agila",   Avatar="elements/eagle.png",   PriceStars=50 },
            new Narrator{ Id="monkey",  Name="Unggoy",  Avatar="elements/monkey.png",  PriceStars=100 },
        };

        public static List<Story> Stories { get; } = new()
        {
            new Story{
                Id="1_juan_tamad", Title="Juan Tamad", PriceStars=0, RewardStars=20, Thumb ="stories/juantamad/juan_tamad_thumb.png",
                Slides = new()
                {
                    new Slide{ 
                        Background="stories/juantamad/juan_tamad_scene1.png",
                        Characters=new(){ "stories/juan/char_boy.png" },
                        Subtitle="Si Juan ay naghihintay sa ilalim ng puno.",
                        AudioByNarrator = new()
                                {
                                    ["tarsier"] = "juantamad/audio/juantamad_tarsier1.mp3",
                                    ["eagle"]   = "juantamad/audio/juantamad_tarsier1.mp3",
                                    ["monkey"]  = "juantamad/audio/juantamad_tarsier1.mp3",
                                }
                        },
                    new Slide{ 
                        Background="stories/juantamad/juan_tamad_scene2.png",
                        Characters=new(){ "stories/juan/char_boy.png","stories/juan/char_tarsier.png" },
                        Subtitle="Inaabangan niya ang pagbagsak ng bunga.",
                        AudioByNarrator = new()
                                {
                                    ["tarsier"] = "juantamad/audio/juantamad_tarsier2.mp3",
                                    ["eagle"]   = "juantamad/audio/juantamad_tarsier2.mp3",
                                    ["monkey"]  = "juantamad/audio/juantamad_tarsier2.mp3",
                                }
                        },
                },
                Quiz = new()
                {
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "quiz/juan/a.png","quiz/juan/b.png","quiz/juan/c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
            },
            new Story{
                Id="2_chocolate", Title="Alamat ng Tsokolate", PriceStars=1, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/choco/s1.png", Subtitle="..." },
                              new Slide{ Background="stories/choco/s2.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Ano ang paksa?",
                           ChoiceImages=new(){ "quiz/choco/a.png","quiz/choco/b.png","quiz/choco/c.png"},
                           CorrectIndex=1 } }
            },
            new Story{
                Id="3_maton", Title="Alamat ng Maton", PriceStars=2, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/maton/s1.png", Subtitle="" } }, // no subtitle example
                Quiz=new(){ new QuizQuestion{ Prompt="Sino ang bida?",
                           ChoiceImages=new(){ "quiz/maton/a.png","quiz/maton/b.png","quiz/maton/c.png"},
                           CorrectIndex=2 } }
            },
            new Story{
                Id="4_mangga", Title="Alamat ng Mangga", PriceStars=3, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/mangga/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong prutas?",
                           ChoiceImages=new(){ "quiz/mangga/a.png","quiz/mangga/b.png","quiz/mangga/c.png"},
                           CorrectIndex=1 } }
            },
            new Story{
                Id="5_saging", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },
            new Story{
                Id="5_luya", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },new Story{
                Id="5_kamatis", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },new Story{
                Id="5_bawang", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },
        };

        // ------- In-memory runtime (no DB yet) -------
        public static int Stars { get; set; } = 50;
        public static int Hearts { get; set; } = 3;  // daily refill later
        public static string SelectedNarratorId { get; set; } = "tarsier";
        public static HashSet<string> UnlockedStories { get; } = new() { "1_juan_tamad" };
        public static HashSet<string> UnlockedNarrators { get; } = new() { "tarsier" };

        // ------- Helpers -------
        public static Narrator CurrentNarrator => Narrators.First(n => n.Id == SelectedNarratorId);
        public static Story GetStory(string id) => Stories.First(s => s.Id == id);
        public static bool IsStoryUnlocked(string id) =>
            UnlockedStories.Contains(id) || GetStory(id).PriceStars == 0;
        public static bool IsNarratorUnlocked(string id) =>
            UnlockedNarrators.Contains(id) || Narrators.First(n => n.Id == id).PriceStars == 0;
        public static bool TrySpendStars(int amount)
        {
            if (Stars < amount) return false;
            Stars -= amount; return true;
        }
    }
}
