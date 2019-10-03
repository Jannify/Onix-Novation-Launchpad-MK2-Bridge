using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityOSC;

namespace Onix_Launchpad
{
    public class OSCManager
    {
        public OSCManager()
        {
            OSCHandler.Instance.Init();

            Task<bool> updateTask = Update();
        }

        async Task<bool> Update()
        {
            while (true)
            {
                while(OSCHandler.Instance.reciever.hasWaitingMessages())
                {
                    processOSCMessage(OSCHandler.Instance.reciever.getNextMessage());
                }

                await Task.Delay(5); // arbitrary delay
            }
        }

        private void processOSCMessage(OSCMessage message)
        {
            Console.WriteLine(string.Format("message received: {0} {1}", message.Address, ListToString(message.Data)));

            DeviceAction deviceAction = DeviceAction.None;
            int data = -1;
            if (message.Address.Contains("Text")) return;
            try
            {
                string[] address = message.Address.Split('/');
                string action = address[2];
                data = int.Parse(address[3]);
                if (action == "fader" && address.Length == 4) deviceAction = DeviceAction.FaderSlider;
                else return;
            }
            catch { Console.WriteLine("Error on OSC Packet processing. Unknow packet?"); }

            Programm.onDeviceEvent(Device.Launchpad, deviceAction, new int[] { data }, message.Data.ToArray());
        }
        public void processPacket(InputOutputItem item, object[] eventPacket)
        {
            if (item.deviceAction == DeviceAction.FaderSlider)
            {
                int value;
                int valuewdwdw;
                int buttonY = int.Parse(eventPacket[1].ToString());

                OSCHandler.Instance.SendMessageToClient("UnityOSC", "/Mx/fader/" + item.oscParams[0], (int)Math.Round(buttonY / (float)item.midiParams[2] * 255));
            }
        }

        private string ListToString(List<object> input)
        {
            string output = "";

            foreach (object value in input)
            {
                output += value.ToString();
            }

            return output;
        }
    }
}
