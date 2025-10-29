using FilipinoFolkloreApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Services
{
    public class DatabaseService
    {
        readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Character>().Wait();
            _database.CreateTableAsync<StoryMonitored>().Wait();
        }

        public Task<Character> GetCharAsync()
        {
            return _database.Table<Character>()
                            .Where(a => a.Id == 1)
                            .FirstOrDefaultAsync();
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

    }
}
