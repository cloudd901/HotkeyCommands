﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HotkeyCommands
{
    /// <exclude />
    public class KeyHandler
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private int key;
        private IntPtr hWnd;
        private int id;
        private HotkeyCommand.KeyModifier fsModifiers;

        /// <exclude />
        public KeyHandler(Keys newKey, IntPtr newHandle, int newId, HotkeyCommand.KeyModifier newModifiers)
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