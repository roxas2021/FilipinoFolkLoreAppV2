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
            public string Avatar { get; set; } = "";   
            public int PriceStars { get; set; } = 0;   
        }

        public class Slide
        {
            public string Background { get; set; } = "";        
            public List<string> Characters { get; set; } = new();
            public string? Subtitle { get; set; }          

            public Dictionary<string, string> AudioByNarrator { get; set; } = new();
        }

        public class QuizQuestion
        {
            public string Prompt { get; set; } = "";
            public List<string> ChoiceImages { get; set; } = new();
            public int CorrectIndex { get; set; } = 0;          
            public int TimeLimitSec { get; set; } = 1000;
        }

        public class Story
        {
            public int StoryIndex { get; set; } = 0;
            public string Id { get; set; } = ""; // monitor
            public string Title { get; set; } = "";
            public int PriceStars { get; set; } = 0;       
            public int RewardStars { get; set; } = 20;   
            public string Thumb = "";
            public bool IsRewardClaimed = false; // monitor
            public bool IsPurchased = false; // monitor
            public bool NarratorEagleUnlocked = false; // monitor
            public bool NarratorMonkeyUnlocked = false; // monitor
            public List<Slide> Slides { get; set; } = new();
            public List<QuizQuestion> Quiz { get; set; } = new(); 
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
                StoryIndex=1, 
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
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
            },
            new Story{
                StoryIndex =2,
                Id="2_chocolate", Title="Alamat ng Tsokolate", PriceStars=1, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/choco/s1.png", Subtitle="..." },
                              new Slide{ Background="stories/choco/s2.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Ano ang paksa?",
                           ChoiceImages=new(){ "quiz/choco/a.png","quiz/choco/b.png","quiz/choco/c.png"},
                           CorrectIndex=1 } }
            },
            new Story{
                StoryIndex =3,
                Id="3_maton", Title="Alamat ng Maton", PriceStars=2, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/maton/s1.png", Subtitle="" } }, // no subtitle example
                Quiz=new(){ new QuizQuestion{ Prompt="Sino ang bida?",
                           ChoiceImages=new(){ "quiz/maton/a.png","quiz/maton/b.png","quiz/maton/c.png"},
                           CorrectIndex=2 } }
            },
            new Story{
                StoryIndex=4,
                Id="4_mangga", Title="Alamat ng Mangga", PriceStars=3, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/mangga/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong prutas?",
                           ChoiceImages=new(){ "quiz/mangga/a.png","quiz/mangga/b.png","quiz/mangga/c.png"},
                           CorrectIndex=1 } }
            },
            new Story{
                StoryIndex= 5,
                Id="5_saging", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },
            new Story{
                StoryIndex =6,
                Id="5_luya", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },new Story{
                StoryIndex = 6,
                Id="5_kamatis", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },new Story{
                StoryIndex =7,
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
        // Prefer checked sets but also fall back to story fields and price.
        public static bool IsStoryUnlocked(string id)
        {
            // fast-set check first
            if (UnlockedStories.Contains(id)) return true;

            // fallback: check story flags / free price
            var s = Stories.FirstOrDefault(x => x.Id == id);
            if (s == null) return false;

            bool unlocked = s.PriceStars == 0 || s.IsPurchased;

            // If story is unlocked by flags, ensure the HashSet mirrors that
            if (unlocked)
                UnlockedStories.Add(id);

            return unlocked;
        }
        // AlamatContent.cs (inside AlamatContent static class)

        private static readonly HashSet<string> _narratorDbCheckInFlight = new();
        public static string CurrentStoryId { get; set; } = "";

        public static bool IsNarratorUnlocked(string id)
        {
            // 'tarsier' is always available
            if (id == "tarsier") return true;

            // Fast in-memory check first
            if (UnlockedNarrators.Contains(id)) return true;

            // Fallback: check per-story monitored flags already in memory
            if (id == "eagle")
            {
                var currentStory = Stories.FirstOrDefault(s => s.Id == CurrentStoryId);
                return currentStory != null && currentStory.NarratorEagleUnlocked;
            }
            if (id == "monkey")
            {
                var currentStory = Stories.FirstOrDefault(s => s.Id == CurrentStoryId);
                return currentStory != null && currentStory.NarratorMonkeyUnlocked;
            }


            // If we haven't already launched a DB check for this narrator, start one in background.
            // This will update UnlockedNarrators if DB says any story has it unlocked.
            // We guard by _narratorDbCheckInFlight so we only query once concurrently per narrator.
            if (!_narratorDbCheckInFlight.Contains(id))
            {
                _narratorDbCheckInFlight.Add(id);
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var dbHas = await App.Database.IsAnyStoryNarratorUnlockedAsync(id);
                        if (dbHas)
                        {
                            // Update the in-memory set for fast lookups later
                            UnlockedNarrators.Add(id);

                            // Optionally: if you want UI to refresh immediately, you can
                            // publish a message here (MessagingCenter) or call a UI refresh handler.
                            // Example (uncomment if you use MessagingCenter):
                            // MessagingCenter.Send<object, string>(this, "NarratorUnlocked", id);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"IsNarratorUnlocked DB check failed for {id}: {ex}");
                    }
                    finally
                    {
                        _narratorDbCheckInFlight.Remove(id);
                    }
                });
            }

            // still locked for now (until DB check completes or a story flag shows unlock)
            return false;
        }


        // ------- Helpers -------
        public static Narrator CurrentNarrator => Narrators.First(n => n.Id == SelectedNarratorId);
        public static Story GetStory(string id) => Stories.First(s => s.Id == id);
        //public static bool IsStoryUnlocked(string id) =>
        //    UnlockedStories.Contains(id) || GetStory(id).PriceStars == 0;
        //public static bool IsNarratorUnlocked(string id) =>
        //    UnlockedNarrators.Contains(id) || Narrators.First(n => n.Id == id).PriceStars == 0;
        public static bool TrySpendStars(int amount)
        {
            if (Stars < amount) return false;
            Stars -= amount; return true;
        }
    }
}
