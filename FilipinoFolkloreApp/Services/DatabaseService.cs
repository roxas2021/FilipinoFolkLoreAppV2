using FilipinoFolkloreApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace FilipinoFolkloreApp.Services
{
    public class DatabaseService
    {
        private const int MAIN_CHAR_ID = 1;
        readonly SQLiteAsyncConnection _database;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Character>().Wait();
            _database.CreateTableAsync<StoryMonitored>().Wait();
            _database.CreateTableAsync<AvatarCostumeSet>().Wait();
            _database.CreateTableAsync<Medals>().Wait();
            _database.CreateTableAsync<BugtongMonitored>().Wait();
            _database.CreateTableAsync<QuizMonitored>().Wait(); 
        }

        public Task<Character> GetCharAsync()
        {
            return _database.Table<Character>()
                            .Where(c => c.Id == MAIN_CHAR_ID)
                            .FirstOrDefaultAsync();
        }

        public async Task<List<Medals>> LoadMedalsAsync()
        {
            var dbMedals = await _database.Table<Medals>().ToListAsync();

            foreach (var medal in MedalHelper.Medals)
            {
                var dbEntry = dbMedals.FirstOrDefault(m => m.MedalId == medal.MedalId);
                if (dbEntry != null)
                {
                    medal.isUnlocked = dbEntry.isUnlocked;
                }
                else
                {
                    await _database.InsertAsync(new Medals
                    {
                        MedalId = medal.MedalId,
                        MedalName = medal.MedalName,
                        MedalDescription = medal.MedalDescription,
                        MedalImagePath = medal.MedalImagePath,
                        TimeStamp = DateTime.Now,
                        isUnlocked = medal.isUnlocked
                    });
                }
            }
            return MedalHelper.Medals;
        }

        public async Task UnlockMedalAsync(int medalId)
        {
            var medal = MedalHelper.Medals.FirstOrDefault(m => m.MedalId == medalId);
            if (medal == null || medal.isUnlocked) return;

            medal.isUnlocked = true;
            medal.TimeStamp = DateTime.Now;
            await _database.UpdateAsync(medal);
        }
        public async Task<List<Medals>> GetMedalAsync()
        {
            var medals = await _database.Table<Medals>()
                                        .Where(c => c.isUnlocked == true)
                                        .ToListAsync();
            
            return medals.OrderBy(c => c.TimeStamp).ToList();
        }
        public static string GetMedalImagePath(int medalId)
        {
            var medal = MedalHelper.Medals.FirstOrDefault(m => m.MedalId == medalId);
            return medal?.MedalImagePath ?? string.Empty;
        }

        public async Task<Character> EnsureMainCharacterExistsAsync()
        {
            var c = await GetCharAsync().ConfigureAwait(false);
            if (c != null) return c;

            var defaultChar = new Character
            {
                Id = MAIN_CHAR_ID,     
                name = "Player",
                currentavatar = string.Empty,
                points = 0,
                stars = 0
            };

            await _database.InsertAsync(defaultChar).ConfigureAwait(false);

            return await GetCharAsync().ConfigureAwait(false);
        }

        public async Task<Character> UpdateCurrentAvatarAsync(string newAvatar)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                var c = await GetCharAsync().ConfigureAwait(false);
                if (c == null) throw new InvalidOperationException("Character row missing — call EnsureMainCharacterExistsAsync() first.");

                c.currentavatar = newAvatar ?? string.Empty;
                await _database.UpdateAsync(c).ConfigureAwait(false);
                return c;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<Character> SetStarsAsync(int stars)
        {
            if (stars < 0) stars = 0;
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                var c = await GetCharAsync().ConfigureAwait(false);
                if (c == null) throw new InvalidOperationException("Character row missing — call EnsureMainCharacterExistsAsync() first.");

                c.stars = stars;
                await _database.UpdateAsync(c).ConfigureAwait(false);
                return c;
            }
            finally
            {
                _lock.Release();
            }
        }

        
        public async Task<Character> AddStarsAsync(int delta)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                var c = await GetCharAsync().ConfigureAwait(false);
                if (c == null) throw new InvalidOperationException("Character row missing — call EnsureMainCharacterExistsAsync() first.");

                long newStars = (long)c.stars + delta;
                if (newStars < 0) newStars = 0;
                c.stars = (int)newStars;

                await _database.UpdateAsync(c).ConfigureAwait(false);
                return c;
            }
            finally
            {
                _lock.Release();
            }
        }

        
        public async Task<Character> UpdateAvatarAndAddStarsAsync(string newAvatar, int starDelta)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                var c = await GetCharAsync().ConfigureAwait(false);
                if (c == null) throw new InvalidOperationException("Character row missing — call EnsureMainCharacterExistsAsync() first.");

                c.currentavatar = newAvatar ?? string.Empty;
                long newStars = (long)c.stars + starDelta;
                if (newStars < 0) newStars = 0;
                c.stars = (int)newStars;

                await _database.UpdateAsync(c).ConfigureAwait(false);
                return c;
            }
            finally
            {
                _lock.Release();
            }
        }

        
        public async Task<Character> ResetCharacterToDefaultsAsync()
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                var defaultChar = new Character
                {
                    Id = MAIN_CHAR_ID,
                    name = "Player",
                    currentavatar = string.Empty,
                    points = 0,
                    stars = 0
                };

                
                var existing = await GetCharAsync().ConfigureAwait(false);
                if (existing == null)
                {
                    await _database.InsertAsync(defaultChar).ConfigureAwait(false);
                }
                else
                {
                    
                    existing.name = defaultChar.name;
                    existing.currentavatar = defaultChar.currentavatar;
                    existing.points = defaultChar.points;
                    existing.stars = defaultChar.stars;
                    await _database.UpdateAsync(existing).ConfigureAwait(false);
                }

                return await GetCharAsync().ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }

       
        public Task<List<Character>> GetAllCharactersAsync()
        {
            return _database.Table<Character>().ToListAsync();
        }
        public Task<int> SaveCharAsync(Character word)
        {
            return _database.InsertAsync(word);
        }

        public Task<int> UpdateCharAsync(Character character)
        {
            return _database.UpdateAsync(character);
        }
        public async Task<List<AlamatContent.Story>> LoadStoriesAsync()
        {
            var dbStories = await _database.Table<StoryMonitored>().ToListAsync();

            foreach (var story in AlamatContent.Stories)
            {
                var dbEntry = dbStories.FirstOrDefault(s => s.StoryIndex == story.StoryIndex);
                if (dbEntry != null)
                {
                    
                    story.IsPurchased = dbEntry.IsPurchased;
                    story.Category = dbEntry.Category;
                    story.IsRewardClaimed = dbEntry.IsRewardClaimed;
                    story.NarratorEagleUnlocked = dbEntry.NarratorEagleUnlocked;
                    story.NarratorMonkeyUnlocked = dbEntry.NarratorMonkeyUnlocked;
                }
                else
                {
                    
                    await _database.InsertAsync(new StoryMonitored
                    {
                        StoryIndex = story.StoryIndex,
                        Category = story.Category,
                        IsPurchased = story.IsPurchased,
                        IsRewardClaimed = story.IsRewardClaimed,
                        NarratorEagleUnlocked = story.NarratorEagleUnlocked,
                        NarratorMonkeyUnlocked = story.NarratorMonkeyUnlocked
                    });
                }
            }

            
            AlamatContent.UnlockedStories.Clear();
            foreach (var s in AlamatContent.Stories)
            {
                if (s.PriceStars == 0 || s.IsPurchased)
                    AlamatContent.UnlockedStories.Add(s.Id);
            }

            
            AlamatContent.UnlockedNarrators.Clear();
            AlamatContent.UnlockedNarrators.Add("tarsier");
            foreach (var s in AlamatContent.Stories)
            {
                if (s.NarratorEagleUnlocked) AlamatContent.UnlockedNarrators.Add("eagle");
                if (s.NarratorMonkeyUnlocked) AlamatContent.UnlockedNarrators.Add("monkey");
            }

            return AlamatContent.Stories;
        }
        
        public async Task UpdateStoryAsync(AlamatContent.Story story)
        {
            var dbStory = await _database.FindAsync<StoryMonitored>(story.StoryIndex);
            if (dbStory != null)
            {
                
                dbStory.IsPurchased = story.IsPurchased;
                dbStory.Category = story.Category;
                dbStory.IsRewardClaimed = story.IsRewardClaimed;
                dbStory.NarratorEagleUnlocked = story.NarratorEagleUnlocked;
                dbStory.NarratorMonkeyUnlocked = story.NarratorMonkeyUnlocked;
                await _database.UpdateAsync(dbStory);
            }
            else
            {
                
                await _database.InsertAsync(new StoryMonitored
                {
                    StoryIndex = story.StoryIndex,
                    IsPurchased = story.IsPurchased,
                    Category = story.Category,
                    IsRewardClaimed = story.IsRewardClaimed,
                    NarratorEagleUnlocked = story.NarratorEagleUnlocked,
                    NarratorMonkeyUnlocked = story.NarratorMonkeyUnlocked
                });
            }

            
            if (story.IsPurchased || story.PriceStars == 0)
                AlamatContent.UnlockedStories.Add(story.Id);
            else
                AlamatContent.UnlockedStories.Remove(story.Id);

            
            if (story.NarratorEagleUnlocked) AlamatContent.UnlockedNarrators.Add("eagle");
            
            else
            {
                
                if (!AlamatContent.Stories.Any(s => s.NarratorEagleUnlocked))
                    AlamatContent.UnlockedNarrators.Remove("eagle");
            }

            if (story.NarratorMonkeyUnlocked) AlamatContent.UnlockedNarrators.Add("monkey");
            else
            {
                if (!AlamatContent.Stories.Any(s => s.NarratorMonkeyUnlocked))
                    AlamatContent.UnlockedNarrators.Remove("monkey");
            }

            
            AlamatContent.UnlockedNarrators.Add("tarsier");
        }
        public async Task<bool> IsAnyStoryNarratorUnlockedAsync(string narratorId)
        {
            if (string.IsNullOrEmpty(narratorId)) return false;
            
            if (narratorId == "tarsier") return true;

           
            switch (narratorId)
            {
                case "eagle":
                    return await _database.Table<StoryMonitored>()
                                          .Where(s => s.NarratorEagleUnlocked == true)
                                          .FirstOrDefaultAsync() != null;
                case "monkey":
                    return await _database.Table<StoryMonitored>()
                                          .Where(s => s.NarratorMonkeyUnlocked == true)
                                          .FirstOrDefaultAsync() != null;
                default:
                    return false;
            }
        }
        public Task<List<AvatarCostumeSet>> GetAllAvatarSetsAsync()
        {
            return _database.Table<AvatarCostumeSet>().ToListAsync();
        }

        
        public Task<AvatarCostumeSet> GetAvatarSetByAvatarIdAsync(string avatarId)
        {
            return _database.Table<AvatarCostumeSet>()
                            .Where(a => a.avatarid == avatarId)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> SaveAvatarSetAsync(AvatarCostumeSet set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));
            if (string.IsNullOrWhiteSpace(set.avatarid)) throw new ArgumentException("avatarid required", nameof(set.avatarid));

            try
            {
               
                var existing = await GetAvatarSetByAvatarIdAsync(set.avatarid);

                if (existing != null)
                {
                    
                    set.id = existing.id;
                    return await _database.UpdateAsync(set);
                }
                else
                {
                    
                    return await _database.InsertAsync(set);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public async Task<bool> UnlockCostumeAsync(string avatarId, string costumeKey)
        {
            if (string.IsNullOrWhiteSpace(avatarId)) throw new ArgumentNullException(nameof(avatarId));
            if (string.IsNullOrWhiteSpace(costumeKey)) throw new ArgumentNullException(nameof(costumeKey));

            var set = await GetAvatarSetByAvatarIdAsync(avatarId);

            if (set == null)
            {
               
                set = new AvatarCostumeSet
                {
                    avatarid = avatarId
                };
            }

            
            switch (costumeKey.ToLowerInvariant())
            {
                case "avatarblue":
                    set.avatarblueunlocked = true;
                    break;
                case "avatarbluered":
                    set.avatarblueredunlocked = true;
                    break;
                case "avatargreen":
                    set.avatargreenunlocked = true;
                    break;
                case "avatarpink":
                    set.avatarpinkunlocked = true;
                    break;
                case "avatarred":
                    set.avatarredunlocked = true;
                 
                    break;
                default:
                    
                    return false;
            }

            await SaveAvatarSetAsync(set);
            return true;
        }

        
        public async Task<bool> IsCostumeUnlockedAsync(string avatarId, string costumeKey)
        {
            var set = await GetAvatarSetByAvatarIdAsync(avatarId);
            if (set == null) return false;

            switch (costumeKey.ToLowerInvariant())
            {
                case "avatarblue": return set.avatarblueunlocked;
                case "avatarbluered": return set.avatarblueredunlocked;
                case "avatargreen": return set.avatargreenunlocked;
                case "avatarpink": return set.avatarpinkunlocked;
                case "avatarred": return set.avatarredunlocked;
                default: return false;
            }
        }

        public async Task<Character> UpdateSelectedNarratorAsync(string narratorId)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                var c = await GetCharAsync().ConfigureAwait(false);
                if (c == null) throw new InvalidOperationException("Character row missing — call EnsureMainCharacterExistsAsync() first.");

                c.selectedNarrator = narratorId ?? "tarsier";
                await _database.UpdateAsync(c).ConfigureAwait(false);
                return c;
            }
            finally
            {
                _lock.Release();
            }
        }

       
        public async Task<Character> UpdateNarratorBatteryAsync(int battery, DateTime lastUseTime)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                var c = await GetCharAsync().ConfigureAwait(false);
                if (c == null) throw new InvalidOperationException("Character row missing — call EnsureMainCharacterExistsAsync() first.");

                c.narratorBattery = Math.Clamp(battery, 0, 3);
                c.lastNarratorUseTime = lastUseTime;
                await _database.UpdateAsync(c).ConfigureAwait(false);
                return c;
            }
            finally
            {
                _lock.Release();
            }
        }

        
        public async Task LoadNarratorDataAsync()
        {
            var c = await GetCharAsync().ConfigureAwait(false);
            if (c != null)
            {
                AlamatContent.SelectedNarratorId = c.selectedNarrator ?? "tarsier";
                AlamatContent.CurrentNarratorImage = AlamatContent.Narrators
                    .FirstOrDefault(n => n.Id == AlamatContent.SelectedNarratorId)?.Avatar ?? "elements/tarsier.png";
                AlamatContent.NarratorBattery = c.narratorBattery;
                AlamatContent.LastNarratorUseTime = c.lastNarratorUseTime;
                
                AlamatContent.InitializeNarratorBatteryTimer();
            }
        }

       
        public async Task<List<Bugtong>> LoadBugtongsAsync()
        {
            var dbBugtongs = await _database.Table<BugtongMonitored>().ToListAsync();

            foreach (var bugtong in BugtongService.Bugtongs)
            {
                var dbEntry = dbBugtongs.FirstOrDefault(b => b.BugtongId == bugtong.Id);
                if (dbEntry == null)
                {
                    await _database.InsertAsync(new BugtongMonitored
                    {
                        BugtongId = bugtong.Id,
                        IsCompleted = false,
                        IsRewardClaimed = false
                    });
                }
            }

            return BugtongService.Bugtongs;
        }

        public async Task<bool> IsBugtongCompletedAsync(string bugtongId)
        {
            var bugtong = await _database.Table<BugtongMonitored>()
                                 .Where(b => b.BugtongId == bugtongId)
                                 .FirstOrDefaultAsync();
            return bugtong?.IsCompleted ?? false;
        }

        public async Task SetBugtongCompletedAsync(string bugtongId)
        {
            var bugtong = await _database.Table<BugtongMonitored>()
                                 .Where(b => b.BugtongId == bugtongId)
                                 .FirstOrDefaultAsync();
    
            if (bugtong != null)
            {
                bugtong.IsCompleted = true;
                bugtong.CompletedDate = DateTime.Now;
                await _database.UpdateAsync(bugtong);
            }
            else
            {
                await _database.InsertAsync(new BugtongMonitored
                {
                    BugtongId = bugtongId,
                    IsCompleted = true,
                    IsRewardClaimed = true,
                    CompletedDate = DateTime.Now
                });
            }
        }

        
        public async Task<bool> IsQuizQuestionAnsweredAsync(string storyId, int questionIndex)
        {
            var quiz = await _database.Table<QuizMonitored>()
                                 .Where(q => q.StoryId == storyId && q.QuestionIndex == questionIndex)
                                 .FirstOrDefaultAsync();
            return quiz?.IsAnsweredCorrectly ?? false;
        }

        
        public async Task<bool> SetQuizQuestionAnsweredAsync(string storyId, int questionIndex)
        {
            var quiz = await _database.Table<QuizMonitored>()
                                 .Where(q => q.StoryId == storyId && q.QuestionIndex == questionIndex)
                                 .FirstOrDefaultAsync();
    
            if (quiz != null)
            {
                
                if (quiz.IsAnsweredCorrectly)
                    return false;
                
                quiz.IsAnsweredCorrectly = true;
                quiz.AnsweredDate = DateTime.Now;
                await _database.UpdateAsync(quiz);
                return true;
            }
            else
            {
                
                await _database.InsertAsync(new QuizMonitored
                {
                    StoryId = storyId,
                    QuestionIndex = questionIndex,
                    IsAnsweredCorrectly = true,
                    AnsweredDate = DateTime.Now
                });
                return true;
            }
        }
    }
}
