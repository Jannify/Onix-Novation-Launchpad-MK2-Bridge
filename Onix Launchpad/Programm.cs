using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Onix_Launchpad
{
    class Programm
    {
        //public static MainWindow mainWindow;
        public static OSCManager oscManager;
        public static MidiManager midiManager;

        private static List<InputOutputItem> items;

        public static List<InputOutputItem> Items { get => items; }

       private static string saveDataPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/data.json";

        static void Main(string[] args)
        {
            loadItemList();
            try { oscManager = new OSCManager(); }
            catch { Console.WriteLine("Failed to load OSC Manager."); }

            try { midiManager = new MidiManager(); }
            catch { Console.WriteLine("Failed to load OSC Manager."); }

            //saveItemList(); //Manual saveData
            Console.ReadKey();
        }

        private static void loadItemList()
        {
            try
            {
                string data = File.ReadAllText(saveDataPath);
                items = (List<InputOutputItem>)JsonConvert.DeserializeObject(data);
            }
            catch (Exception e) { Console.WriteLine("Unable to load Data" + e); }
        }
        private static void saveItemList()
        {
            try
            {
                string data = JsonConvert.SerializeObject(items);
                if (!File.Exists(saveDataPath)) File.Create(saveDataPath);
                File.WriteAllText(saveDataPath, data);
            }
            catch (Exception e) { Console.WriteLine("Unable to save Data" + e); }
        }

        ////private static void addInputOutputItem(DeviceAction deviceAction, int[] oscParams, int[] midiParams)
        ////{
        ////    items.Add(new InputOutputItem(deviceAction, oscParams, midiParams));
        ////    //midiManager.addButtonAction(deviceAction, midiParams);
        ////}

        /// <summary>
        /// Sends Event to target device.
        /// </summary>
        /// <param name="targetDevice">Target Device</param>
        /// <param name="deviceAction">Trigger</param>
        /// <param name="inputParams">Parameter of coresponding InputOutputItem</param>
        /// <param name="eventData">Data of the event.</param>
        public static void onDeviceEvent(Device targetDevice, DeviceAction deviceAction, int[] inputParams, object[] eventData)
        {
            try
            {
                InputOutputItem item = items.Where(x => x.deviceAction == deviceAction && (x.oscParams.SequenceEqual(inputParams) || x.midiParams.SequenceEqual(inputParams))).FirstOrDefault();
                if (item != null)
                {
                    if (targetDevice == Device.Onix) oscManager.processPacket(item, eventData);
                    else if (targetDevice == Device.Launchpad) midiManager.processPacket(item, eventData);
                }
            }
            catch { Console.WriteLine("Error in Event-Manager in Programm.cs"); }
        }

        public static void catchFatalException(string msg, Exception e)
        {
            Console.WriteLine(msg);
            Console.WriteLine(e);
            Console.WriteLine("Press any Key to close.");
            Console.ReadKey();
        }
    }

    public class InputOutputItem
    {
        public readonly DeviceAction deviceAction;
        public readonly int[] oscParams;
        public readonly int[] midiParams;

        /// <summary>
        /// Bidirectional Input-Output-Item. Links DeviceAction to Action-ID (Onix) and Button-Coords (Launchpad).
        /// </summary>
        /// <param name="_deviceAction">Trigger/Processor (Like FaderValueChange)</param>
        /// <param name="_oscParams">Button/Fader ID (starting with 4201)</param>
        /// <param name="_midiParams">Coords of Launchpad Button(s) [0]=x [1]=y [2]=yMax</param>
        public InputOutputItem(DeviceAction _deviceAction, int[] _oscParams, int[] _midiParams)
        {
            deviceAction = _deviceAction;
            oscParams = _oscParams;
            midiParams = _midiParams;
        }
    }
    public enum Device
    {
        Onix,
        Launchpad
    }
    public enum DeviceAction
    {
        None,
        FaderSlider,
        FaderCueGo,
        FaderCueRelease,
        FaderCuePause
    }
}
