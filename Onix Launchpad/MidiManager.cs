using IntelOrca.Launchpad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Onix_Launchpad
{
    public class MidiManager
    {
        private LaunchpadDevice launchpad = new LaunchpadDevice();
        // Dictionary<int[], DeviceAction> keyActions = new Dictionary<int[], DeviceAction>(); //int[]: 0=x, 1=y, 3=yMax

        public MidiManager()
        {
            try
            {
                launchpad = new LaunchpadDevice();
                launchpad.DoubleBuffered = true;
                launchpad.ButtonPressed += packetRecievedEvent;

                Console.WriteLine("Launchpad found");
            }
            catch (Exception e)
            {
                Console.WriteLine("No launchpad found");
                Console.WriteLine(e);
            }
        }

        public void Close()
        {
            launchpad.Close();
        }

        void packetRecievedEvent(object sender, ButtonPressEventArgs eventArgs)
        {
            try
            {
                int[] coords = new int[] { eventArgs.X, eventArgs.Y };
                InputOutputItem item = Programm.Items.Where(x => x.midiParams[0] == coords[0] && x.midiParams[1] <= coords[1] && x.midiParams[2] >= coords[1]).FirstOrDefault();
                if (item != null)
                {
                    Programm.onDeviceEvent(Device.Onix, item.deviceAction, item.midiParams, new object[] { eventArgs.X, eventArgs.Y });
                }
            }
            catch { Console.WriteLine("Error on Midi Packet reading"); }
        }

        public void processPacket(InputOutputItem item, object[] eventPacket)
        {
            try
            {
                if (item.deviceAction == DeviceAction.FaderSlider)
                {
                    int ledToLit = (int)Math.Round((float)eventPacket[0] / 255f * (item.midiParams[2] - item.midiParams[1])) + 1;
                    for (int i = 0; i <= item.midiParams[2]; i++)
                    {
                        if (i < ledToLit) launchpad[item.midiParams[0], item.midiParams[1] + i].SetColor(ButtonColor.Grey);
                        else launchpad[item.midiParams[0], item.midiParams[1] + i].SetColor(ButtonColor.Off);
                    }
                }
                else if (item.deviceAction == DeviceAction.FaderCueGo)
                {
                    if (eventPacket[0].ToString() == "0") launchpad[item.midiParams[0], item.midiParams[1]].SetColor(ButtonColor.Red);
                    else launchpad[item.midiParams[0], item.midiParams[1]].SetColor(ButtonColor.Green);
                }
                else if (item.deviceAction == DeviceAction.FaderCueRelease) { Console.WriteLine("FaderCueRelease"); }
                else if (item.deviceAction == DeviceAction.FaderCueRelease) { Console.WriteLine("FaderCueRelease"); }
                else { Console.WriteLine("No Midi-Processor found for this packet."); }
            }
            catch { Console.WriteLine("Error on processing Packet for Midi."); }
        }

        //public void addButtonAction(DeviceAction deviceAction, int[] coords) => keyActions.Add(coords, deviceAction);
        //public void removeButtonAction(int[] coords) => keyActions.Remove(coords);

        public void setFader(int row, float value)
        {
            int cubes = (int)value * 8;
            for (int i = 0; i < cubes; i++)
            {
                launchpad.GetButtons()[i, row].SetColor(ButtonColor.Red);
            }
        }

        private InputOutputItem getCueGoItem(int itemNumber)
        {
            return Programm.Items.Where(x => x.deviceAction == DeviceAction.FaderCueGo && x.oscParams[0] == itemNumber).FirstOrDefault();
        }
    }
}