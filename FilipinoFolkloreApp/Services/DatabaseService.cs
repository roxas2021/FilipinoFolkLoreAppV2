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
    }
}
