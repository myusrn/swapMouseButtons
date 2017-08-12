using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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

            if (args.Length > 0 && (Regex.IsMatch(args[0], @"[-/][\?h]") || Regex.IsMatch(args[0], @"/[^lr]")))
            {
                // pattern @"[-/][\?h]" can also be written as @"(-|/)(\?|h)"
                ShowUsage();
            }
            else if (args.Length > 0 && String.Compare(args[0], "/l", true) == 0)
            {
                // set to left handed regardless of current persisted and runtime state
                SetMouseButtonsSetting(MouseButtonsSetting.LeftHanded);
            }
            else if (args.Length > 0 && String.Compare(args[0], "/r", true) == 0)
            {
                // set to right handed regardless of current persisted and runtime state
                SetMouseButtonsSetting(MouseButtonsSetting.RightHanded);
            }
            else // lookup current persisted setting, no runtime setting lookup option, and swap it
            {
                var currentSetting = GetMouseButtonsSetting();
                if (currentSetting == MouseButtonsSetting.RightHanded) SetMouseButtonsSetting(MouseButtonsSetting.LeftHanded);
                else /* (currentSetting == MouseButtonSettings.LeftHanded) */ SetMouseButtonsSetting(MouseButtonsSetting.RightHanded);
            }
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
                if (mouseButtonsSetting == MouseButtonsSetting.LeftHanded)
                {
                    SwapMouseButton(true); // change runtime setting
                    try { key.SetValue("SwapMouseButtons", "1", RegistryValueKind.String); } // change persisted setting
                    catch (UnauthorizedAccessException) { Console.WriteLine(
                        "unable to persist change execute from \"run as administrator\" environment"); }
                }
                else /* (mouseButtonsSetting == MouseButtonSettings.RightHanded) */
                {
                    SwapMouseButton(false); // change runtime setting
                    try { key.SetValue("SwapMouseButtons", "0", RegistryValueKind.String); } // change persisted setting
                    catch (UnauthorizedAccessException) { Console.WriteLine(
                        "unable to persist change execute from \"run as administrator\" environment"); }
                }
            }
        }

        static void ShowUsage()
        {
            var asm = Assembly.GetExecutingAssembly();
            var asmVersion = asm.GetName().Version.ToString();
            //var asmName = Regex.Match(Environment.CommandLine, @"[^\\]+(?:\.exe)", RegexOptions.IgnoreCase).Value;
            var asmName = Regex.Match(Environment.CommandLine, @"(?<fname>[^\\]+)(?<ext>\.exe)", RegexOptions.IgnoreCase).
                Groups["fname"].Value;

            const string Status = "in progress"; const string Version = "12aug17";
            //Console.WriteLine("\nstatus = " + Status + ", version = " + Version + "\n");  
            Console.WriteLine("\nversion = " + Version + "\n");
            Console.WriteLine("description");
            Console.WriteLine("  command line utility to switch primary and secondary mouse buttons\n");
            Console.WriteLine("usage");
            Console.WriteLine("  " + asmName + " [/l | /r | /h]\n");
            Console.WriteLine("where");
            Console.WriteLine("     = no arguments swaps whatever currently persisted setting is");
            Console.WriteLine("  /l = switches to left handed regardless of current setting");
            Console.WriteLine("  /r = switches to right handed regardless of current setting");
            Console.WriteLine("  /h = [ | /? | unsupported argument ] shows this usage info\n");
            Console.WriteLine("examples");
            Console.WriteLine("  " + asmName + " ");
            Console.WriteLine("  " + asmName + " /l");
            Console.WriteLine("  " + asmName + " /r");
            Console.WriteLine("");
        }
    }
}
