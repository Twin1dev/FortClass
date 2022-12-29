using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FortClass
{
    public class Fortnite
    {
        /// <summary>
        /// Launches Fortnite.
        /// </summary>
        /// <param name="username">Username of the player</param>
        /// <param name="path">Path to the fortnite build</param>
        /// <param name="DownloadTo">Download the special FN client for ssl</param>
        /// <param name="lawindll">Name of Cumar dll to inject.</param>
        /// <param name="Consoledll">Name of Console dll to inject.</param>
        public static void Launch(string username, string password, string path, string DownloadTo, string Consoledll, string lawindll = "", bool isHeadless = false, bool LawinServerV2 = false, string redirectdll = "", string AddedArgs = "", string gameserverDLL = "")
        {
            #region Checks

            
            if (username.Contains(" "))
            {
                MessageBox.Show("You have spaces in your username! Please change it to not have any, or just use underscores");
                return;
            }
            else if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Your name is empty! Please input your username.");
            }
            else if (!Directory.Exists(path + "\\FortniteGame"))
            {
                MessageBox.Show("The selected path does not have Fortnite! Make sure it has Engine and FortniteGame in it.");
            }


            #endregion


            if (!File.Exists(DownloadTo + "\\FortniteClient-Win64-Shipping_BE.exe"))
            {
                DownloadFile("https://cdn.discordapp.com/attachments/958139296936783892/1000707724507623424/FortniteClient-Win64-Shipping_BE.exe", DownloadTo + "\\FortniteClient-Win64-Shipping_BE.exe");
            }
            if (!File.Exists(DownloadTo + "\\FortniteLauncher.exe"))
            {
                DownloadFile("https://cdn.discordapp.com/attachments/958139296936783892/1000707724818006046/FortniteLauncher.exe", DownloadTo + "\\FortniteLauncher.exe");
            }
            Process.Start(new ProcessStartInfo
            {
                FileName = DownloadTo + "\\FortniteLauncher.exe",
                CreateNoWindow = true,
                UseShellExecute = false
            });
            Process.Start(new ProcessStartInfo
            {
                FileName = DownloadTo + "\\FortniteClient-Win64-Shipping_BE.exe",
                CreateNoWindow = true,
                UseShellExecute = false
            });
            if (isHeadless)
                AddedArgs = "-log -nosplash -nosound -nullrhi";
            Process proc = new Process();
            proc.StartInfo.FileName = path + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe";
            proc.StartInfo.Arguments = "-epicapp=Fortnite -epicenv=Prod -epicportal -AUTH_TYPE=epic -AUTH_LOGIN=" + username + " -AUTH_PASSWORD=" + password + " -epiclocale=en-us -fltoken=7a848a93a74ba68876c36C1c -fromfl=none -noeac -nobe -skippatchcheck " + AddedArgs;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            if (lawindll != "")
                Inject(proc.Id, lawindll);
            if (LawinServerV2)
                Inject(proc.Id, redirectdll);
            for (; ; )
            {
                string output = proc.StandardOutput.ReadLine();
                if (output.Contains("Game Engine Initialized"))
                {
                    if (!isHeadless)
                    {
                        Thread.Sleep(5000);
                        Inject(proc.Id, Consoledll);
                        Environment.Exit(0);
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        Inject(proc.Id, gameserverDLL);
                        Environment.Exit(0);
                    }

                }

            }
        }


        public static void Inject(int pid, string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("DLL not found! Make sure your antivirus did not Remove it!");
                return;
            }
            IntPtr hProcess = Win32.OpenProcess(1082, false, pid);
            IntPtr procAdress = Win32.GetProcAddress(Win32.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            uint num = checked((uint)((path.Length + 1) * Marshal.SizeOf(typeof(char))));
            IntPtr intPtr = Win32.VirtualAllocEx(hProcess, IntPtr.Zero, num, 12288U, 4U);
            UIntPtr uintPtr;
            Win32.WriteProcessMemory(hProcess, intPtr, Encoding.Default.GetBytes(path), num, out uintPtr);
            Win32.CreateRemoteThread(hProcess, IntPtr.Zero, 0U, procAdress, intPtr, 0U, IntPtr.Zero);
        }
        internal static void DownloadFile(string URL, string path)
        {
            new WebClient().DownloadFile(URL, path);
        }
       
    }
}
