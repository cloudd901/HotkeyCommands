namespace PCAFFINITY
{
    using System;
    using System.Windows.Forms;

    /// <exclude />
    public class HotkeysExtensionForm : Form
    {
        private const int WM_HOTKEY = 0x0312;

        /// <exclude />
        public delegate void KeyPressedCallEventHandler(Form f, short k);

        /// <exclude />
        public event KeyPressedCallEventHandler KeyPressedCall;

        /// <exclude />
        public virtual void OnKeyPressedCall(IntPtr k)
        {
            KeyPressedCall?.Invoke(this, (short)k.ToInt32());
        }

        /// <exclude />
        protected override void WndProc(ref Message m)
        {
            //KeyPreview = true;
            if (m.Msg == WM_HOTKEY)//Msg ID for Hotkeys
            {
                OnKeyPressedCall(m.WParam);
            }
            base.WndProc(ref m);
        }
    }
}