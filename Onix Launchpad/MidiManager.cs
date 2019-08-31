using IntelOrca.Launchpad;
using System;
using System.Collections.Generic;

namespace Onix_Launchpad
{
    public class MidiManager
    {
        private LaunchpadDevice launchpad = new LaunchpadDevice();
        private Dictionary<int[], InputAction> keyActions = new Dictionary<int[], InputAction>();

        public MidiManager()
        {
            try
            {
                launchpad = new LaunchpadDevice();
                launchpad.DoubleBuffered = true;
                launchpad.ButtonPressed

                Console.WriteLine("Launchpad found");
            }
            catch (Exception e)
            {
                Console.WriteLine("No launchpad found");
                Console.WriteLine(e);
            }
        }

        void packetRecievedEvent(ButtonPressEventArgs eventArgs)
        {
                        try
                        {
                int[] coords = new int[] { eventArgs.X, eventArgs.Y };
                            Programm.onDeviceEvent(keyActions[new int[] { eventArgs.X, eventArgs.Y }], new int[] { eventArgs.X, eventArgs.Y }); //TODO: msg Data 
                        }
                        catch { Console.WriteLine("Error on OSC Packet reading"); }
        }

        public void addButtonAction(int x, int y, InputAction inputAction) => keyActions.Add(new int[] { x, y }, inputAction);
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