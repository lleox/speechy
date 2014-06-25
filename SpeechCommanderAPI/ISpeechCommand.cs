using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechCommanderAPI
{
    [InheritedExport]
    public interface ISpeechCommand
    {
        GrammarBuilder GetGrammar();

        void Execute(RecognitionResult result);
    }
}