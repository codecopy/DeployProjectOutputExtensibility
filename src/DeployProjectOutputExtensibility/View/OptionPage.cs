using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using TP.AutoDeploy.Configuration;
using TP.AutoDeploy.Helper;
using TP.AutoDeploy.Manager;
using TP.AutoDeploy.Models;

namespace TP.AutoDeploy.View
{
    public class ModelMark
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is new.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsNew { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has changed; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasChanged { get; }
    }

    public class TargetConfig : ModelMark
    {
        /// <summary>
        /// The raw data
        /// </summary>
        private TargetInfoBase rawData;

        /// <summary>
        /// Gets or sets a value indicating whether this instance has changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has changed; otherwise, <c>false</c>.
        /// </value>
        public override bool HasChanged => this.rawData != null && !string.Equals(this.rawData.TargetDir, this.TargetDir);

        /// <summary>
        /// Gets or sets a value indicating whether this instance is new.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
        public override bool IsNew => this.rawData == null || !string.Equals(this.rawData.Name, this.Name);

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the deploy location.
        /// </summary>
        /// <value>
        /// The deploy location.
        /// </value>
        public string TargetDir { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public string Parent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TargetInfoBase"/> is inherit.
        /// </summary>
        /// <value>
        ///   <c>true</c> if inherit; otherwise, <c>false</c>.
        /// </value>
        public bool Inherit => !string.IsNullOrWhiteSpace(this.Parent);

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fixed target dir.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fixed target dir; otherwise, <c>false</c>.
        /// </value>
        public bool IsFixedTargetDir => !this.Inherit;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetConfig"/> class.
        /// </summary>
        public TargetConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetConfig"/> class.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        public TargetConfig(TargetInfoBase rawData)
        {
            this.Name = rawData.Name;
            this.TargetDir = rawData.TargetDir;
            this.Parent = rawData.Parent;
            this.rawData = rawData;
        }
    }
    
    public class OptionPage : DialogPage, IToolSetting
    {
        /// <summary>
        /// The setting
        /// </summary>
        private IToolSetting setting;

        /// <summary>
        /// The configuration PRD
        /// </summary>
        ConfigurationProvider configPrd = Configuration.ConfigurationProvider.Instance;

        /// <summary>
        /// Gets the installed tool.
        /// </summary>
        /// <value>
        /// The installed tool.
        /// </value>
        [Category("General")]
        [DisplayName("Tool Location")]
        [Description("The installed location of the Tool.")]
        [ReadOnly(true)]
        public string InstalledTool { get; private set; }

        /// <summary>
        /// Gets or sets the maximum history.
        /// </summary>
        /// <value>
        /// The maximum history.
        /// </value>
        [Category("General")]
        [DisplayName("Maximum recent")]
        [Description("Input maximum location will be stored in data.")]
        public int MaxHistory { get; set; }

        [Category("Target")]
        [DisplayName("Common Targets")]
        [Description("Define common deploy target.")]
        public List<TargetConfig> CommonTargets { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionPage"/> class.
        /// </summary>
        public OptionPage()
        {
            this.InstalledTool = Path.GetDirectoryName(this.GetType().Assembly.Location);
        }

        /// <summary>
        /// Resets the setting.
        /// </summary>
        private void ResetSetting()
        {
            this.setting = configPrd.Setting;
            this.MaxHistory = this.setting.MaxHistory;
        }

        /// <summary>
        /// Raises the <see cref="E:Activate" /> event.
        /// </summary>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);
            this.ResetSetting();

            // Load user data
            this.CommonTargets?.Clear();
            if (SolutionManager.Instance.IsSolutionOpen)
            {
                this.CommonTargets = this.configPrd.UserMetadata.CommonTarget.Select(i => new TargetConfig(i)).ToList();
            }
        }

        /// <summary>
        /// Saves the settings to storage.
        /// </summary>
        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();
            configPrd.Setting = new ToolSetting(this);

            try
            {
                this.configPrd.SaveSetting();

                if (this.UpdateUserData())
                {
                    this.configPrd.Save();
                }
            }
            catch (Exception ex)
            {
                EnvironmentHelper.WriteError($"Got an exception while saving Tool setting. {ex}");
                VSUIHelper.ShowMessageBox($"Got an exception while saving Tool setting. {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the user data.
        /// </summary>
        private bool UpdateUserData()
        {
            if (!SolutionManager.Instance.IsSolutionOpen)
            {
                return false;
            }

            // Update common target
            var hasChanged = false;
            if (this.CommonTargets.Any())
            {
                if (this.CommonTargets.Count != configPrd.UserMetadata.CommonTarget.Count || 
                    this.CommonTargets.Any(i => i.IsNew || i.HasChanged))
                {
                    configPrd.UserMetadata.CommonTarget = this.CommonTargets.Select(item =>
                    {
                        return new TargetInfoBase
                        {
                            Name = item.Name,
                            TargetDir = item.TargetDir,
                            Parent = item.Parent,
                            IsFixedTargetDir = item.IsFixedTargetDir,
                            Inherit = item.Inherit
                        };
                    }).ToList();
                    hasChanged = true;
                }
            }
            else
            {
                configPrd.UserMetadata.CommonTarget?.Clear();
                hasChanged = true;
            }

            if (hasChanged)
            {
                configPrd.UserMetadata.UpdateData();
            }

            return hasChanged;
        }

        /// <summary>
        /// Resets the settings.
        /// </summary>
        public override void ResetSettings()
        {
            base.ResetSettings();
            this.ResetSetting();
        }
    }
}
