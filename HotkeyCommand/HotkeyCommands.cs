using HotkeyCommands.HKCFormExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace HotkeyCommands
{
    /* 
     * new HotKeys class requires a <Form> and a List<short>.
     * 
     * The <Form> will be used as the button press receiver.
     * 
     * HKeys List<string> will need to be set before use.
     * This takes a string representation of your key.
     * 
     * InitiateHotKeys() will take the HKeys list and call the hooks.
     * HotkeyCommands.KeyActionCall action will fire when hotkey is pressed.
     * 
     * ---
     * 
     * Reference HotkeyCommands.dll in the Forms.cs.
     * - Set the Extension method next to your Form class.
     * - Create new instance of HotkeyCommands
     * - Set Action KeyActionCall (Uses Form and string)-(Current Form and Key Pressed)
     * 
     * Example:
     * 

        using HotkeyCommands;
        using HotkeyCommands.HKCFormExtension;
        using System;
        using System.Windows.Forms;

        namespace YourHotkeyProgram
        {
            public partial class Form1 : HotkeysExtensionForm
            {
                public Form1()
                {
                    InitializeComponent();

                    //Configure Hotkeys
                    HotkeyCommand hotkeyComm = new HotkeyCommand(this, new string[] { "F1", "F2", "Escape", "F3" });
                    hotkeyComm.InitiateHotKeys();

                    //Returns Form and short to represent the originating Form and F key representation
                    hotkeyComm.KeyActionCall += onKeyAction;
                }
            }
        }

     *
     */

    public class HotkeyCommand
    {
        public delegate void KeyActionCallEventHandler(Form f, string k);
        public event KeyActionCallEventHandler KeyActionCall;

        internal void OnKeyActionCall(Form f, short k)
        {
            string s; _keyvaluepairs.TryGetValue(k, out s);
            KeyActionCall?.Invoke(f, s);
        }//Received from 'HotkeysExtension'. Alert new event to HotkeyCommand instance.

        private readonly Dictionary<short, string> _keyvaluepairs = new Dictionary<short, string>();
        private readonly Form _form;
        private bool registered = false;


        public HotkeyCommand(Form f, string[] newKeyList)
        { _form = f; HKeys = newKeyList.ToList(); }

        public List<string> HKeys { get; set; } = new List<string>();
        public ComboBox HKComboBoxList { get; set; }
        public Label HKCountingLabel { get; set; } = null;
        public Label HKTextLabel { get; set; } = null;
        public bool SortList { get; set; } = false;
        public bool UniqueList { get; set; } = false;

        public void InitiateHotKeys()
        {
            if (registered)
            { throw new InvalidOperationException("HotkeyCommanderF is already Initiated."); }
            else
            { registered = true; }

            try
            { (_form as HotkeysExtensionForm).KeyPressedCall += OnKeyActionCall; }
            catch
            { throw new InvalidCastException("Unable to subscribe to KeyCalled event. Please ensure your Form is using the HotkeysExtension with KeyPressedCall event."); }


            if (HKeys.Count <= 0)
            { throw new InvalidFilterCriteriaException("Please set HKeys - Numbers 1 through 12 to corrispond to F* keys."); }
            try
            {
                int i = 0;
                foreach (string s in HKeys)
                {
                    i++;
                    _keyvaluepairs.Add((short)i, s);
                    new KeyHandler((Keys)Enum.Parse(typeof(Keys), s), _form.Handle, i).Register();
                }
            }
            catch
            {
                throw new Exception("An unknown error occurred while registering shortcutkeys.");
            }
        }


    }
}
