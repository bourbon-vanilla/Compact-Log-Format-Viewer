namespace LogViewer.Wpf.Client.Helper
{
    internal static class SystemDialog
    {
        public static string GetOpenFileDialogFilterString(string[] fileExtensions)
        {
            var filterString = "Framework files (";
            var tempString = string.Empty;
            for (var i = 0; i < fileExtensions.Length; i++)
            {
                filterString += "*.";
                tempString += "*.";
                filterString += fileExtensions[i];
                tempString += fileExtensions[i];
                if (i != fileExtensions.Length - 1)
                {
                    filterString += ",";
                    tempString += ";";
                }
                else
                    filterString += ")";
            }
            filterString += "|" + tempString + "|" + "All files (*.*)|*.*";
            return filterString;
        }
    }
}
