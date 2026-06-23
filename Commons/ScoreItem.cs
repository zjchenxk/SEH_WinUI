using System.Collections.Generic;

namespace SEH.Commons
{
    public class ScoreItem
    {
        public enum ScoreItemType
        {
            Folder,
            File,
        }

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ScoreItemType Type { get; set; }
        public List<ScoreItem> Children { get; set; } = new List<ScoreItem>();
    }
}
