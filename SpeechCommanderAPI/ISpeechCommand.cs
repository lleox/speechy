using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace SpeechCommanderAPI
{
    [InheritedExport]
    public interface ISpeechCommand
    {
        void Initialize(ISpeechEngine engine);

        Choices GetCommandActivators();
        GrammarBuilder GetCommandArgs();

        void Execute(RecognitionResult result);
    }
}