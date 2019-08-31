using Midi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntelOrca.Launchpad
{
    public enum ToolbarButton { Up, Down, Left, Right, Session, User1, User2, Mixer }
    public enum SideButton { Volume, Pan, SendA, SendB, Stop, Mute, Solo, Arm }

    public class LaunchpadDevice
    {
        private InputDevice mInputDevice;
        private OutputDevice mOutputDevice;

        private bool mDoubleBuffered;
        private bool mDoubleBufferedState;

        private readonly LaunchpadButton[] mToolbar = new LaunchpadButton[8];
        private readonly LaunchpadButton[] mSide = new LaunchpadButton[8];
        private readonly LaunchpadButton[,] mGrid = new LaunchpadButton[8, 8];

        public event EventHandler<ButtonPressEventArgs> ButtonPressed;

        public LaunchpadDevice() : this(0) { }

        public LaunchpadDevice(int index)
        {
            InitialiseButtons();

            int i = 0;
            mInputDevice = InputDevice.InstalledDevices.Where(x => x.Name.Contains("Launchpad")).
                FirstOrDefault(x => i++ == index);
            i = 0;
            mOutputDevice = OutputDevice.InstalledDevices.Where(x => x.Name.Contains("Launchpad")).
                FirstOrDefault(x => i++ == index);

            if (mInputDevice == null)
                throw new LaunchpadException("Unable to find input device.");
            if (mOutputDevice == null)
                throw new LaunchpadException("Unable to find output device.");

            mInputDevice.Open();
            mOutputDevice.Open();

            mInputDevice.StartReceiving(new Clock(120));
            mInputDevice.NoteOn += mInputDevice_NoteOn;
            mInputDevice.ControlChange += mInputDevice_ControlChange;

            Reset();
        }

        private void InitialiseButtons()
        {
            for (int i = 0; i < 8; i++)
            {
                mToolbar[i] = new LaunchpadButton(this, ButtonType.Toolbar, i + 104);
                mSide[i] = new LaunchpadButton(this, ButtonType.Side, Math.Abs(i - 7) * 10 + 19);
            }

            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    mGrid[x, y] = new LaunchpadButton(this, ButtonType.Grid, Math.Abs(y - 7) * 10 + 11 + x);
        }

        private void StartDoubleBuffering()
        {
            mDoubleBuffered = true;
            mDoubleBufferedState = false;
            mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 32 | 16 | 1);
        }

        public void Refresh()
        {
            if (!mDoubleBufferedState)
                mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 32 | 16 | 4);
            else
                mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 32 | 16 | 1);
            mDoubleBufferedState = !mDoubleBufferedState;
        }

        private void EndDoubleBuffering()
        {
            mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 32 | 16);
            mDoubleBuffered = false;
        }

        public void Reset()
        {
            mOutputDevice.SendControlChange(Channel.Channel1, (Control)0, 0);
            Buttons.ToList().ForEach(x => x.SetColor(ButtonColor.Off));
        }

        private void mInputDevice_NoteOn(NoteOnMessage msg)
        {
            LaunchpadButton button = GetButton(msg.Pitch);
            if (button == null)
                return;

            button.State = (ButtonPressState)msg.Velocity;

            if (ButtonPressed != null && button.State == ButtonPressState.Down)
            {
                if (button.type == ButtonType.Side)
                    ButtonPressed.Invoke(this, new ButtonPressEventArgs((SideButton)((((int)msg.Pitch - 9)) / 10) - 1));
                else if (button.type == ButtonType.Grid)
                {
                    int[] pos = GetPosFromGridButton(button);
                    ButtonPressed.Invoke(this, new ButtonPressEventArgs(pos[0], pos[1]));
                }
            }
        }

        private void mInputDevice_ControlChange(ControlChangeMessage msg)
        {
            ToolbarButton toolbarButton = (ToolbarButton)((int)msg.Control - 104);

            LaunchpadButton button = GetButton(toolbarButton);
            if (button == null)
                return;

            button.State = (ButtonPressState)msg.Value;
            if (ButtonPressed != null && button.State == ButtonPressState.Down)
            {
                ButtonPressed.Invoke(this, new ButtonPressEventArgs(toolbarButton));
            }
        }

        public LaunchpadButton GetButton(ToolbarButton toolbarButton)
        {
            return mToolbar[(int)toolbarButton];
        }

        public LaunchpadButton GetButton(SideButton sideButton)
        {
            return mSide[(int)sideButton];
        }

        private LaunchpadButton GetButton(Pitch pitch)
        {
            foreach (LaunchpadButton button in mGrid)
            {
                if (button.index == (int)pitch) return button;
            }

            foreach (LaunchpadButton button in mSide)
            {
                if (button.index == (int)pitch) return button;
            }

            foreach (LaunchpadButton button in mToolbar)
            {
                if (button.index == (int)pitch) return button;
            }

            return null;
        }

        private int[] GetPosFromGridButton(LaunchpadButton button)
        {
            if (button.index < 10) return null;
            int x = int.Parse(button.index.ToString().ToCharArray()[1].ToString()) - 1;
            int y = int.Parse(button.index.ToString().ToCharArray()[0].ToString()) - 1;
            return new int[] { x, y };
        }

        public bool DoubleBuffered
        {
            get { return mDoubleBuffered; }
            set {
                if (mDoubleBuffered)
                    EndDoubleBuffering();
                else
                    StartDoubleBuffering();
            }
        }

        public LaunchpadButton this[int x, int y]
        {
            get { return mGrid[x, y]; }
        }

        public IEnumerable<LaunchpadButton> Buttons
        {
            get {
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++)
                        yield return mGrid[x, y];
            }
        }
        public LaunchpadButton[,] GetButtons()
        {
            return mGrid;
        }

        internal OutputDevice OutputDevice
        {
            get { return mOutputDevice; }
        }
    }
}
