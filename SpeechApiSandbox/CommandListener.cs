using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using SpeechCommanderAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.IO;
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
        CompositionContainer _container;

        [ImportMany]
        public IEnumerable<ISpeechCommand> Commands { get; set; }

        SpeechRecognitionEngine engine;
        public RecognizerInfo Recognizer { get; private set; }

        public CommandListener(RecognizerInfo recognizer)
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(App).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(System.AppDomain.CurrentDomain.BaseDirectory));
            
            if (Directory.Exists("plugin"))
                catalog.Catalogs.Add(new DirectoryCatalog("plugin"));

            _container = new CompositionContainer(catalog);

            _container.ComposeParts(this);

            Recognizer = recognizer;
            engine = new SpeechRecognitionEngine(recognizer);
            engine.SetInputToDefaultAudioDevice();

            Choices commands = new Choices("flush dns", "speak");
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

            if (e.Result.Text == "speak")
            {
                SpeechSynthesizer synth = new SpeechSynthesizer();
                synth.SetOutputToDefaultAudioDevice();
                synth.Speak("Lorem ipsum dolor sit am.");

                synth.Dispose();
                synth = null;
            }
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