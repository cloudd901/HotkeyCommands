# HotkeyCommands
Resource to add Hotkey shortcuts to any form.

The 'new HotKeys' class requires a Form and a List-short.

The Form will be used as the button press receiver.

HKeys List-short will need to be set before use.
This takes a string representation of your key.

InitiateHotKeys() will take the HKeys list and call the hooks.
HotkeyCommands.KeyActionCall action will fire when hotkey is pressed.

________________________________________________________________

- Reference HotkeyCommands.dll in the Forms.cs.
- Set the Extension method next to your Form class.
- Create new instance of HotkeyCommands
- Set Action KeyActionCall (Uses Form and string)-(Current Form and Key Pressed)

________________________________________________________________

Example:

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

		    //Returns Form and string to represent the originating Form and Key representation
		    hotkeyComm.KeyActionCall += onKeyAction;
		}
	    }
	}
