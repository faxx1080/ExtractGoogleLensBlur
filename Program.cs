// Copyright Frank Migliorino, 2015. Licenced under GPLv3
// See LICENSE file
// Exiftool is from http://www.sno.phy.queensu.ca/~phil/exiftool/
// Written by Phil Harvey

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace ExtractGoogleLensBlur
{
    class Program
    {
        static void Main(string[] args)
        {

            Process p1 = new Process();
            p1.StartInfo.UseShellExecute = false;
            p1.StartInfo.FileName = "exiftool.exe";
            // p1.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + "\\Externals";
            p1.StartInfo.CreateNoWindow = true;
            p1.StartInfo.Arguments = " -xmp -b -a " + args[0];
            Console.WriteLine("Running " + p1.StartInfo.FileName + " " + p1.StartInfo.Arguments);
            p1.StartInfo.RedirectStandardOutput = true;
            p1.Start();

            StreamReader reader = p1.StandardOutput;
            string output = reader.ReadToEnd();

            p1.WaitForExit();
            p1.Close();

            Console.WriteLine("Finished Running exiftool.exe -xmp -b -a -w xmp " + args[0]);

            // get GImage:Data
            output = output.Substring(output.IndexOf("GImage:Data=\"") + "GImage:Data=\"".Length);
            string[] outarr = output.Split('\n');

            outarr[0] = outarr[0].Remove(outarr[0].Length - 1); // Image Data
            outarr[1] = outarr[1].Substring(outarr[1].IndexOf("GDepth:Data=\"") + "GDepth:Data=\"".Length);
            outarr[1] = outarr[1].Remove(outarr[1].Length - 3); // Depth Data

            byte[] img = Convert.FromBase64String(outarr[0]);
            byte[] depth = Convert.FromBase64String(outarr[1]);

            // Get output path
            int lastPathSep = args[0].LastIndexOf("\\");
            string outPath = System.IO.Directory.GetCurrentDirectory();
            string outStName = args[0];
            if (lastPathSep > 1) {
                outPath = args[0].Remove(lastPathSep + 1);
                outStName = args[0].Substring(lastPathSep+1);
            }

            outStName = outStName.Remove(outStName.Length - 4);

            File.WriteAllBytes(outPath + outStName + "-img.jpg",img);
            File.WriteAllBytes(outPath + outStName + "-depth.jpg",depth);

            Console.WriteLine("Done");
            Console.ReadLine();

        }

    }
}
