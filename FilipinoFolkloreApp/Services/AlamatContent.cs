using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

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
            public string NarratorBackground { get; set; } = "";
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
            public int MedalId { get; set; } = 0;
            public int PriceStars { get; set; } = 0;
            public string Category { get; set; } = "";
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
            new Narrator{ Id="tarsier", Name="Tarsier", Avatar="elements/tarsier.png",NarratorBackground ="Ako si Tarsy maliit lang ako, pero may malalaking mata na nakakatulong sa akin na makita kahit madilim ang paligid. Mula pagkabata, ako na ang bantay ng gubat tuwing gabi. Ako ang nagbabantay para siguraduhing ligtas ang lahat. Dito sa laro, tutulungan kitang makita ang mga bagay na mahirap makita kapag gabi at turuan kang maging matapang kahit madilim ang paligid. Pinili ako dahil gusto kong ipakita na kahit maliit ka, may malaking bagay kang magagawa kapag matapang ka at mapagmasid sa iyong paligid. Tara, sasamahan kita na maglakbay sa gabi.", PriceStars=0 },
            new Narrator{ Id="eagle",   Name="Agila",   Avatar="elements/eagle.png",NarratorBackground="Ako si Agie isang agila na malakas ang pakpak at mabilis lumipad sa himpapawid. Nakikita ko ang buong mundo mula sa taas ng langit. Palagi akong nagmamasid kung ano ang nangyayari mula sa itaas, para maprotektahan ang mga naninirahan sa gubat sa baba. Pinili mo ako para maging iyong gabay dahil gusto kitang turuan na maging matapang at laging handang tumulong sa oras ng problema. Dito sa laro, tutulungan kitang tingnan ang mga bagay mula sa malayo at mapag matyag, para lagi kang handa sa kahit anong mangyayari. Sama na, lipad tayo at alamin ang mga sikreto ng gubat",   PriceStars=50 },
            new Narrator{ Id="monkey",  Name="Unggoy",  Avatar="elements/monkey.png",NarratorBackground ="Ako si Makkie ang unggoy na laging masaya at malikot. Mahilig ako tumalon-talon sa mga puno at maghanap ng masasarap na saging na paborito kong prutas. Palagi akong naglalaro sa gubat at nakikisama sa iba pang mga hayop. Pinili mo ako para maging narrator dahil gusto kong ipakita sa'yo na ang gubat ay puno ng saya at magkaron ng mga kaibigan. Dito sa laro, tutulungan kitang maghanap ng mga kayamanan at matuto habang ikaw ay naglalaro. Halika samahan mo ako sa masayang pakikipagsapalaran sa gubat.",  PriceStars=100 },
        };
        // Replace the async methods with synchronous versions:

        public static string[] LoadSubtitles(string fileName)
        {
            try
            {
                using var stream = FileSystem.OpenAppPackageFileAsync(fileName).GetAwaiter().GetResult();
                using var reader = new StreamReader(stream);

                var subtitles = new List<string>();
                string? line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        // Replace literal \n with actual newline character
                        line = line.Replace("\\n", "\n");
                        subtitles.Add(line);
                    }
                }

                return subtitles.ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading subtitles from {fileName}: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        private static List<Slide> CreateSlidesFromSubtitles(string subtitleFile, string backgroundPrefix, string storyName)
        {
            var subtitles = LoadSubtitles(subtitleFile);
            var slides = new List<Slide>();

            for (int i = 0; i < subtitles.Length; i++)
            {
                slides.Add(new Slide
                {
                    Background = $"{backgroundPrefix}/{storyName}{i + 1}.png",
                    Subtitle = subtitles[i],
                    AudioByNarrator = new()
                    {
                        ["tarsier"] = $"{backgroundPrefix}_voices/tarsier/{storyName}{i + 1}_tarsier.mp3",
                        ["eagle"] = $"{backgroundPrefix}_voices/eagle/{storyName}{i + 1}_eagle.mp3",
                        ["monkey"] = $"{backgroundPrefix}_voices/monkey/{storyName}{i + 1}_monkey.mp3",
                    }
                });
            }

            return slides;
        }
        public static List<Story> Stories { get; } = new()
        {
            //Alamat
            //new Story{
            //    StoryIndex=1, Category = "alamat",
            //    Id="1_juan_tamad", Title="Juan Tamad", PriceStars=0, RewardStars=20, Thumb ="stories/juantamad/juan_tamad_thumb.png",
            //    Slides = new()
            //    {
            //        new Slide{
            //            Background="stories/juantamad/juan_tamad_scene1.png",
            //            Characters=new(){ "stories/juan/char_boy.png" },
            //            Subtitle="Si Juan ay naghihintay sa ilalim ng puno.",
            //            AudioByNarrator = new()
            //                    {
            //                        ["tarsier"] = "storiesalamat/juantamad/audio/juantamad_tarsier1.mp3",
            //                        ["eagle"]   = "storiesalamat/juantamad/audio/juantamad_tarsier1.mp3",
            //                        ["monkey"]  = "storiesalamat/juantamad/audio/juantamad_tarsier1.mp3",
            //                    }
            //            },
            //        new Slide{
            //            Background="stories/juantamad/juan_tamad_scene2.png",
            //            Characters=new(){ "stories/juan/char_boy.png","stories/juan/char_tarsier.png" },
            //            Subtitle="Inaabangan niya ang pagbagsak ng bunga.",
            //            AudioByNarrator = new()
            //                    {
            //                        ["tarsier"] = "storiesalamat/juantamad/audio/juantamad_tarsier2.mp3",
            //                        ["eagle"]   = "storiesalamat/juantamad/audio/juantamad_tarsier2.mp3",
            //                        ["monkey"]  = "storiesalamat/juantamad/audio/juantamad_tarsier2.mp3",
            //                    }
            //            },
            //    },
            //    Quiz = new()
            //    {
            //        new QuizQuestion{
            //            Prompt="Sino ang pangunahing tauhan?",
            //            ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
            //            CorrectIndex=0, TimeLimitSec=20
            //        }
            //    }
            //},
            
            new Story{
                StoryIndex=1, Category = "alamat", MedalId = 1,
                Id="1_juan_tamad", Title="Alamat ng Alimango", PriceStars=0, RewardStars=20, 
                Thumb ="storiesalamat/alamatngalimgango/iconalamatngalimango.png",
                Slides =  CreateSlidesFromSubtitles(
                "subs/alamatngalimango.txt","storiesalamat/alamatngalimango","alamatngalimango"
                ),
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
                StoryIndex =2,Category = "alamat", MedalId = 2,
                Id="2_alamatngbahaghari", Title="alamatngbahaghari", PriceStars=1, RewardStars=20,Thumb="storiesalamat/alamatngbahaghari/iconalamatngbahaghari.png",
                Slides =  CreateSlidesFromSubtitles(
                "subs/alamatngbahaghari.txt","storiesalamat/alamatngbahaghari","alamatngbahaghari"
                ),
                Quiz=new(){ new QuizQuestion{ Prompt="Ano ang paksa?",
                           ChoiceImages=new(){ "quiz/choco/a.png","quiz/choco/b.png","quiz/choco/c.png"},
                           CorrectIndex=1 } }
            },
            new Story{
                StoryIndex =3,Category = "alamat", MedalId = 3,
                Id="3_alamatngpinya", Title="Alamat ng Pinya", PriceStars=2, RewardStars=50,
                Thumb= "storiesalamat/alamatngpinya/iconalamatngpinya.png",
                Slides =  CreateSlidesFromSubtitles(
                "subs/alamatngpinya.txt","storiesalamat/alamatngpinya","alamatngpinya"
                ), // no subtitle example
               
                Quiz=new(){ new QuizQuestion{ Prompt="Sino ang bida?",
                           ChoiceImages=new(){ "quiz/maton/a.png","quiz/maton/b.png","quiz/maton/c.png"},
                           CorrectIndex=2 } }
            },
            new Story{
                StoryIndex=4,Category = "alamat", MedalId= 4,
                Id="4_batmayarawbuwanatbituin", Title="Bat may araw,buwan at bituin", PriceStars=3, RewardStars=20,
                Thumb="storiesalamat/batmayarawbuwanatbituin/iconbatmayarawbuwanatbituin.png",
                Slides =  CreateSlidesFromSubtitles(
                "subs/batmayarawbuwanatbituin.txt","storiesalamat/batmayarawbuwanatbituin","batmayarawbuwanatbituin"
                ),
                Quiz=new(){ new QuizQuestion{ Prompt="Anong prutas?",
                           ChoiceImages=new(){ "quiz/mangga/a.png","quiz/mangga/b.png","quiz/mangga/c.png"},
                           CorrectIndex=1 } }
            },
            new Story{
                StoryIndex= 5,Category = "alamat", MedalId = 5,
                Id="5_kungbakitmayulan", Title="Kung bakit may ulan", PriceStars=1, RewardStars=25,
                Thumb = "storiesalamat/kungbakitmayulan/iconkungbakitmayulan.png",
                Slides =  CreateSlidesFromSubtitles(
                "subs/kungbakitmayulan.txt","storiesalamat/kungbakitmayulan","kungbakitmayulan"
                ),
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },
            
            //Epiko
            new Story{
                StoryIndex =21,Category = "epiko", MedalId =6,
                Id="6_Bantugan", Title="Bantugan", PriceStars=1, RewardStars=25,Thumb ="storiesepiko/bantugan/iconbantugan.png",
                Slides =  CreateSlidesFromSubtitles(
                "subs/bantugan.txt","storiesepiko/bantugan","bantugan"
                ),
                Quiz = new()
                {
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    },
                    new QuizQuestion{
                        Prompt="Sino ang kontrabida sa kwento?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
            },
            new Story{
                StoryIndex = 22,
                Category = "epiko",
                Id = "7_hinilawod",
                Title = "hinilawod",
                MedalId = 7,
                PriceStars = 0,
                RewardStars = 50,
                Thumb = "storiesepiko/hinilawod/iconhinilawod.png",
                Slides =  CreateSlidesFromSubtitles(
                "subs/hinilawod.txt","storiesepiko/hinilawod","hinilawod"
                ),
                Quiz = new()
                {
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    },
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
            },
            new Story{
            StoryIndex = 23,
            Category = "epiko",
            Id = "8_ibalon",
            Title = "Ibalon",
            MedalId = 8,
            PriceStars = 1,
            RewardStars = 50,
            Thumb = "storiesepiko/ibalon/iconibalon.png",
            Slides =  CreateSlidesFromSubtitles(
                "subs/ibalon.txt","storiesepiko/ibalon","ibalon"
                ),
            Quiz=new(){ new QuizQuestion{ Prompt="Ano ang paksa?",
                           ChoiceImages=new(){ "quiz/choco/a.png","quiz/choco/b.png","quiz/choco/c.png"},
                           CorrectIndex=1 } }
        },
            new Story{
            StoryIndex = 24,
            Category = "epiko",
            Id = "9_manimimbin",
            Title = "Manimimbin",
            MedalId = 9,
            PriceStars = 0,
            RewardStars = 50,
            Thumb = "storiesepiko/manimimbin/iconmanimimbin.png",
            Slides =  CreateSlidesFromSubtitles(
                "subs/manimimbin.txt","storiesepiko/manimimbin","manimimbin"
                ),
            Quiz=new(){ new QuizQuestion{ Prompt="Ano ang paksa?",
                           ChoiceImages=new(){ "quiz/choco/a.png","quiz/choco/b.png","quiz/choco/c.png"},
                           CorrectIndex=1 } }
        },
        new Story{
                StoryIndex =25,Category = "epiko", MedalId = 10,
                Id="10_tudbulul", Title="tudbulul", PriceStars=2, RewardStars=20,
                Thumb = "storiesepiko/tudbulul/icontudbulul.png",
                Slides =  CreateSlidesFromSubtitles(
                "subs/tudbulul.txt","storiesepiko/tudbulul","tudbulul"
                ),

            },

            new Story{
                StoryIndex = 31,Category = "pabula", MedalId = 11,
                Id="11_angaralkayloro", Title="angaralkayloro", PriceStars=0, RewardStars=25,Thumb ="storiespabula/angaralkayloro/iconangaralkayloro.png",
                Slides =  CreateSlidesFromSubtitles(
                "subs/angaralkayloro.txt","storiespabula/angaralkayloro","angaralkayloro"
                ),
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
            StoryIndex = 32,
            Category = "pabula",
            Id = "12_angkabayoatkalabaw",
            Title = "Kabayo at Kalabaw",
            MedalId =  12,
            PriceStars = 0,
            RewardStars = 50,
            Thumb = "storiespabula/angkabayoatkalabaw/iconangkabayoatkalabaw.png",
            Slides =  CreateSlidesFromSubtitles(
                "subs/angkabayoatkalabaw.txt","storiespabula/angkabayoatkalabaw","angkabayoatkalabaw"
                ),
            Quiz = new()
                {
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    },
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
        },
            new Story{
                StoryIndex =33,Category = "pabula", MedalId = 13,
                Id="3_pagongatkuneho", Title="Pagong at Kuneho", PriceStars=1, RewardStars=20,
                Thumb = "storiespabula/pagongatkuneho/iconpagongatkuneho.png",
                Slides =  CreateSlidesFromSubtitles(
                "subs/pagongatkuneho.txt","storiespabula/pagongatkuneho","pagongatkuneho"
                ),
                 // no subtitle example
                Quiz=new(){ new QuizQuestion{ Prompt="Sino ang bida?",
                           ChoiceImages=new(){ "quiz/maton/a.png","quiz/maton/b.png","quiz/maton/c.png"},
                           CorrectIndex=2 } }
            },
            new Story{
            StoryIndex = 34,
            Category = "pabula",
            Id = "14_pagong_at_matsing",
            Title = "Pagong at Matsing",
            MedalId = 14,
            PriceStars = 1,
            RewardStars = 50,
            Thumb = "storiespabula/pagongatmatsing/iconpagongatmatsing.png",
            Slides =  CreateSlidesFromSubtitles(
                "subs/pagongatmatsing.txt","storiespabula/pagongatmatsing","pagongatmatsing"
                ),

            Quiz = new()
                {
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    },
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
        },
        
        new Story{
                StoryIndex =35,Category = "pabula", MedalId = 15,
                Id="3_tularansikawayan", Title="Tularan si Kawayan", PriceStars=1, RewardStars=20,
                Thumb = "storiespabula/tularansikawayan/icontularansikawayan.png",
                Slides =  CreateSlidesFromSubtitles(
                "subs/tularansikawayan.txt","storiespabula/tularansikawayan","tularansikawayan"
                ), // no subtitle example
                Quiz=new(){ new QuizQuestion{ Prompt="Sino ang bida?",
                           ChoiceImages=new(){ "quiz/maton/a.png","quiz/maton/b.png","quiz/maton/c.png"},
                           CorrectIndex=2 } }
            },

        };

        // ------- In-memory runtime (no DB yet) -------
        public static int Stars { get; set; } = 50;
        public static int Hearts { get; set; } = 3;  // daily refill later
        public static string SelectedNarratorId { get; set; } = "tarsier";
        public static string CurrentNarratorImage { get; set; } = "elements/tarsier.png"; // Global variable for narrator image
        
        // Narrator battery system
        public static int NarratorBattery { get; set; } = 3; // 3 = full battery
        public static DateTime LastNarratorUseTime { get; set; } = DateTime.MinValue;
        private static System.Threading.Timer? _narratorBatteryTimer;
        
        public static HashSet<string> UnlockedStories { get; } = new() { "1_juan_tamad" };
        public static HashSet<string> UnlockedNarrators { get; } = new() { "tarsier" };
        public static string category { get; set; } = "";

        // Narrator battery helper methods
        public static string GetNarratorBatteryImage()
        {
            return NarratorBattery switch
            {
                3 => "batteryfull.png",
                2 => "batterythreequarters.png",
                1 => "batteryquarter.png",
                _ => "batteryempty.png"
            };
        }

        public static bool CanUseNarrator()
        {
            CheckAndRefreshNarratorBattery();
            return NarratorBattery > 0;
        }

        public static async Task<bool> UseNarratorAsync()
        {
            if (!CanUseNarrator())
                return false;

            NarratorBattery--;
            LastNarratorUseTime = DateTime.Now;
            
            // Save to database
            await App.Database.UpdateNarratorBatteryAsync(NarratorBattery, LastNarratorUseTime);
            
            // Start 10-minute timer if battery is depleted
            if (NarratorBattery == 0)
            {
                StartNarratorBatteryRefreshTimer();
            }
            
            return true;
        }

        public static void CheckAndRefreshNarratorBattery()
        {
            if (NarratorBattery >= 3)
                return;

            if (LastNarratorUseTime == DateTime.MinValue)
                return;

            var timeSinceLastUse = DateTime.Now - LastNarratorUseTime;
            
            // Check if 10 minutes have passed since last use
            if (timeSinceLastUse.TotalMinutes >= 10)
            {
                // Reset to full battery after 10 minutes
                NarratorBattery = 3;
                LastNarratorUseTime = DateTime.MinValue; // Reset timer
                
                // Save to database asynchronously
                _ = Task.Run(async () => 
                {
                    try
                    {
                        await App.Database.UpdateNarratorBatteryAsync(NarratorBattery, LastNarratorUseTime);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to update narrator battery: {ex.Message}");
                    }
                });
                
                // Stop the timer since battery is full
                _narratorBatteryTimer?.Dispose();
                _narratorBatteryTimer = null;
            }
        }

        private static void StartNarratorBatteryRefreshTimer()
        {
            // Cancel existing timer if any
            _narratorBatteryTimer?.Dispose();
            
            // Create timer that checks every 30 seconds for better responsiveness
            _narratorBatteryTimer = new System.Threading.Timer(
                callback: _ => 
                {
                    CheckAndRefreshNarratorBattery();
                    
                    // If battery is full, dispose the timer
                    if (NarratorBattery >= 3)
                    {
                        _narratorBatteryTimer?.Dispose();
                        _narratorBatteryTimer = null;
                    }
                },
                state: null,
                dueTime: TimeSpan.FromSeconds(30),
                period: TimeSpan.FromSeconds(30)
            );
        }

        public static void InitializeNarratorBatteryTimer()
        {
            CheckAndRefreshNarratorBattery();
            
            if (NarratorBattery < 3)
            {
                StartNarratorBatteryRefreshTimer();
            }
        }

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

        private static readonly HashSet<string> _narratorDbCheckInFlight = new();
        public static string CurrentStoryId { get; set; } = "";
        public static bool MusicIsEnabled { get; set; } = true;
        
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
                            UnlockedNarrators.Add(id);
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

            return false;
        }

        // ------- Helpers -------
        public static Narrator CurrentNarrator => Narrators.First(n => n.Id == SelectedNarratorId);
        public static Story GetStory(string id) => Stories.First(s => s.Id == id);

        public static double NarratorVolume { get; set; } = 1.0;
        public static bool TrySpendStars(int amount)
        {
            if (CharacterHelper.CurrentStars < amount) return false;
            return true;
        }
    }
}
