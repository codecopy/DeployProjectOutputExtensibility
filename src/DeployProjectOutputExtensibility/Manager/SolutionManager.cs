using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TP.AutoDeploy.Configuration;

namespace TP.AutoDeploy.Manager
{
    public class SolutionManager: IDisposable
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static SolutionManager Instance { get; } = new SolutionManager();

        #region Variables

        /// <summary>
        /// The service provider
        /// </summary>
        private IServiceProvider serviceProvider;

        /// <summary>
        /// The caching project dictionary
        /// </summary>
        private Dictionary<string, Project> cachingProjectDict = new Dictionary<string, Project>();

        /// <summary>
        /// The vs monitor selection
        /// </summary>
        private DTE dte;

        /// <summary>
        /// The vs solution
        /// </summary>
        private IVsSolution vsSolution;

        /// <summary>
        /// The solution events
        /// </summary>
        private SolutionEvents solutionEvents;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there is a solution open in the IDE.
        /// </summary>
        public bool IsSolutionOpen
        {
            get
            {
                return this.dte != null &&
                       this.dte.Solution != null &&
                       this.dte.Solution.IsOpen &&
                       !this.IsSolutionSavedAsRequired();
            }
        }

        #endregion
        /// <summary>
        /// Prevents a default instance of the <see cref="SolutionManager"/> class from being created.
        /// </summary>
        private SolutionManager()
        {
        }

        /// <summary>
        /// Initializes the specified service provider.
        /// </summary>
        /// <param name="svrrovider">The service provider.</param>
        public void Initialize(IServiceProvider svrrovider)
        {
            this.serviceProvider = svrrovider;
            this.dte = this.serviceProvider.GetService(typeof(DTE)) as DTE;
            this.vsSolution = this.serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;

            this.solutionEvents = this.dte.Events.SolutionEvents;

            this.RegisterSolutionEvent();
            this.EnsureProjectCache();
        }

        /// <summary>
        /// Solutions the directory.
        /// </summary>
        /// <returns></returns>
        public string GetSolutionDirectory()
        {
            if (!IsSolutionOpen)
            {
                return null;
            }

            string solutionFilePath = this.GetSolutionFilePath();

            if (String.IsNullOrEmpty(solutionFilePath))
            {
                return null;
            }
            return Path.GetDirectoryName(solutionFilePath);
        }

        /// <summary>
        /// Gets the solution file path.
        /// </summary>
        /// <returns></returns>
        public string GetSolutionFilePath()
        {
            // Use .Properties.Item("Path") instead of .FullName because .FullName might not be
            // available if the solution is just being created
            string solutionFilePath = null;

            var property = this.dte.Solution.Properties.Item("Path");
            if (property == null)
            {
                return null;
            }
            try
            {
                // When using a temporary solution, (such as by saying File -> New File), querying this value throws.
                // Since we wouldn't be able to do manage any packages at this point, we return null. Consumers of this property typically 
                // use a String.IsNullOrEmpty check either way, so it's alright.
                solutionFilePath = property.Value.ToString();
            }
            catch (COMException)
            {
                return null;
            }

            return solutionFilePath;
        }

        /// <summary>
        /// Gets the projects.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Project> GetProjects()
        {
            if (this.IsSolutionOpen)
            {
                return this.cachingProjectDict.Values;
            }
            else
            {
                return Enumerable.Empty<Project>();
            }
        }

        /// <summary>
        /// Gets the project.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Project GetProject(string name)
        {
            name = name.ToLower();
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (this.cachingProjectDict.ContainsKey(name))
                {
                    return this.cachingProjectDict[name];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the active file path.
        /// </summary>
        /// <returns>Active doc file path</returns>
        public string GetActiveFilePath()
        {
            var applicationObject = this.serviceProvider.GetService(typeof(DTE)) as EnvDTE80.DTE2;
            return applicationObject.ActiveDocument.FullName;
        }

        /// <summary>
        /// Loads the selected project.
        /// </summary>
        /// <exception cref="NotSupportedException">Cannot create window.</exception>
        public Project GetActivatedProject()
        {
            var activateProjects = ((Array)((DTE2)this.dte).ActiveSolutionProjects);
            if (activateProjects.Length > 0)
            {
                return activateProjects.GetValue(0) as Project;
            }

            return null;
        }

        /// <summary>
        /// Checks whether the current solution is saved to disk, as opposed to be in memory.
        /// </summary>
        public bool IsSolutionSavedAsRequired()
        {
            // Check if user is doing File - New File without saving the solution.
            object value;
            this.vsSolution.GetProperty((int)(__VSPROPID.VSPROPID_IsSolutionSaveAsRequired), out value);
            if ((bool)value)
            {
                return true;
            }

            // Check if user unchecks the "Tools - Options - Project & Soltuions - Save new projects when created" option
            this.vsSolution.GetProperty((int)(__VSPROPID2.VSPROPID_DeferredSaveSolution), out value);
            return (bool)value;
        }

        /// <summary>
        /// Ensures the project cache.
        /// </summary>
        private void EnsureProjectCache()
        {
            if (this.IsSolutionOpen)
            {
                var allProjects = this.dte.Solution.Projects;
                foreach (Project project in allProjects)
                {
                    AddProjectToCache(project);
                }
            }
        }

        /// <summary>
        /// Adds the project to cache.
        /// </summary>
        /// <param name="project">The project.</param>
        private void AddProjectToCache(Project project)
        {
            if (!this.cachingProjectDict.ContainsKey(project.Name.ToLower()))
            {
                this.cachingProjectDict.Add(project.Name.ToLower(), project);
            }
        }

        /// <summary>
        /// Adds the project to cache.
        /// </summary>
        /// <param name="project">The project.</param>
        private void RemoveProjectFromCache(Project project)
        {
            if (this.cachingProjectDict.ContainsKey(project.Name.ToLower()))
            {
                this.cachingProjectDict.Remove(project.Name.ToLower());
            }
        }

        #region Implement IDisposeable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Registers the solution event.
        /// </summary>
        private void RegisterSolutionEvent()
        {
            // Register project events
            this.solutionEvents.ProjectAdded += OnProjectAdded;
            this.solutionEvents.ProjectRemoved += OnProjectRemoved;
            this.solutionEvents.ProjectRenamed += OnProjectRenamed;

            // Register solution events
            this.solutionEvents.Opened += OnSolutionOpened;
            this.solutionEvents.AfterClosing += OnSolutioClosing;
        }

        /// <summary>
        /// Registers the solution event.
        /// </summary>
        private void UnregisterSolutionEvent()
        {
            // Register project events
            this.solutionEvents.ProjectAdded -= OnProjectAdded;
            this.solutionEvents.ProjectRemoved -= OnProjectRemoved;
            this.solutionEvents.ProjectRenamed -= OnProjectRenamed;

            // Register solution events
            this.solutionEvents.Opened -= OnSolutionOpened;
            this.solutionEvents.AfterClosing -= OnSolutioClosing;
        }

        /// <summary>
        /// Called when [solution closing].
        /// </summary>
        private void OnSolutioClosing()
        {
            var configPrd = ConfigurationProvider.Instance;
            if (File.Exists(configPrd.ConfigFilePath))
            {
                configPrd.Save();
            }
            this.cachingProjectDict.Clear();
        }

        /// <summary>
        /// Called when [solution opened].
        /// </summary>
        private void OnSolutionOpened()
        {
            this.Name = this.dte.Solution.FileName;
            ConfigurationProvider.Instance.LoadConfig();

            this.EnsureProjectCache();
        }

        /// <summary>
        /// Called when [project renamed].
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="oldName">The old name.</param>
        private void OnProjectRenamed(Project project, string oldName)
        {
            var exsitingProject = this.GetProject(oldName);
            if (exsitingProject != null)
            {
                this.RemoveProjectFromCache(exsitingProject);
            }
            this.OnProjectAdded(project);
        }

        /// <summary>
        /// Called when [project removed].
        /// </summary>
        /// <param name="project">The project.</param>
        private void OnProjectRemoved(Project project)
        {
            this.RemoveProjectFromCache(project);
        }

        /// <summary>
        /// Called when [project added].
        /// </summary>
        /// <param name="project">The project.</param>
        private void OnProjectAdded(Project project)
        {
            this.AddProjectToCache(project);
        }
    }
}
