using System;
using System.Collections.Generic;
using UnityOSC;

namespace Onix_Launchpad
{
    public class OSCManager
    {
        Queue<OSCMessage> _queue = new Queue<OSCMessage>();

        public OSCManager()
        {
            OSCHandler.Instance.Init();
            OSCHandler.Instance.server.PacketReceivedEvent += packetRecievedEvent;
        }

        public void Touch()
        {
            OSCHandler.Instance.SendMessageToClient("UnityOSC", "/Mx/fader/4203", 0);
        }

        void packetRecievedEvent(OSCServer sender, OSCPacket packet)
        {
            lock (_queue)
            {
                if (packet.IsBundle())
                {
                    var bundle = packet as OSCBundle;

                    foreach (object obj in bundle.Data)
                    {
                        OSCMessage msg = obj as OSCMessage;
                        try
                        {
                            DeviceAction deviceAction;
                            string action = msg.Address.Split('/')[1];
                            int data = int.Parse(msg.Address.Split('/')[2]);
                            if (action == "fader") deviceAction = DeviceAction.FaderSlider;
                            else return;

                            _queue.Enqueue(msg);
                            Programm.onDeviceEvent(Device.Onix, deviceAction, new int[1]); //TODO: msg Data 
                        }
                        catch { Console.WriteLine("Error on OSC Packet reading"); }
                }
                }
                else
                {
                    _queue.Enqueue(packet as OSCMessage);
                }
            }
        }
        public void processPacket(InputOutputItem item, int[] eventPacket)
        {
            if (item.deviceAction == DeviceAction.FaderSlider) OSCHandler.Instance.SendMessageToClient("UnityOSC", "/Mx/fader/" + item.inputParams[0], eventPacket[0]);
        }
    }
}
