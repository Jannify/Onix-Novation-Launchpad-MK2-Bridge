using Midi;

namespace IntelOrca.Launchpad
{
    public enum ButtonType { Grid, Toolbar, Side }
    public enum ButtonPressState { Up = 0, Down = 127 };
    public enum ButtonColor
    {
        Off = 0,
        Grey = 1,
        LightGrey = 2,
        White = 3,
        Red = 5,
        Orange = 9,
        Yellow = 12,
        LightGreen = 17,
        Green = 18,
        DarkGreen = 19,
        Cyan = 33,
        LightBlue = 41,
        Blue = 45,
        Rose = 52,
        Pink = 53,
        Purple = 54
    }

    public class LaunchpadButton
    {
        private LaunchpadDevice mLaunchpadDevice;
        private ButtonPressState mState;

        private ButtonType mType;
        private int mIndex;

        internal LaunchpadButton(LaunchpadDevice launchpadDevice, ButtonType type, int index)
        {
            mLaunchpadDevice = launchpadDevice;
            mType = type;
            mIndex = index;
        }

        public void TurnOnLight()
        {
            SetColor(ButtonColor.White);
        }

        public void TurnOffLight()
        {
            SetColor(ButtonColor.Off);
        }

        public void SetColor(ButtonColor color)
        {
            if (mType == ButtonType.Toolbar)
                mLaunchpadDevice.OutputDevice.SendControlChange(Channel.Channel1, (Control)mIndex, (int)color);
            else
                mLaunchpadDevice.OutputDevice.SendNoteOn(Channel.Channel1, (Pitch)mIndex, (int)color);
        }

        public ButtonPressState State
        {
            get { return mState; }
            internal set { mState = value; }
        }

        public int index
        {
            get { return mIndex; }
        }

        public ButtonType type
        {
            get { return mType; }
        }
    }
}
