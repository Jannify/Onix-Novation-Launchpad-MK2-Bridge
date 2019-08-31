using System.Collections.Generic;

namespace Onix_Launchpad
{
    class Programm
    {
        public static MainWindow mainWindow;
        public static OSCManager oscManager;
        public static MidiManager midiManager;

        private List<InputOutputItem> items = new List<InputOutputItem>();

        public List<InputOutputItem> Items { get => items;}

        public Programm()
        {
            loadSaveFile();
            oscManager = new OSCManager();
            midiManager = new MidiManager();
        }

        private void loadSaveFile()
        {
            //TODO: Jsonw + button registered
        }

        public static void onDeviceEvent(InputAction inputAction, int[] inputData)
        {

        }

    }

    public class InputOutputItem
    {
        readonly InputAction inputAction;
        readonly int[] inputData;
        readonly OutputAction outputAction;
        readonly int[] outputData;

        public InputOutputItem(InputAction _inputAction, int[] _inputData, OutputAction _outputAction, int[] _outputData)
        {
            inputAction = _inputAction;
            inputData = _inputData;
            outputAction = _outputAction;
            outputData = _outputData;
        }
    }

    public enum InputAction
    {
        FaderInput,
        FaderCueGo,
        FaderCueRelease,
        FaderCuePause
    }

    public enum OutputAction
    {
        FaderInput,
        FaderCueGo,
        FaderCueRelease,
        FaderCuePause
    }
}
