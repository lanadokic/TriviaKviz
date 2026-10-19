using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public sealed class ResultRepository
    {
        private readonly Db _db;

        public ResultRepository(Db db)
        {
            _db = db;
        }

        public async Task SaveResultAsync(LeaderboardRow row)
        {
            using (var conn = _db.OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                INSERT INTO dbo.TRIVIA_DOKIC
                    (name, score, time, category, difficulty)
                VALUES
                    (@name, @score, @time, @category, @difficulty);";

                cmd.Parameters.AddWithValue("@name", row.name);
                cmd.Parameters.AddWithValue("@score", row.score);

                // time je VARCHAR u bazi
                cmd.Parameters.AddWithValue("@time", row.time.ToString());

                cmd.Parameters.AddWithValue("@category", row.category);
                cmd.Parameters.AddWithValue("@difficulty", row.difficulty);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<LeaderboardRow>> GetLeaderboardAsync(int top)
        {
            var list = new List<LeaderboardRow>();

            using (var conn = _db.OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                SELECT TOP (@Top)
                    name, score, time, category, difficulty
                FROM dbo.TRIVIA_DOKIC
                ORDER BY score DESC;";

                cmd.Parameters.AddWithValue("@Top", top);

                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        var playTimeText = rdr["time"] == DBNull.Value ? "0" : rdr["time"].ToString();
                        if (!int.TryParse(playTimeText, out var playTimeMs))
                            playTimeMs = 0;

                        list.Add(new LeaderboardRow
                        {
                            name = rdr["name"]?.ToString() ?? "",
                            score = rdr["score"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["score"]),
                            time = playTimeMs,
                            category = rdr["category"]?.ToString() ?? "",
                            difficulty = rdr["difficulty"]?.ToString() ?? ""
                        });
                    }
                }
            }

            return list;
        }

        // KATEGORIJE - tablica kategorija
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var list = new List<Category>();

            using (var conn = _db.OpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, category FROM dbo.TRIVIA_KAT_DOKIC ORDER BY category;";

                using (var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (await rdr.ReadAsync())
                    {
                        list.Add(new Category
                        {
                            Id = rdr.GetInt32(0),
                            category = rdr.GetString(1)
                        });
                    }
                }
            }

            return list;
        }

        public async Task UpsertCategoriesAsync(IEnumerable<Category> categories)
        {
            if (categories == null) return;

            using (var conn = _db.OpenConnection())
            {
                foreach (var c in categories)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        IF EXISTS (SELECT 1 FROM dbo.TRIVIA_KAT_DOKIC WHERE Id = @Id)
                            UPDATE dbo.TRIVIA_KAT_DOKIC SET category = @category WHERE Id = @Id;
                        ELSE
                            INSERT INTO dbo.TRIVIA_KAT_DOKIC (Id, category) VALUES (@Id, @category);";

                        cmd.Parameters.AddWithValue("@Id", c.Id);
                        cmd.Parameters.AddWithValue("@category", c.category ?? "");

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }
    }
}
