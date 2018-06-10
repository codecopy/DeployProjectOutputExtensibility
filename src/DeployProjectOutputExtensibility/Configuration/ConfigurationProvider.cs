using System;
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
        /// The tool setting file n ame
        /// </summary>
        private const string ToolSettingFileName = "Setting.xml";

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
        /// The installation path
        /// </summary>
        private string installationPath;

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
        /// Gets or sets the setting.
        /// </summary>
        /// <value>
        /// The setting.
        /// </value>
        public IToolSetting Setting { get; set; }

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
                // Load default data
                var rootPath = Path.GetDirectoryName(typeof(ConfigurationProvider).Assembly.Location);
                var defaultConfigFile = Path.Combine(rootPath, @"Resources\DefaultConfig.xml");
                this.UserMetadata = XmlHelper.LoadFromFile<UserMetadata>(defaultConfigFile);
            }

            this.UserMetadata.UpdateData();
            return this.UserMetadata;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationProvider"/> class.
        /// </summary>
        public ConfigurationProvider()
        {
            this.installationPath = Path.GetDirectoryName(this.GetType().Assembly.Location);
            this.LoadSeting();
        }

        /// <summary>
        /// Loads the seting.
        /// </summary>
        private void LoadSeting()
        {
            var settingFile = Path.Combine(installationPath, ToolSettingFileName);

            if (File.Exists(settingFile))
            {
                this.Setting = XmlHelper.LoadFromFile<ToolSetting>(settingFile);
            }
            else
            {
                this.Setting = new ToolSetting();
            }
        }

        /// <summary>
        /// Saves the setting.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void SaveSetting()
        {
            var settingFile = Path.Combine(installationPath, ToolSettingFileName);
            XmlHelper.SaveToFile((ToolSetting) this.Setting, settingFile);
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
