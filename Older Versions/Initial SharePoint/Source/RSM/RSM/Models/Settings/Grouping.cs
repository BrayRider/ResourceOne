using System.Collections.Generic;

namespace RSM.Models.Settings
{
    public class Grouping
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public List<SettingModel> SettingCollection { get; set; }
    }
}