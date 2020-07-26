using PCAFFINITY;
using System;
using System.Threading;
using System.Windows.Forms;

namespace HotkeyCommands_Example
{
    public partial class Form1 : HotkeysExtensionForm
    {
        public HotkeyCommand HotkeyCommand { get; set; }
        private readonly object _Lock = new object();

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            HotkeyCommand = new HotkeyCommand(this) { SetHotkeysGlobally = true };

            HotkeyCommand.KeyActionCall += HotkeyCommand_KeyActionCall;
            HotkeyCommand.KeyRegisteredCall += HotkeyCommand_KeyRegisteredCall;
            HotkeyCommand.KeyUnregisteredCall += HotkeyCommand_KeyUnregisteredCall;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HotkeyCommand.IsRegistered) { HotkeyCommand.StopHotkeys(); }
        }

        private void Button1_Click(object sender, EventArgs e)//Start
        {
            try
            {
                lbl_Info.Text = "Starting Hotkeys.";
                Application.DoEvents(); Thread.Sleep(1000);
                HotkeyCommand.StartHotkeys();
            }
            catch (Exception ex) { lbl_Info.Text = $"Err: {ex.Message}"; }
        }
        private void Button2_Click(object sender, EventArgs e)//Stop
        {
            try
            {
                lbl_Info.Text = "Stopping Hotkeys.";
                Application.DoEvents(); Thread.Sleep(1000);
                HotkeyCommand.StopHotkeys();
            }
            catch (Exception ex) { lbl_Info.Text = $"Err: {ex.Message}"; }
        }
        private void Button3_Click(object sender, EventArgs e)//Add
        {
            try
            {
                HotkeyCommand.HotkeyAddKey(textBox1.Text);
                lbl_Info.Text = $"Hotkey {textBox1.Text} has been added to Dictionary.";
            }
            catch (Exception ex) { lbl_Info.Text = $"Err: {ex.Message}"; }
        }
        private void Button4_Click(object sender, EventArgs e)//Remove
        {
            try
            {
                HotkeyCommand.HotkeyRemoveKey(textBox1.Text);
                lbl_Info.Text = $"Hotkey {textBox1.Text} has been removed from the Dictionary.";
            }
            catch (Exception ex) { lbl_Info.Text = $"Err: {ex.Message}"; }
        }
        private void Button5_Click(object sender, EventArgs e)//Restart
        {
            HotkeyCommand.RestartHotkeys();
        }

        private void HotkeyCommand_KeyUnregisteredCall(string key, short id)
        {
            lock (_Lock)
            {
                if (lbl_Info.Text.Contains(" unregistered")) { Application.DoEvents(); Thread.Sleep(1000); }
                lbl_Info.Text = $"Shortcut {key} was unregistered.";
            }
        }
        private void HotkeyCommand_KeyRegisteredCall(bool result, string key, short id)
        {
            lock (_Lock)
            {
                if (lbl_Info.Text.Contains(" registered")) { Application.DoEvents(); Thread.Sleep(1000); }
                lbl_Info.Text = $"Shortcut {key} was {(!result ? "NOT " : "")}registered.";
            }
        }
        private void HotkeyCommand_KeyActionCall(Form form, short id, string key)
        {
            lock (_Lock)
            {
                if (lbl_Info.Text.Contains(" was pressed")) { Application.DoEvents(); Thread.Sleep(1000); }
                lbl_Info.Text = $"Shortcut {key} was pressed.";
            }
        }
    }
}
