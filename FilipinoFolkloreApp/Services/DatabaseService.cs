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
        }

        public Task<Character> GetCharAsync()
        {
            return _database.Table<Character>()
                            .Where(c => c.Id == MAIN_CHAR_ID)
                            .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Ensures the single row with Id==1 exists. If it doesn't, create it (with Id = 1).
        /// Returns the character that exists/was created.
        /// </summary>
        /// 
        public async Task<List<Medals>> LoadMedalsAsync()
        {
            var dbMedals = await _database.Table<Medals>().ToListAsync();

            foreach (var medal in MedalHelper.Medals)
            {
                var dbEntry = dbMedals.FirstOrDefault(m => m.MedalId == medal.MedalId);
                if (dbEntry != null)
                {
                    // Copy monitored fields into the in-memory medal
                    medal.isUnlocked = dbEntry.isUnlocked;
                }
                else
                {
                    // Insert default if not found
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
            medal.TimeStamp = DateTime.UtcNow;
            await _database.UpdateAsync(medal);
        }
        public async Task<List<Medals>> GetMedalAsync()
        {
            var medals = await _database.Table<Medals>()
                                        .Where(c => c.isUnlocked == true)
                                        .ToListAsync();
            
            return medals;
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
                Id = MAIN_CHAR_ID,      // assign the ID explicitly so the single-row is always id==1
                name = "Player",
                currentavatar = string.Empty,
                points = 0,
                stars = 0
            };

            // InsertAsync will use the provided Id value
            await _database.InsertAsync(defaultChar).ConfigureAwait(false);

            // return the inserted row
            return await GetCharAsync().ConfigureAwait(false);
        }

        // ---------- Update operations (no id parameter) ----------

        /// <summary>
        /// Update currentavatar of the single character.
        /// </summary>
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

        /// <summary>
        /// Set stars to an absolute non-negative value.
        /// </summary>
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

        /// <summary>
        /// Add (or subtract if negative) a delta to stars. Ensures stars never go below 0.
        /// Returns updated character.
        /// </summary>
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

        /// <summary>
        /// Update avatar and change stars together (atomic from caller POV).
        /// </summary>
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

        /// <summary>
        /// Optional helper: reset the character back to defaults (keeps Id == 1).
        /// </summary>
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

                // If a row exists -> Update, otherwise Insert
                var existing = await GetCharAsync().ConfigureAwait(false);
                if (existing == null)
                {
                    await _database.InsertAsync(defaultChar).ConfigureAwait(false);
                }
                else
                {
                    // keep the Id==1 and update other fields
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

        /// <summary>
        /// Return everything (should be only one row) - useful for debugging.
        /// </summary>
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
                    // Copy monitored fields into the in-memory story
                    story.IsPurchased = dbEntry.IsPurchased;
                    story.Category = dbEntry.Category;
                    story.IsRewardClaimed = dbEntry.IsRewardClaimed;
                    story.NarratorEagleUnlocked = dbEntry.NarratorEagleUnlocked;
                    story.NarratorMonkeyUnlocked = dbEntry.NarratorMonkeyUnlocked;
                }
                else
                {
                    // Insert default if not found
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

            // ---- Sync global sets from the story monitored flags ----
            // Unlocked stories: include purchased OR free stories (PriceStars == 0).
            AlamatContent.UnlockedStories.Clear();
            foreach (var s in AlamatContent.Stories)
            {
                if (s.PriceStars == 0 || s.IsPurchased)
                    AlamatContent.UnlockedStories.Add(s.Id);
            }

            // Unlocked narrators: always include 'tarsier' (free). Add others if any monitored true.
            AlamatContent.UnlockedNarrators.Clear();
            AlamatContent.UnlockedNarrators.Add("tarsier");
            foreach (var s in AlamatContent.Stories)
            {
                if (s.NarratorEagleUnlocked) AlamatContent.UnlockedNarrators.Add("eagle");
                if (s.NarratorMonkeyUnlocked) AlamatContent.UnlockedNarrators.Add("monkey");
            }

            return AlamatContent.Stories;
        }
        // Update a story’s monitored data
        public async Task UpdateStoryAsync(AlamatContent.Story story)
        {
            var dbStory = await _database.FindAsync<StoryMonitored>(story.StoryIndex);
            if (dbStory != null)
            {
                // Update DB record
                dbStory.IsPurchased = story.IsPurchased;
                dbStory.Category = story.Category;
                dbStory.IsRewardClaimed = story.IsRewardClaimed;
                dbStory.NarratorEagleUnlocked = story.NarratorEagleUnlocked;
                dbStory.NarratorMonkeyUnlocked = story.NarratorMonkeyUnlocked;
                await _database.UpdateAsync(dbStory);
            }
            else
            {
                // Insert if missing
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

            // ---- Keep global in-memory sets in sync immediately ----
            // Story unlocked:
            if (story.IsPurchased || story.PriceStars == 0)
                AlamatContent.UnlockedStories.Add(story.Id);
            else
                AlamatContent.UnlockedStories.Remove(story.Id);

            // Narrator unlocks (global): if any story turn on narrator unlock, add it globally.
            if (story.NarratorEagleUnlocked) AlamatContent.UnlockedNarrators.Add("eagle");
            // If you want to allow turning narrator lock OFF (rare), you might need to recompute from all stories:
            else
            {
                // recompute presence
                if (!AlamatContent.Stories.Any(s => s.NarratorEagleUnlocked))
                    AlamatContent.UnlockedNarrators.Remove("eagle");
            }

            if (story.NarratorMonkeyUnlocked) AlamatContent.UnlockedNarrators.Add("monkey");
            else
            {
                if (!AlamatContent.Stories.Any(s => s.NarratorMonkeyUnlocked))
                    AlamatContent.UnlockedNarrators.Remove("monkey");
            }

            // ensure 'tarsier' always present
            AlamatContent.UnlockedNarrators.Add("tarsier");
        }
        public async Task<bool> IsAnyStoryNarratorUnlockedAsync(string narratorId)
        {
            if (string.IsNullOrEmpty(narratorId)) return false;
            // tarsier is always unlocked
            if (narratorId == "tarsier") return true;

            // Query the StoryMonitored table directly for any row that has the narrator flag set
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

        // get by avatarid (string)
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
                // try to find an existing record with the same avatarid
                var existing = await GetAvatarSetByAvatarIdAsync(set.avatarid);

                if (existing != null)
                {
                    // ensure we're updating the existing row (preserve its PK)
                    set.id = existing.id;
                    return await _database.UpdateAsync(set);
                }
                else
                {
                    // no existing record -> insert
                    return await _database.InsertAsync(set);
                }
            }
            catch (Exception)
            {
                // rethrow so calling code can inspect the exception details (or log here)
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
                // create default set if not exists
                set = new AvatarCostumeSet
                {
                    avatarid = avatarId
                };
            }

            // set the right flag
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
                    // unknown key - optionally throw or return false
                    return false;
            }

            await SaveAvatarSetAsync(set);
            return true;
        }

        // helper to check if costume unlocked
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
    }
}
