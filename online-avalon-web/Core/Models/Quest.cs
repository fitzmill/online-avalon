using System;
using online_avalon_web.Core.Enums;

namespace online_avalon_web.Core.Models
{
    public class Quest
    {
        public Quest()
        {
        }

        public long QuestId { get; set; }
        public long GameId { get; set; }
        public int QuestNumber { get; set; }
        public QuestResultEnum? QuestResult { get; set; }

        public Game Game { get; set; }
    }
}
