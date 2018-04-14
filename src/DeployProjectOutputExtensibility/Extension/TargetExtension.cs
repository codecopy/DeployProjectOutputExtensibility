using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TP.AutoDeploy.Models;

namespace TP.AutoDeploy.Extension
{
    public static class TargetExtension
    {
        /// <summary>
        /// Updates the parent object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="userMetadata">The user metadata.</param>
        /// <exception cref="ArgumentException">Inherit is True but Parent is Null</exception>
        public static void UpdateData(this TargetInfoBase target, UserMetadata userMetadata)
        {
            target.CommonTarget = new ObservableCollection<TargetInfoBase>(userMetadata.CommonTarget);
            if (target.Inherit && target != null && userMetadata != null)
            {
                if (string.IsNullOrWhiteSpace(target.Parent))
                {
                    throw new ArgumentException($"Inherit is True but Parent is Null. Target name is {target.Name}");
                }

                target.TargetDir = string.IsNullOrWhiteSpace(target.TargetDir)
                                                ? string.Empty :
                                                target.TargetDir;

                var parent = userMetadata.CommonTarget.FirstOrDefault(tg => tg.Name.Equals(target.Parent));
                if (parent != null)
                {
                    parent.UpdateData(userMetadata);
                    target.ParentObject = parent;
                }
            }
        }

        /// <summary>
        /// Gets the target path.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static string GetAbsolute(this TargetInfoBase target)
        {
            var result = target.TargetDir;
            if (target.Inherit && target.ParentObject != null)
            {
                var parentPath = target.ParentObject.GetAbsolute();
                var targetDir = new DirectoryInfo(Path.Combine(parentPath, result));
                result = targetDir.FullName;
            }

            return result;
        }
    }
}
