using SpeechCommanderAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCommands
{
    public class OpenVideoCommand : ISpeechCommand
    {
        public Microsoft.Speech.Recognition.GrammarBuilder GetGrammar()
        {
            throw new NotImplementedException();
        }

        public void Execute(Microsoft.Speech.Recognition.RecognitionResult result)
        {
            throw new NotImplementedException();
        }
    }
}
