using System;

namespace DataAccessLayer
{
    public sealed class Category
    {
        public int Id { get; set; }
        public string category { get; set; } = "";
        public override string ToString() => category;


    }

    public sealed class LeaderboardRow
    {
        public string name { get; set; } = "";
        public int score { get; set; }
        public int time { get; set; }
        public string category { get; set; } = "";
        public string difficulty { get; set; } = "";
    }
}
