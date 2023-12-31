// A custom ZIP-based format designed to enhance the Sims 1 modding experience.
// Copyright (C) 2023 Tony Bark
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// this program.  If not, see <http://www.gnu.org/licenses/>.
using System.IO.Compression;

namespace S1Util;

public class PackUtil
{
    public PackUtil(string destination, string file, bool simulate, bool verbose)
    {
        Destination = destination;
        FileName = file;
        Simulate = simulate;
        Verbose = verbose;
    }

    string Destination { get; set; } = string.Empty;
    string FileName { get; set; } = string.Empty;
    bool Simulate { get; set; } = false;
    bool Verbose { get; set; } = false;

    public void Extract()
    {
        // Use as placeholder
        if (Simulate) Destination = Environment.CurrentDirectory;

        // Check if the s1pk file exists
        if (!File.Exists(FileName))
        {
            Console.WriteLine("The file does not exist.");
            Environment.Exit(Environment.ExitCode);
        }

        var os = Environment.OSVersion;

        if (os.Platform == PlatformID.Win32NT && Destination == string.Empty && !Simulate)
        {
            // TODO Presumed location. I haven't played the game in a while.
            var progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var gamePath = Path.Combine(progFiles, "The Sims Complete Collection", "Downloads");

            Destination = gamePath;
        }


        // Check file extensions
        if (!FileName.EndsWith("s1pk", StringComparison.InvariantCultureIgnoreCase)
        || !FileName.EndsWith("sims1pack", StringComparison.InvariantCultureIgnoreCase)
        || !FileName.EndsWith("zip", StringComparison.InvariantCultureIgnoreCase))
        {
            Console.WriteLine("Invalid format");
            Environment.Exit(Environment.ExitCode);
        }

        // Check if game directory exists
        if (!Directory.Exists(Destination) && !Simulate)
        {
            Console.WriteLine("Game not found.");
            Environment.Exit(Environment.ExitCode);
        }

        // Extract the s1pk file
        using var archive = ZipFile.OpenRead(FileName);

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            // Check if the entry has the .iff extension
            if (entry.Name.EndsWith(".iff", StringComparison.InvariantCultureIgnoreCase))
            {
                // Build the full path for the extracted file
                var extractFilePath = Path.Combine(Destination, entry.Name);


                Console.WriteLine(extractFilePath);

                // Extract the file
                if (!Simulate)
                    entry.ExtractToFile(extractFilePath, true);
            }
        }

        Console.WriteLine("Extraction completed successfully.");

    }

    public void Compress()
    {
        using var zipFile = File.Open(FileName, FileMode.Create);
        using var archive = new ZipArchive(zipFile);

        var directory = new DirectoryInfo(Destination);
        if (!directory.Exists)
        {
            Console.WriteLine("Directory does not exist.");
            Environment.Exit(Environment.ExitCode);
        }

        foreach (var dir in directory.EnumerateFiles("*.iff"))
            archive.CreateEntry(dir.FullName);


    }
}