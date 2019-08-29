# HotkeyCommandF
Resource to add FKey shortcuts to any form.

The 'new HotKeys' class requires a <Form> and a List<short>.

The <Form> will be used as the button press receiver.

HKeys List<short> will need to be set before use.
This takes number values between 1 and 12 then convert them to F* keys.

InitiateHotKeys() will take the HKeys list and call the hooks.
HotkeyCommand.KeyActionCall action will fire when hotkey is pressed.

________________________________________________________________

- Reference HotkeyCommandF.dll in the Forms.cs.
- Set the Extension method next to your Form class.
- Create new instance of HotkeyCommand
- Set Action KeyActionCall (Returns Form and short to represent F key)

________________________________________________________________

Example:

using HotkeyCommandF;
using HotkeyCommandF.HKCFormExtension;
using System;
using System.Windows.Forms;

namespace YourHotkeyProgram
{
	public partial class Form1 : HotkeysExtensionForm
	{
		public Form1()
		{
			InitializeComponent();

			//Configure Hotkeys (F1, F2, and F3)
			HotkeyCommand hotkeyComm = new HotkeyCommand(this, new short[] { 1, 2, 3 });
			hotkeyComm.InitiateHotKeys();

			//Returns Form and short to represent the originating Form and F key representation
			hotkeyComm.KeyActionCall += onKeyAction;
		}
	}
}
