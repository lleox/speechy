using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpeechApiSandbox
{
    class SpeechEngineManager
    {
        static SpeechEngineManager()
        {
            Recognizers = SpeechRecognitionEngine.InstalledRecognizers();
        }

        public static ReadOnlyCollection<RecognizerInfo> Recognizers { get; set; }
    }

    class LogEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public LogEventArgs(string msg)
	    {
            Message = msg;
	    }
    }

    class CommandListener : IDisposable
    {
        SpeechRecognitionEngine engine;
        public RecognizerInfo Recognizer { get; private set; }

        public CommandListener(RecognizerInfo recognizer)
        {
            Recognizer = recognizer;
            engine = new SpeechRecognitionEngine(recognizer);
            engine.SetInputToDefaultAudioDevice();

            Choices commands = new Choices("flush dns");
            GrammarBuilder gb = new GrammarBuilder(commands);
            Grammar g = new Grammar(gb);

            engine.LoadGrammar(g);

            engine.SpeechRecognized += sre_SpeechRecognized;
            engine.SpeechDetected += engine_SpeechDetected;
            engine.SpeechHypothesized += engine_SpeechHypothesized;
            engine.SpeechRecognitionRejected += engine_SpeechRecognitionRejected;

            engine.RecognizeAsync(RecognizeMode.Multiple);
        }

        void engine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            RaiseLogEvent("Rejected " + e.Result.Text);
        }

        void engine_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            RaiseLogEvent("Hypothesized " + e.Result.Text);
        }

        void engine_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            RaiseLogEvent("Detected");
        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            RaiseLogEvent("Recognized " + e.Result.Text);
        }

        public event EventHandler<LogEventArgs> LogEvent;

        protected void RaiseLogEvent(string logMsg)
        {
            if (LogEvent != null)
            {
                LogEvent(this, new LogEventArgs(logMsg));
            }
        }

        public void Dispose()
        {
            if (engine != null)
            {
                engine.SpeechRecognized -= sre_SpeechRecognized;
                engine.SpeechDetected -= engine_SpeechDetected;
                engine.SpeechHypothesized -= engine_SpeechHypothesized;
                engine.SpeechRecognitionRejected -= engine_SpeechRecognitionRejected;

                engine.Dispose();
                engine = null;
            }
        }
    }
}