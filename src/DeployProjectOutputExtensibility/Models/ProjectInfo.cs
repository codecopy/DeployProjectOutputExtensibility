using EnvDTE;
using PropertyChanged;
using System;
using System.IO;

namespace TP.AutoDeploy.Models
{
    [ImplementPropertyChanged]
    public class ProjectInfo
    {
        [DoNotNotify]
        public Project RawProject { get; }

        public string ProjectName { get; set; }

        public string Output { get; set; }

        public ProjectInfo(Project project)
        {
            this.RawProject = project;
            this.ProjectName = project.Name;
            this.Output = this.GetProjectOutput();
        }

        /// <summary>
        /// Gets the project output.
        /// </summary>
        /// <returns></returns>
        public string GetProjectOutput()
        {
            var binFolder = string.Empty;
            if (this.RawProject != null && !string.IsNullOrEmpty(this.RawProject.FullName))
            {
                string fullPath = this.RawProject.Properties.Item("FullPath").Value.ToString();
                string outputPath = this.RawProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
                string outputFileName = this.RawProject.Properties.Item("OutputFileName").Value.ToString();


                string outputDir = Path.Combine(fullPath, outputPath);
                var folderObj = new DirectoryInfo(outputDir);
                binFolder = Path.Combine(folderObj.FullName, outputFileName);
            }

            return binFolder;
        }

        /// <summary>
        /// Deploy project output
        /// </summary>
        /// <param name="target"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Deploy(TargetInfo target, out string message)
        {
            message = string.Empty;
            if (File.Exists(this.Output))
            {
                var fileName = Path.GetFileName(this.Output);
                var destDir = target.AbsoluteDir;
                var destFilePath = Path.Combine(destDir, fileName);

                try
                {
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    File.Copy(this.Output, destFilePath, target.OverrideMode);
                    message = $"Deploy {this.ProjectName} successfully.";
                    return true;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            else
            {
                message = "The file is not found.";
            }
            return false;
        }
    }
}
