using IntelOrca.Launchpad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Onix_Launchpad
{
    public class MidiManager
    {
        private LaunchpadDevice launchpad = new LaunchpadDevice();
        private Dictionary<int[], DeviceAction> keyActions = new Dictionary<int[], DeviceAction>(); //int[]: 0=x, 1=y, 3=yMax

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

        void packetRecievedEvent(object sender, ButtonPressEventArgs eventArgs)
        {
            try
            {
                int[] coords = new int[] { eventArgs.X, eventArgs.Y };
                Programm.onDeviceEvent(Device.Launchpad, keyActions[new int[] { eventArgs.X, eventArgs.Y }], new int[] { eventArgs.X, eventArgs.Y }); //TODO: msg Data 
            }
            catch { Console.WriteLine("Error on OSC Packet reading"); }
        }

        public void processPacket(InputOutputItem item, int[] eventPacket)
        {
            if (item.deviceAction == DeviceAction.FaderSlider)
            {
                int[] barY = keyActions.Where(x => x.Value == DeviceAction.FaderSlider && x.Key[0] == item.outputParams[0]).Select(x => new int[] { x.Key[1], x.Key[2] }).FirstOrDefault();
                int ledToLit = (int)((barY[1] - barY[0]) / 100f * (float)eventPacket[0]);
                for (int i = 0; i < ledToLit; i++)
                {
                    launchpad[item.outputParams[0], barY[0] + i].SetColor(ButtonColor.Red);
                }
            }
        }

        public void addButtonAction(int x, int y, DeviceAction deviceAction) => keyActions.Add(new int[] { x, y }, deviceAction);
        public void removeButtonAction(int x, int y) => keyActions.Remove(new int[] { x, y });

        public void setFader(int row, float value)
        {
            int cubes = (int)value * 8;
            for (int i = 0; i < cubes; i++)
            {
                launchpad.GetButtons()[i, row].SetColor(ButtonColor.Red);
            }
        }
    }
}