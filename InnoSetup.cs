using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Tasks;

namespace InnoSetupTasks
{
    [TaskName("innosetup")]
    public class InnoSetupTask : ExternalProgramBase
    {
        private string BuildProgramArguments()
        {
            StringBuilder arguments = new StringBuilder();

            if (Output != null)
            {
                arguments.AppendFormat("/o\"{0}\" ", OutputPath);
            }

            if (BaseFilename != null)
            {
                arguments.AppendFormat("/f\"{0}\" ", BaseFilename);
            }

            arguments.AppendFormat("\"{0}\"", Script);

            return arguments.ToString();
        }

        private const string InnoSetupDirectoryName = @"Inno Setup 5\";
        private const string InnoSetupExe = @"iscc.exe";

        private string FindInnoSetupExe()
        {
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            if (!programFiles.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                programFiles = programFiles + System.IO.Path.DirectorySeparatorChar;
            }
            
            string exePath = InnoSetupExe;

            if (Directory.Exists(programFiles + InnoSetupDirectoryName))
            {
                exePath = programFiles + InnoSetupDirectoryName + exePath;
                if (!File.Exists(exePath))
                {
                    this.Log(Level.Info, "Inno Setup executable not found at '{0}'. Assuming it's on the path.", new object[] { exePath });
                }
            }
            else
            {
                this.Log(Level.Info, "Inno Setup directory not found at '{0}'.", new object[] { programFiles + InnoSetupDirectoryName });
                programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                if (!programFiles.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                {
                    programFiles = programFiles + System.IO.Path.DirectorySeparatorChar;
                }

                if (Directory.Exists(programFiles + InnoSetupDirectoryName))
                {
                    exePath = programFiles + InnoSetupDirectoryName + exePath;
                    if (!File.Exists(exePath))
                    {
                        this.Log(Level.Info, "Inno Setup executable not found at '{0}'. Assuming it's on the path.", new object[] { exePath });
                    }
                }
            }

            this.Log(Level.Info, "Using Inno Setup executable '{0}'.", new object[] { exePath });
            return exePath;
        }

        public override string ProgramArguments
        {
            get { return BuildProgramArguments(); }
        }

        public override string ProgramFileName
        {
            get
            {
                return FindInnoSetupExe();
            }
        }

        [TaskAttribute("script", Required = true)]
        public FileInfo Script { get; set; }

        [TaskAttribute("output", Required = false)]
        public DirectoryInfo OutputPath { get; set; }

        [TaskAttribute("basefilename", Required = false)]
        public string BaseFilename { get; set; }
    }

}
