﻿using System;
using System.Windows.Forms;

namespace HotkeyCommand.HKCFormExtension
{
    public class HotkeysExtensionForm : Form
    {

        public delegate void KeyPressedCallEventHandler(Form f, short k);
        public event KeyPressedCallEventHandler KeyPressedCall;
        protected override void WndProc(ref Message m)
        {
            KeyPreview = true;
            if (m.Msg == 0x0312)//Msg ID for Hotkeys
            {
                OnKeyPressedCall(m.WParam);
            }

            base.WndProc(ref m);
        }
        public virtual void OnKeyPressedCall(IntPtr k)
        {
            KeyPressedCall?.Invoke(this, (short)k.ToInt32());
        }//Send to Parent 'HotkeyCommand'
    }
}
