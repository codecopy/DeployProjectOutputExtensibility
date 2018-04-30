﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.AutoDeploy.Helper;
using TP.AutoDeploy.Manager;
using TP.AutoDeploy.Models;

namespace TP.AutoDeploy.Configuration
{
    public class ConfigurationProvider
    {
        /// <summary>
        /// The configuration file extension
        /// </summary>
        private const string ConfigFileExtension = ".tpe";

        /// <summary>
        /// The specific extension folder name
        /// </summary>
        private const string SpecificExtensionFolderName = @"TP.Extension\AutoDeploy";

        /// <summary>
        /// The user data configuration file name
        /// </summary>
        private const string UserDataConfigFileName = "StructureConfig";

        /// <summary>
        /// The instance
        /// </summary>
        private static Lazy<ConfigurationProvider> instance = new Lazy<ConfigurationProvider>(() => new ConfigurationProvider());

        /// <summary>
        /// Gets or sets the configuration file path.
        /// </summary>
        /// <value>
        /// The configuration file path.
        /// </value>
        public string ConfigFilePath { get; private set; }

        /// <summary>
        /// Gets the solution manager.
        /// </summary>
        /// <value>
        /// The solution manager.
        /// </value>
        private SolutionManager solutionManager => SolutionManager.Instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ConfigurationProvider Instance => instance.Value;

        /// <summary>
        /// The current data
        /// </summary>
        public UserMetadata UserMetadata { get; private set; }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <returns></returns>
        public UserMetadata LoadConfig()
        {
            var configFile = this.GetUserConfigFile();
            this.ConfigFilePath = configFile;

            if (File.Exists(configFile))
            {
                this.UserMetadata = XmlHelper.LoadFromFile< UserMetadata>(configFile);
            }
            else
            {
                this.UserMetadata = new UserMetadata();
            }

            this.UserMetadata.CommonTarget.Add(new TargetInfoBase
            {
                Name = "SMEE",
                TargetDir = @"C:\Program Files (x86)\SMEE"
            });
            this.UserMetadata.CommonTarget.Add(new TargetInfoBase
            {
                Name = "PLATFORM",
                TargetDir = "PLATFORM-LIBS",
                Inherit = true,
                Parent = "SMEE"
            });
            this.UserMetadata.CommonTarget.Add(new TargetInfoBase
            {
                Name = "MODULE-LIBS",
                TargetDir = "MODULE-LIBS",
                Inherit = true,
                Parent = "SMEE"
            });
            this.UserMetadata.CommonTarget.Add(new TargetInfoBase
            {
                Name = "NADAE",
                TargetDir = "NADAE",
                Inherit = true,
                Parent = "SMEE"
            });
            this.UserMetadata.CommonTarget.Add(new TargetInfoBase
            {
                Name = "TOOLS",
                TargetDir = "TOOLS",
                Inherit = true,
                Parent = "SMEE"
            });

            this.UserMetadata.UpdateData();
            return this.UserMetadata;
        }

        /// <summary>
        /// Gets the user configuration file.
        /// </summary>
        /// <returns></returns>
        private string GetUserConfigFile()
        {
            var solutionDir = this.solutionManager.GetSolutionDirectory();
            var configFile = Path.Combine(solutionDir, $"{this.solutionManager.Name}{ConfigFileExtension}");

            if (File.Exists(configFile))
            {
                return configFile;
            }

            var commonConfigFileName = $"{UserDataConfigFileName}{ConfigFileExtension}";
            configFile = Path.Combine(solutionDir, commonConfigFileName);
            if (File.Exists(configFile))
            {
                return configFile;
            }

            return configFile;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            XmlHelper.SaveToFile(this.UserMetadata, this.ConfigFilePath);
        }
    }
}
