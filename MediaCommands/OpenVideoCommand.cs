using SpeechCommanderAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCommands
{
    public class OpenVideoCommand : ISpeechCommand
    {
        private ISpeechEngine _engine;

        public void Execute(System.Speech.Recognition.RecognitionResult result)
        {
            
        }

        public void Initialize(ISpeechEngine engine)
        {
            _engine = engine;
        }

        public System.Speech.Recognition.Choices GetCommandActivators()
        {
            return new System.Speech.Recognition.Choices("open video");
        }

        public System.Speech.Recognition.GrammarBuilder GetCommandArgs()
        {
            var gb = new System.Speech.Recognition.GrammarBuilder();
            gb.AppendDictation();
            return gb;
        }
    }
}