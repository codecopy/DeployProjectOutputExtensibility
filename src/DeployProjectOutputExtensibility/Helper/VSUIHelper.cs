using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace TP.AutoDeploy.Helper
{
    public static class VSUIHelper
    {
        /// <summary>
        /// Shows the message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The level.</param>
        /// <returns>Result</returns>
        public static int ShowMessageBox(string message, OLEMSGICON level = OLEMSGICON.OLEMSGICON_INFO)
        {
            var vsUIShell = (IVsUIShell)VSContext.ServiceProvider.GetService(typeof(SVsUIShell));
            var empty = Guid.Empty;
            int refNum;
            return ErrorHandler.ThrowOnFailure(vsUIShell.ShowMessageBox(0u, ref empty, string.Empty,
                message, string.Empty, 0u, 0, 0, level, 0, out refNum));
        }
    }
}
