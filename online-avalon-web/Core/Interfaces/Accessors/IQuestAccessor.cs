using System;
using online_avalon_web.Core.Models;

namespace online_avalon_web.Core.Interfaces.Accessors
{
    public interface IQuestAccessor
    {
        void AddQuest(Quest quest);
        void UpdateQuest(Quest quest);
    }
}
