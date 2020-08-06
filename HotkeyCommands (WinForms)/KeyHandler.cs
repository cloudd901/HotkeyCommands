using System;

namespace PCAFFINITY
{
    /// <exclude />
    public class KeyHandler
    {
        private readonly HotkeyCommand.KeyModifier fsModifiers;

        private readonly IntPtr hWnd;

        private readonly int id;

        private readonly int key;

        /// <exclude />
        public KeyHandler(int newKey, IntPtr newHandle, int newId, HotkeyCommand.KeyModifier newModifiers)
        {
            key = newKey;
            hWnd = newHandle;
            id = newId;
            fsModifiers = newModifiers;
        }

        /// <exclude />
        public bool Register()
        {
            return NativeMethods.RegisterHotKey(hWnd, id, (uint)fsModifiers, key);
        }

        /// <exclude />
        public bool Unregister()
        {
            return NativeMethods.UnregisterHotKey(hWnd, id);
        }
    }
}