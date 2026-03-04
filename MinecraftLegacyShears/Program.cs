using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftLegacyShears
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Legacy Shears";
            Console.WriteLine("Make sure to have the LegacyShears.exe inside of the game folder, and make sure your game is closed.");

            Console.WriteLine("Press enter to start");
            Console.ReadLine();

            main.Start();

            Console.WriteLine("\nFinished. Press enter to exit.");
            Console.ReadLine();

        }
    }

    class main
    {
        static string root = Directory.GetCurrentDirectory();
        static string log = Path.Combine(root, "LegacyShears.log");

        static List<string> exceptions = new List<string>()
        {
            "Common\\Trial",
            "Common\\res",
            "Common\\Media\\de-DE",
            "Common\\Media\\es-ES",
            "Common\\Media\\font",
            "Common\\Media\\fr-FR",
            "Common\\Media\\Graphics",
            "Common\\Media\\it-IT",
            "Common\\Media\\ja-JP",
            "Common\\Media\\ko-KR",
            "Common\\Media\\pt-BR",
            "Common\\Media\\pt-PT",
            "Common\\Media\\Sound",
            "Common\\Media\\zh-CHT",
            "Common\\Media\\4J_strings.resx",
            "Common\\Media\\MediaWindows64.arc",
            "Durango\\Sound",
            "Windows64\\gameHDD",
            "Windows64Media\\DLC",
            "Windows64Media\\loc",
        };

        public static void Start()
        {
            if (File.Exists(log))
                File.Delete(log);

            Log("Starting Shears\n");

            DeleteIfExists("Minecraft.Client.pdb");
            DeleteIfExists("Minecraft.Client.ilk");
            DeleteIfExists("Minecraft.Client.pch");
            DeleteIfExists("windows.xbox.networking.realtimesession.dll");
            DeleteIfExists("windows.xbox.networking.realtimesession.pdb");
            DeleteIfExists("windows.xbox.networking.realtimesession.winmd");
            DeleteIfExists("Effects.msscmp");

            DeleteIfExists("Comman\\Trial\\TrialMode.cpp");
            DeleteIfExists("Comman\\Trial\\TrialMode.h");



            DeleteAllExcept("Windows64Media", exceptions);
            DeleteAllExcept("Common", exceptions);
            DeleteAllExcept("Durango", exceptions);
            DeleteAllExcept("Windows64", exceptions);

            DeleteDirectory("Saves");

        }

        static void DeleteIfExists(string relpath)
        {
            string fullpath = Path.Combine(root, relpath);

            if (File.Exists(fullpath))
            {
                try
                {
                    File.Delete(fullpath);
                    Log($"Deleted file: {relpath}");
                }
                catch (Exception ex)
                {
                    Log($"Failed to delete file: {relpath} | " + ex.Message);
                }
            }
        }

        static void DeleteDirectory(string relpath)
        {
            string fullpath = Path.Combine(root, relpath);

            if (Directory.Exists(fullpath))
            {
                try
                {
                    Directory.Delete(fullpath, true);
                    Log($"Deleted directory: {relpath}");
                }
                catch (Exception ex)
                {
                    Log("Failed to delete directory: {relativePath} | " + ex.Message);
                }
            }
        }

        static void DeleteAllExcept(string relfolder, List<string> exceptions)
        {
            string fullpath = Path.Combine(root, relfolder);
            if (!Directory.Exists(fullpath))
                return;

            ProcessFolder(fullpath, relfolder, exceptions);
        }

        static void ProcessFolder(string fullPath, string relativePath, List<string> exceptions)
        {
            foreach (var file in Directory.GetFiles(fullPath))
            {
                string rel = Path.Combine(relativePath, Path.GetFileName(file));

                if (IsException(rel, exceptions))
                    continue;

                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                    Log($"Deleted file: {rel}");
                }
                catch (Exception ex)
                {
                    Log($"Failed to delete file: {rel} | {ex.Message}");
                }
            }

            foreach (var dir in Directory.GetDirectories(fullPath))
            {
                string folderName = Path.GetFileName(dir);
                string rel = Path.Combine(relativePath, folderName);

                if (IsException(rel, exceptions))
                {
                    Log($"Kept directory: {rel}");
                    continue;
                }

                ProcessFolder(dir, rel, exceptions);

                if (!Directory.EnumerateFileSystemEntries(dir).Any())
                {
                    try
                    {
                        Directory.Delete(dir);
                        Log($"Deleted directory: {rel}");
                    }
                    catch (Exception ex)
                    {
                        Log($"Failed to delete directory: {rel} | {ex.Message}");
                    }
                }
            }
        }

        static bool IsException(string path, List<string> exceptions)
        {
            foreach (var ex in exceptions)
            {
                if (path.Replace("/", "\\")
                        .StartsWith(ex.Replace("/", "\\"), StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        static void Log(string text)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("H:mm:ss")}] " + text);
            File.AppendAllText(log, text + Environment.NewLine);
        }
    }
}
