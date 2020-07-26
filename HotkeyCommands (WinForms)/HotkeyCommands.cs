using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PCAFFINITY
{
    /* 
     * - Reference HotkeyCommands (WinForms).dll in your Forms.cs.
     * - Set the Extension method next to your Form class.
     * - Create new instance of HotkeyCommands.
     * - Set Action KeyActionCall (Returns Current Form and Key Pressed)
     */

    /// <summary>Initialize the HotkeyCommand class.</summary>
    /// <seealso cref="System.IDisposable" />
    /// <example>This is how to quickly Initiate Hotkeys:
    ///     <code>
    ///         <para>using PCAFFINITY;</para>
    ///         <para>using System;</para>
    ///         <para>using System.Windows.Forms;</para>
    ///         <para>namespace YourHotkeyProgram</para>
    ///         <para>{</para>
    ///         <para>    public partial class Form1 : HotkeysExtensionForm</para>
    ///         <para>    {</para>
    ///         <para>        public Form1()</para>
    ///         <para>        {</para>
    ///         <para>            InitializeComponent();</para>
    ///         <para>            HotkeyCommand hotkeyComm = new HotkeyCommand(this, new string[] { "F1", "Escape", "{CTRL}{Shift}A" });</para>
    ///         <para>            hotkeyComm.KeyActionCall += onKeyAction;</para>
    ///         <para>            hotkeyComm.StartHotkeys();</para>
    ///         <para>        }</para>
    ///         <para>    }</para>
    ///         <para>}</para>
    ///     </code>
    /// </example>
    public class HotkeyCommand : IDisposable
    {
        /// <summary>Key Register Modifiers.</summary>
        [Flags]
        public enum KeyModifier
        {
            /// <exclude />
            None = 0x0000,
            /// <exclude />
            Alt = 0x0001,
            /// <exclude />
            Ctrl = 0x0002,
            /// <exclude />
            NoRepeat = 0x4000,
            /// <exclude />
            Shift = 0x0004,
            /// <exclude />
            Win = 0x0008
        }
        const uint WM_KEYDOWN = 0x100;
        const uint WM_KEYUP = 0x101;

        private readonly Form _form;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>EventHandler for Hotkey Registration events.</summary>
        /// <param name="result"><c>true</c> if the registration was successfull.</param>
        /// <param name="key">The Hotkey being registered.</param>
        /// <param name="id">They ID of the Hotkey being registered</param>
        public delegate void KeyRegisteredEventHandler(bool result, string key, short id);
        /// <summary>Occurs when Hotkey is Registered.</summary>
        public event KeyRegisteredEventHandler KeyRegisteredCall;

        /// <summary>EventHandler for Hotkey Unregistration events.</summary>
        /// <param name="key">The Hotkey being unregistered.</param>
        /// <param name="id">They ID of the Hotkey being unregistered</param>
        public delegate void KeyUnregisteredEventHandler(string key, short id);
        /// <summary>Occurs when Hotkey is Unregistered.</summary>
        public event KeyUnregisteredEventHandler KeyUnregisteredCall;

        /// <summary>EventHandler for Hotkey Pressed events.</summary>
        /// <param name="form">The form.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="key">The key.</param>
        public delegate void KeyActionCallEventHandler(Form form, short id, string key);
        /// <summary>Occurs when Hotkey is called.</summary>
        public event KeyActionCallEventHandler KeyActionCall;
        /// <summary>Gets the Dictionary of all current Hotkeys.</summary>
        /// <value>The Dictionary of Hotkeys.</value>
        public Dictionary<short, string> HotkeyDictionary { get; private set; } = new Dictionary<short, string>();
        /// <summary>Check if Hotkeys are active.</summary>
        /// <value>Get the Hotkey status.</value>
        public bool IsRegistered { get; private set; } = false;
        /// <summary>Gets or sets a value indicating whether Hotkeys trigger regardless of active window.</summary>
        /// <value>
        ///   <c>true</c> if [set Hotkeys globally]; otherwise, <c>false</c>.</value>
        public bool SetHotkeysGlobally { get; set; } = true;
        /// <summary>Gets or sets a value indicating whether Exceptions are thown or ignored.</summary>
        /// <value>
        ///   <c>true</c> if [Ignore Exceptions]; otherwise, <c>false</c>.</value>
        public bool SetSuppressExceptions { get; set; } = false;

        /// <summary>Initializes a new instance of the <see cref="HotkeyCommand"/> class.</summary>
        /// <param name="f">The Form listening for Hotkeys.</param>
        /// <param name="newKeyList">New String list of Hotkeys. Keymodifiers in brackets {}."</param>
        /// <example><code>HotkeyCommand hotkeyComm = new HotkeyCommand(this, new string[] { "F1", "Escape", "{CTRL}{Shift}A" });</code></example>
        /// <exception cref="InvalidCastException">Unable to subscribe to KeyCalled event. Please ensure your Form is using the HotkeysExtension with KeyPressedCall event.</exception>
        public HotkeyCommand(Form f, string[] newKeyList = null)
        {
            _form = f;
            try
            { (_form as HotkeysExtensionForm).KeyPressedCall += OnKeyActionCall; }
            catch
            {
                if (!SetSuppressExceptions)
                { throw new InvalidCastException("Unable to subscribe to KeyCalled event. Please ensure your Form is using the HotkeysExtension with KeyPressedCall event."); }
                else { return; }
            }
            if (newKeyList != null)
            {
                newKeyList = (string[])newKeyList.Distinct().ToArray();
                for (int i = 0; i < newKeyList.Count(); i++)
                { HotkeyDictionary.Add((short)(i + 1), newKeyList[i]); }
            }
        }

        private void OnKeyActionCall(Form form, short id)
        {
            if (!SetHotkeysGlobally)
            {
                IntPtr window = GetForegroundWindow();
                if (form.Handle != window)
                {
                    try
                    {
                        HotkeyDictionary.TryGetValue(id, out string keyString);
                        Enum.TryParse(keyString, out Keys keyKeys);
                        PostMessage(window, WM_KEYDOWN, (IntPtr)keyKeys, IntPtr.Zero);
                        PostMessage(window, WM_KEYUP, (IntPtr)keyKeys, IntPtr.Zero);
                    }
                    catch { }
                    return;
                }
            }
            HotkeyDictionary.TryGetValue(id, out string key);
            KeyActionCall?.Invoke(form, id, key);
        }//Received from 'HotkeysExtension'. Alert new event to HotkeyCommand instance.

        /// <summary>Register a list of Hotkeys.</summary>
        /// <param name="newKeyList">The new Hotkey list.</param>
        /// <param name="replaceCurrentKeys">if set to <c>true</c> [replace current List of Hotkeys]. see <see cref="HotkeyDictionary"/></param>
        public void HotkeyAddKeyList(string[] newKeyList, bool replaceCurrentKeys = false)
        {
            if (replaceCurrentKeys) { HotkeyUnregisterAll(true); HotkeyDictionary.Clear(); }
            foreach (string s in newKeyList)
            {
                if (HotkeyDictionary.ContainsValue(s))
                {
                    short id = HotkeyDictionary.FirstOrDefault(x => x.Value == s).Key;
                    HotkeyDictionary.Remove(id);
                }
            }
            foreach (string s in newKeyList)
            {
                HotkeyAddKey(s);
            }
        }
        /// <summary>Register a list of Hotkeys.</summary>
        /// <param name="newKeyList">The new Hotkey list.</param>
        /// <param name="newIDList">The new Hotkey ID list.</param>
        /// <param name="replaceCurrentKeys">if set to <c>true</c> [replace current List of Hotkeys]. see <see cref="HotkeyDictionary"/></param>
        /// <exception cref="InvalidOperationException">
        /// Size of newKeyList is not the same as newIDList.
        /// or
        /// newKeyList cannot contain duplicate keys.
        /// or
        /// '{s}' Key is already in HotkeyDictionary.
        /// or
        /// '{s}' ID is already in HotkeyDictionary.
        /// </exception>
        public void HotkeyAddKeyList(string[] newKeyList, short[] newIDList, bool replaceCurrentKeys = false)
        {
            if (newIDList == null) { HotkeyAddKeyList(newKeyList, replaceCurrentKeys); return; }
            if (newIDList.Count() != newKeyList.Count())
            {
                if (!SetSuppressExceptions)
                { throw new InvalidOperationException("Size of newKeyList is not the same as newIDList."); }
                else
                { return; }
            }
            newKeyList = newKeyList.Distinct().ToArray();
            if (newIDList.Count() != newKeyList.Count())
            {
                if (!SetSuppressExceptions)
                { throw new InvalidOperationException("newKeyList cannot contain duplicate keys."); }
                else
                { return; }
            }

            if (replaceCurrentKeys) { HotkeyUnregisterAll(true); HotkeyDictionary.Clear(); }

            for (int i = 0; i < newKeyList.Count(); i++)
            {
                HotkeyAddKey(newKeyList[i], newIDList[i]);
            }
        }
        /// <summary>Register a single Hotkey to <see cref="HotkeyDictionary"/>.</summary>
        /// <param name="key">The Hotkey.</param>
        /// <param name="id">The Hotkey ID.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">'{key}' Key is already in HotkeyDictionary.
        /// or
        /// ID '{id}' is already registered to key '{key}' in HotkeyDictionary.</exception>
        public HotkeyCommand HotkeyAddKey(string key, short? id = null)
        {
            key = key.Trim().ToUpper();
            if (HotkeyDictionary.ContainsValue(key))
            {
                if (!SetSuppressExceptions) { throw new InvalidOperationException($"'{key}' Key is already in HotkeyDictionary."); }
                else { return this; }
            }
            
            short idK;
            try
            {
                if (id == null) { idK = (short)((int)HotkeyDictionary.Keys.Count + 1); }
                else { idK = (short)id; }
            }
            catch
            { idK = 1; }

            short? knownId = null;
            HotkeyDictionary.TryGetValue((short)idK, out string knownKey);

            if (knownKey != null)
            {
                if (knownKey == key)
                {
                    if (!SetSuppressExceptions) { throw new InvalidOperationException($"{key} is already in HotkeyDictionary."); }
                    else { return this; }
                }
                else if (knownId == idK)
                {
                    if (!SetSuppressExceptions) { throw new InvalidOperationException($"ID '{idK}' is already registered to key '{knownKey}' in HotkeyDictionary."); }
                    else { return this; }
                }
            }
            HotkeyDictionary.Add(idK, key);
            return this;
        }
        /// <summary>Unregister a single Hotkey from <see cref="HotkeyDictionary"/>.</summary>
        /// <param name="key">The Hotkey.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Unable to Unregister '{key}' key.</exception>
        public HotkeyCommand HotkeyRemoveKey(string key)
        {
            key = key.Trim().ToUpper();
            short? knownId = null;

            if (!HotkeyDictionary.ContainsValue(key))
            { if (!SetSuppressExceptions) { throw new InvalidOperationException($"Unable to Unregister '{key}' key. Key Not Found"); } else { return this; } }

            knownId = HotkeyDictionary.FirstOrDefault(x => x.Value == key).Key;
            HotkeyDictionary.Remove((short)knownId);
            return this;
        }
        /// <summary>Unregisters all Hotkeys from <see cref="HotkeyDictionary"/>.</summary>
        /// <param name="removeCurrentKeys">if set to <c>true</c> [Delete all from HotkeyDictionary], else <c>false</c> to [keep HotkeyDictionary].</param>
        public void HotkeyUnregisterAll(bool removeCurrentKeys = true)
        {
            Dictionary<short, string> copyDictionary = new Dictionary<short, string>(HotkeyDictionary);
            foreach (var p in copyDictionary)
            {
                string keyString = p.Value;
                KeyModifier km = new KeyModifier();
                while (keyString.Contains("{"))
                {
                    int loc1 = keyString.IndexOf('{');
                    int loc2 = keyString.IndexOf('}');
                    string mod = keyString.Substring(loc1 + 1, loc2 - loc1 - 1);
                    keyString = keyString.Replace("{" + mod + "}", "");
                    mod = mod.ToUpper();
                    if (mod == "SHFT" || mod == "SHIFT") { km |= KeyModifier.Shift; }
                    if (mod == "CTRL" || mod == "CONTROL") { km |= KeyModifier.Ctrl; }
                    if (mod == "ALT") { km |= KeyModifier.Alt; }
                    if (mod == "WIN") { km |= KeyModifier.Win; }
                }
                try
                {
                    if (removeCurrentKeys) { HotkeyDictionary.Remove(p.Key); }

                    int keys = (int)Keys.None;
                    if (keyString != "")
                    { keys = FilterKeytoInt(keyString); }

                    new KeyHandler(keys, _form.Handle, p.Key, km).Unregister();
                    KeyUnregisteredCall?.Invoke(p.Value, p.Key);
                }
                catch { }
            }
        }

        private int FilterKeytoInt(string keyString)
        {
            keyString = keyString.ToUpper();
            int keys;

            if (keyString.Contains("PRINTSCREEN"))
            { keys = (int)Keys.PrintScreen; keyString.Replace("PRINTSCREEN", ""); }
            else if (keyString.Contains("PRINTSCRN"))
            { keys = (int)Keys.PrintScreen; keyString.Replace("PRINTSCRN", ""); }

            else if (keyString.Contains("PRINT"))
            { keys = (int)Keys.Print; keyString.Replace("PRINT", ""); }

            else if (keyString.Contains("PLAY"))
            { keys = (int)Keys.Play; keyString.Replace("PLAY", ""); }
            else if (keyString.Contains("PAUSE"))
            { keys = (int)Keys.Pause; keyString.Replace("PAUSE", ""); }

            else if (keyString.Contains("LWIN"))
            { keys = (int)Keys.LWin; keyString.Replace("LWIN", ""); }
            else if (keyString.Contains("RWIN"))
            { keys = (int)Keys.RWin; keyString.Replace("RWIN", ""); }
            else if (keyString.Contains("WIN"))
            { keys = (int)Keys.LWin; keyString.Replace("WIN", ""); }

            else if (keyString.Contains("UP"))
            { keys = (int)Keys.Up; keyString.Replace("UP", ""); }
            else if (keyString.Contains("DOWN"))
            { keys = (int)Keys.Down; keyString.Replace("DOWN", ""); }
            else if (keyString.Contains("LEFT"))
            { keys = (int)Keys.Left; keyString.Replace("LEFT", ""); }
            else if (keyString.Contains("RIGHT"))
            { keys = (int)Keys.Right; keyString.Replace("RIGHT", ""); }

            else if (keyString.Contains("SPACE"))
            { keys = (int)Keys.Space; keyString.Replace("SPACE", ""); }
            else if (keyString.Contains("SPC"))
            { keys = (int)Keys.Space; keyString.Replace("SPC", ""); }
            else if (keyString.Contains("ESCAPE"))
            { keys = (int)Keys.Escape; keyString.Replace("ESCAPE", ""); }
            else if (keyString.Contains("ESC"))
            { keys = (int)Keys.Escape; keyString.Replace("ESC", ""); }
            else if (keyString.Contains("CLEAR"))
            { keys = (int)Keys.OemClear; keyString.Replace("CLEAR", ""); }
            else if (keyString.Contains("CLR"))
            { keys = (int)Keys.OemClear; keyString.Replace("CLR", ""); }
            else if (keyString.Contains("CAPSLOCK"))
            { keys = (int)Keys.CapsLock; keyString.Replace("CAPSLOCK", ""); }
            else if (keyString.Contains("END"))
            { keys = (int)Keys.End; keyString.Replace("END", ""); }
            else if (keyString.Contains("HOME"))
            { keys = (int)Keys.Home; keyString.Replace("HOME", ""); }
            else if (keyString.Contains("INSERT"))
            { keys = (int)Keys.Insert; keyString.Replace("INSERT", ""); }
            else if (keyString.Contains("PAGEUP"))
            { keys = (int)Keys.PageUp; keyString.Replace("PAGEUP", ""); }
            else if (keyString.Contains("PGUP"))
            { keys = (int)Keys.PageUp; keyString.Replace("PGUP", ""); }
            else if (keyString.Contains("PAGEDOWN"))
            { keys = (int)Keys.PageDown; keyString.Replace("PAGEDOWN", ""); }
            else if (keyString.Contains("PGDOWN"))
            { keys = (int)Keys.PageDown; keyString.Replace("PGDOWN", ""); }

            else if (keyString == "]")
            { keys = (int)Keys.OemCloseBrackets; }
            else if (keyString == "[")
            { keys = (int)Keys.OemOpenBrackets; }
            else if (keyString == ",")
            { keys = (int)Keys.Oemcomma; }
            else if (keyString == ".")
            { keys = (int)Keys.OemPeriod; }
            else if (keyString == "?")
            { keys = (int)Keys.OemQuestion; }
            else if (keyString == "\"")
            { keys = (int)Keys.OemQuotes; }
            else if (keyString == "|")
            { keys = (int)Keys.OemPipe; }
            else if (keyString == "+")
            { keys = (int)Keys.Oemplus; }
            else if (keyString == "-")
            { keys = (int)Keys.OemMinus; }
            else if (keyString == "*")
            { keys = (int)Keys.Multiply; }
            else if (keyString == "`")
            { keys = (int)Keys.Oemtilde; }
            else if (keyString == "1")
            { keys = (int)Keys.D1; }
            else if (keyString == "2")
            { keys = (int)Keys.D2; }
            else if (keyString == "3")
            { keys = (int)Keys.D3; }
            else if (keyString == "4")
            { keys = (int)Keys.D4; }
            else if (keyString == "5")
            { keys = (int)Keys.D5; }
            else if (keyString == "6")
            { keys = (int)Keys.D6; }
            else if (keyString == "7")
            { keys = (int)Keys.D7; }
            else if (keyString == "8")
            { keys = (int)Keys.D8; }
            else if (keyString == "9")
            { keys = (int)Keys.D9; }
            else if (keyString == "0")
            { keys = (int)Keys.D0; }
            else
            { keys = (int)Enum.Parse(typeof(Keys), keyString, true); }

            return keys;
        }

        /// <summary>Start using hotkeys.</summary>
        /// <exception cref="InvalidOperationException">HotkeyCommands is already Initiated. Try stopping first.</exception>
        public void StartHotkeys()
        {
            if (IsRegistered)
            {
                if (!SetSuppressExceptions)
                { throw new InvalidOperationException("HotkeyCommands is already Initiated. Try stopping first."); }
                else { return; }
            }
            else
            { IsRegistered = true; }
            RegAllDictionary();
        }
        private void RegAllDictionary()
        {
            foreach (KeyValuePair<short, string> p in HotkeyDictionary)
            {
                string keyString = p.Value;
                KeyModifier km = new KeyModifier();
                while (keyString.Contains("{"))
                {
                    int loc1 = keyString.IndexOf('{');
                    int loc2 = keyString.IndexOf('}');
                    string mod = keyString.Substring(loc1 + 1, loc2 - loc1 - 1);
                    keyString = keyString.Replace("{" + mod + "}", "");
                    mod = mod.ToUpper();
                    if (mod == "SHFT" || mod == "SHIFT") { km |= KeyModifier.Shift; }
                    if (mod == "CTRL" || mod == "CONTROL") { km |= KeyModifier.Ctrl; }
                    if (mod == "ALT") { km |= KeyModifier.Alt; }
                    if (mod == "WIN") { km |= KeyModifier.Win; }
                }
                try
                {
                    int keys = (int)Keys.None;

                    if (keyString != "")
                    { keys = FilterKeytoInt(keyString); }

                    bool test = new KeyHandler(keys, _form.Handle, p.Key, km).Register();

                    KeyRegisteredCall?.Invoke(test, p.Value, p.Key);
                }
                catch (Exception e)
                {
                    HotkeyUnregisterAll(false);
                    if (!SetSuppressExceptions) { throw new InvalidOperationException(e.Message); } else { }
                }
            }
        }
        /// <summary>Stop using hotkeys.</summary>
        /// <exception cref="InvalidOperationException">HotkeyCommands is not started. Try starting it first.</exception>
        public void StopHotkeys()
        {
            if (!IsRegistered)
            {
                if (!SetSuppressExceptions)
                { throw new InvalidOperationException("HotkeyCommands is not started. Try starting it first."); }
                else { return; }
            }
            else
            { IsRegistered = false; }
            HotkeyUnregisterAll(false);
        }
        /// <summary>Quickly Stop and Restart the hotkeys. Only use if already Started.</summary>
        public void RestartHotkeys()
        {
            if (IsRegistered) { StopHotkeys(); }
            StartHotkeys();
        }
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            if (IsRegistered) { StopHotkeys(); }
            try
            { (_form as HotkeysExtensionForm).KeyPressedCall -= OnKeyActionCall; }
            catch
            { }
            HotkeyDictionary.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
