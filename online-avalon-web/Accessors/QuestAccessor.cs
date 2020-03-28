using System;
using online_avalon_web.Core;
using online_avalon_web.Core.Interfaces.Accessors;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Accessors
{
    public class QuestAccessor : IQuestAccessor
    {
        private readonly AvalonContext _avalonContext;
        public QuestAccessor(AvalonContext avalonContext)
        {
            _avalonContext = avalonContext;
        }

        public void AddQuest(Quest quest)
        {
            _avalonContext.Quests.Add(quest);
            _avalonContext.SaveChanges();
        }

        public void UpdateQuest(Quest quest)
        {
            _avalonContext.Quests.Update(quest);
            _avalonContext.SaveChanges();
        }
    }
}
