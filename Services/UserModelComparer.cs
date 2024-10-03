using oa.Models;
using System.Reflection;

namespace oa.Services
{
    public class UserModelComparer
    {
        public UserModelComparer() { }
        public List<LogModel> Compare(AppUserModel oldUser, AppUserModel newUser)
        {
            
            List<LogModel> logList = new List<LogModel>();
            PropertyInfo[] properties = typeof(AppUserModel).GetProperties();
            foreach(PropertyInfo property in properties)
            {
                object oldValue = property.GetValue(oldUser, null);
                object newValue = property.GetValue(newUser, null);
                if(!Equals(oldValue, newValue))
                {
                    LogModel logModel = new LogModel();
                    logModel.ChangeDate = DateTime.Now;
                    logModel.UserId = oldUser.Id;
                    string changedProperty = property.Name;
                    string oldValueString = oldValue?.ToString() ?? "null";
                    string newValueString = newValue?.ToString() ?? "null";
                    oldValueString = changedProperty + ": " + oldValueString;
                    newValueString = changedProperty + ": " + newValueString;
                    logModel.ChangedFrom = oldValueString;
                    logModel.ChangedTo = newValueString;
                    logList.Add(logModel);
                }
            }
            //iterate through the loglist to get rid of null entries.
            for(int i = 0; i <= logList.Count; i++)
            {
                if (logList[i].ChangedFrom.Contains("null") || logList[i].ChangedTo.Contains("null"))
                {
                    logList.RemoveAt(i);
                }
            }


            return logList;
        }
    }
}
