using System.Collections.Generic;

namespace Kubika.Saving
{
    [System.Serializable]
    public class PlayerProgress
    {
        public string lastLevelKubicode;
        public List<string> beatenLevels = new List<string>();
    }
}