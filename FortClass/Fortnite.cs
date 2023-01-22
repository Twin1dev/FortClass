using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        private static void Launch(string username, string password, string path, string DownloadTo, string Consoledll, string SCP = "", string lawindll = "", bool isHeadless = false, bool LawinServerV2 = false, string redirectdll = "", string AddedArgs = "", string gameserverDLL = "")
        {
            try
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
                    return;
                }
                else if (!Directory.Exists(path + "\\FortniteGame"))
                {
                    MessageBox.Show("The selected path does not have Fortnite! Make sure it has Engine and FortniteGame in it.");
                }
                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Password is empty!");
                    return;
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
                /*            if (Consoledll == "none")
                            {
                                proc.StartInfo.RedirectStandardOutput = false;
                            }
                            else
                            {
                                proc.StartInfo.RedirectStandardOutput = true;
                            }*/
                proc.StartInfo.FileName = path + "\\FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe";
                proc.StartInfo.Arguments = "-epicapp=Fortnite -epicenv=Prod -epicportal -AUTH_TYPE=epic -AUTH_LOGIN=" + username + " -AUTH_PASSWORD=" + password + " -epiclocale=en-us -fltoken=7a848a93a74ba68876c36C1c -fromfl=none -noeac -nobe -skippatchcheck " + AddedArgs;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
           
                proc.WaitForInputIdle();











                for (; ; )
                {
                    string output = proc.StandardOutput.ReadLine();
                    if (lawindll != "")
                        Inject(proc.Id, lawindll);
                    if (LawinServerV2)
                    {
                        /*       Inject(proc.Id, Directory.GetCurrentDirectory() + "\\MemoryLeak.dll");*/
                        Inject(proc.Id, redirectdll);

                    }
                    if (output.Contains("Game Engine Initialized"))
                    {
            
                            if (!isHeadless)
                            {
                                Thread.Sleep(5000);

                                if (SCP != "")
                                {
                                    Inject(proc.Id, SCP);


                                }

                                Inject(proc.Id, Consoledll);


                            }
                            else
                            {
                                Thread.Sleep(5000);


                                Inject(proc.Id, gameserverDLL);

                            }
                        } 
                    
                    

                    }








                }


            catch (Exception ex)
            {
               
            }

        }
        public static void LaunchGame(string username, string password, string path, string DownloadTo, string Consoledll, string SCP = "", string lawindll = "", bool isHeadless = false, bool LawinServerV2 = false, string redirectdll = "", string AddedArgs = "", string gameserverDLL = "")
        {
            new Thread(new ThreadStart(() => Launch(username, password, path, DownloadTo, Consoledll, SCP, lawindll, isHeadless, LawinServerV2, redirectdll, AddedArgs, gameserverDLL))).Start();
            
        }
        public static void Inject(int pid, string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("DLL not found! Make sure your antivirus did not Remove it!");
                return;
            }
/*            if (path == "none")
            {
                Environment.Exit(0);
                return;
            }*/
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
    

