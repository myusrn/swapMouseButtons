using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SwapMouseButton
{
    class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SwapMouseButton([param: MarshalAs(UnmanagedType.Bool)] bool fSwap);

        enum MouseButtonsSetting
        {
            RightHanded = 0,
            LeftHanded = 1
        }        

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            // implementation if you just want to set mouse button settings to left handed if 
            // no arguments or argument "/r" found and right handed if "/l" argument found
            //if (args.Length > 0 && String.Compare(args[0], "/r", true) == 0) SetMouseButtonsSetting(MouseButtonsSetting.RightHanded);
            //if (args.Length > 0 && String.Compare(args[0], "/l", true) == 0) SetMouseButtonsSetting(MouseButtonsSetting.LeftHanded);

            // implementation if you want to always swap mouse button settings from current state
            var currentSetting = GetMouseButtonsSetting();

            if (currentSetting == MouseButtonsSetting.RightHanded) SetMouseButtonsSetting(MouseButtonsSetting.LeftHanded);
            else /* (currentSetting == MouseButtonSettings.LeftHanded) */ SetMouseButtonsSetting(MouseButtonsSetting.RightHanded);
        }

        /// <summary>
        /// gets the mouse buttons setting currently in effect
        /// </summary>
        /// <returns>value indicating whether right or left handed setting is in place</returns>
        static MouseButtonsSetting GetMouseButtonsSetting()
        {
            var mouseButtonsSetting = MouseButtonsSetting.RightHanded;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Mouse"))
            {
                if (key is null) throw new ApplicationException("unable to open mouse settings registry key");
                var kv = key.GetValue("SwapMouseButtons");
                if (kv is null) throw new ApplicationException("unable to open mouse settings registry key value");
                if (Convert.ToInt16(kv) == 1) mouseButtonsSetting = MouseButtonsSetting.LeftHanded;
            }

            return mouseButtonsSetting;
        }

        /// <summary>
        /// sets the mouse buttons setting currently in effect
        /// </summary>
        /// <returns>value indicating whether right or left handed setting is in place</returns>
        static void SetMouseButtonsSetting(MouseButtonsSetting mouseButtonsSetting)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Mouse"))
            {
                if (key is null) throw new ApplicationException("unable to open mouse settings registry key");
                if (mouseButtonsSetting == MouseButtonsSetting.RightHanded)
                {
                    SwapMouseButton(false); // change runtime setting
                    try { key.SetValue("SwapMouseButtons", "0", RegistryValueKind.String); } // change persisted setting
                    catch (UnauthorizedAccessException) { Console.WriteLine(
                        "unable to persist change execute from \"run as administrator\" environment"); }
                }
                else /* (mouseButtonsSetting == MouseButtonSettings.LeftHanded) */
                {
                    SwapMouseButton(true); // change runtime setting
                    try { key.SetValue("SwapMouseButtons", "1", RegistryValueKind.String); } // change persisted setting
                    catch (UnauthorizedAccessException) { Console.WriteLine(
                        "unable to persist change execute from \"run as administrator\" environment"); }
                }
            }
        }
    }
}
