using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Onix_Launchpad
{
    class Programm
    {
        public static MainWindow mainWindow;
        public static OSCManager oscManager;
        public static MidiManager midiManager;

        private static List<InputOutputItem> items = new List<InputOutputItem>();

        public List<InputOutputItem> Items { get => items; }

        public Programm()
        {
            loadItemList();
            oscManager = new OSCManager();
            midiManager = new MidiManager();

            addInputOutputItem(Device.Onix, DeviceAction.FaderSlider, new int[] { 4203 }, new int[] { 1 });   
        }

        private void loadItemList()
        {
            try
            {
                string data = File.ReadAllText(System.Reflection.Assembly.GetExecutingAssembly().Location);
                items = (List<InputOutputItem>)JsonConvert.DeserializeObject(data);
            }
            catch { Console.WriteLine("Unable to load Data"); }
        }
        private void saveItemList()
        {
            try
            {
                string data = JsonConvert.SerializeObject(items);
                File.WriteAllText(System.Reflection.Assembly.GetExecutingAssembly().Location, data);
            }
            catch { Console.WriteLine("Unable to save Data"); }
        }

        private void addInputOutputItem(Device device, DeviceAction deviceAction, int[] inputParams, int[] outputParams)
        {
            items.Add(new InputOutputItem(device, deviceAction, inputParams, outputParams));
            if (device == Device.Launchpad) midiManager.addButtonAction(inputParams[0], inputParams[1], deviceAction);
        }

        public static void onDeviceEvent(Device device, DeviceAction deviceAction, int[] eventData)
        {
            InputOutputItem item = (InputOutputItem)items.Where(x => x.deviceAction == deviceAction && x.device == device);
            if (device == Device.Onix) midiManager.processPacket(item, eventData);
            else if (device == Device.Launchpad) oscManager.processPacket(item, eventData);
        }
    }

    public class InputOutputItem
    {
        public readonly Device device;
        public readonly DeviceAction deviceAction;
        public readonly int[] inputParams;
        public readonly int[] outputParams;

        /// <param name="_device">Input Device</param>
        /// <param name="_inputAction">Input Trigger Action</param>
        /// <param name="_inputData">Param from Input Trigger</param>
        /// <param name="_outputAction">Action to trigger</param>
        /// <param name="_outputData">Param for Action</param>
        public InputOutputItem(Device _device, DeviceAction _deviceAction, int[] _inputParams, int[] _outputParams)
        {
            device = _device;
            deviceAction = _deviceAction;
            inputParams = _inputParams;
            outputParams = _outputParams;
        }
    }
    public enum Device
    {
        Onix,
        Launchpad
    }
    public enum DeviceAction
    {
        FaderSlider,
        FaderCueGo,
        FaderCueRelease,
        FaderCuePause
    }
}
