using System;
using System.Runtime.InteropServices;

namespace PCAFFINITY
{
    /// <exclude />
    public class KeyHandler
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly int key;
        private readonly IntPtr hWnd;
        private readonly int id;
        private readonly HotkeyCommand.KeyModifier fsModifiers;

        /// <exclude />
        public KeyHandler(int newKey, IntPtr newHandle, int newId, HotkeyCommand.KeyModifier newModifiers)
        {
            key = (int)newKey;
            hWnd = newHandle;
            id = newId;
            fsModifiers = newModifiers;
        }

        /// <exclude />
        public bool Register()
        {
            return RegisterHotKey(hWnd, id, (uint)fsModifiers, key);
        }

        /// <exclude />
        public bool Unregister()
        {
            return UnregisterHotKey(hWnd, id);
        }
    }
}
