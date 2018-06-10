using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.AutoDeploy.Models;

namespace TP.AutoDeploy.Extension
{
    public static class AnonymousDataEx
    {
        public const int MaxLocationHistoryItem = 5;

        public static List<string> GetRecentLocations(this AnonymousData anonymousData, string groupName)
        {
            List<string> result = new List<string>();
            var locationHistories = anonymousData.HistoryGroups;
            if (locationHistories == null)
            {
                return result;
            }

            var group = locationHistories.FirstOrDefault(h => string.Equals(h.Name, groupName));
            if (group == null)
            {
                var allLocations = locationHistories.SelectMany(l => l.Histories).Distinct();
                result.AddRange(allLocations);
            }
            else
            {
                result.AddRange(group.Histories);
            }

            return result;
        }

        public static void UpdateLocationHistory(this AnonymousData anonymousData, string groupName, string recentLocation)
        {
            groupName = groupName?.Trim();
            recentLocation = recentLocation?.Trim();

            if (string.IsNullOrWhiteSpace(groupName) || string.IsNullOrWhiteSpace(recentLocation))
            {
                return;
            }

            var locationHistories = anonymousData.HistoryGroups;
            if (locationHistories == null)
            {
                locationHistories = new List<HistoryGroup>();
            }

            var group = locationHistories.FirstOrDefault(h => string.Equals(h.Name, groupName));
            if (group == null)
            {
                group = new HistoryGroup
                {
                    Name = groupName,
                    Histories = new List<string>() { recentLocation }
                };
            }
            else
            {
                var exstingLocation = group.Histories.IndexOf(recentLocation);
                if (exstingLocation < 0)
                {
                    group.Histories.Insert(0, recentLocation);
                }
                else
                {
                    group.Histories.RemoveAt(exstingLocation);
                    group.Histories.Insert(0, recentLocation);
                }

                var maxLocation = Configuration.ConfigurationProvider.Instance.Setting?.MaxHistory ?? MaxLocationHistoryItem;
                if (group.Histories.Count > maxLocation)
                {
                    group.Histories.RemoveRange(MaxLocationHistoryItem, group.Histories.Count - maxLocation);
                }

                locationHistories.Remove(group);
            }

            locationHistories.Insert(0, group);
        }
    }
}
