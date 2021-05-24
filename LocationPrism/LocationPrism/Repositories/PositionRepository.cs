using LocationPrism.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LocationPrism.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private SQLiteAsyncConnection connection;

        private async Task CreateConnection()
        {
            if (connection != null)
            {
                return;
            }

            var databasePath =
            Path.Combine(Environment.GetFolderPath
            (Environment.SpecialFolder.MyDocuments), "Positions.db");

            connection = new SQLiteAsyncConnection(databasePath);
            await connection.CreateTableAsync<Position>();
        }

        public async Task Save(Position position)
        {
            await CreateConnection();
            await connection.InsertAsync(position);
        }

        public async Task<List<Position>> GetAll()
        {
            await CreateConnection();

            var positions = await connection.Table<Position>
            ().ToListAsync();

            return positions;
        }

        public async Task Clear()
        {
            await CreateConnection();
            await connection.DropTableAsync<Position>();
        }
    }
}
