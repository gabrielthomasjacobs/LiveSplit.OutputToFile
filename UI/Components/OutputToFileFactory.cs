using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class OutputToFileFactory : IComponentFactory
    {
        public string ComponentName => "Output To File";

        public string Description => "Output information to a .txt file";

        public ComponentCategory Category => ComponentCategory.Other;

        public IComponent Create(LiveSplitState state) => new OutputToFile();

        public string UpdateName => ComponentName;

        public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.OutputToFile.xml";

        public string UpdateURL => "http://livesplit.org/update/";

        public Version Version => Version.Parse("0.0.1");
    }
}
