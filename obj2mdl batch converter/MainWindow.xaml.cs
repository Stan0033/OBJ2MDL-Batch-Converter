using System.Windows;
using System.IO;
using System;
namespace obj2mdl_batch_converter
{
    // (C) 2024 stan0033

    static class ProgramInfo 
    {
        public static string Version = "1.0.7";
        public static string Name = "OBJ2MDL Batch Converter";
        public static string GetTime()
        {
            // Get the local time zone
            TimeZoneInfo timeZone = TimeZoneInfo.Local;

            // Get the standard name of the time zone
            string timeZoneName = timeZone.StandardName; // Use StandardName or DaylightName as needed

            // Get the UTC offset
            TimeSpan offset = timeZone.GetUtcOffset(DateTime.Now);

            // Format the offset as hours and minutes
            string timeZoneOffset = $"{(offset.Hours >= 0 ? "+" : "")}{offset.Hours:D2}:{offset.Minutes:D2}";

            // Format the timestamp
            string timestamp = $"{DateTime.Now:dd MMMM yyyy 'at' HH:mm:ss} {timeZoneName} (UTC{timeZoneOffset})";

            return timestamp;
        }


    }
    public partial class MainWindow : Window
    {

        public MainWindow()
        { InitializeComponent();
            Title = $"{ProgramInfo.Name} v{ProgramInfo.Version}";
        }
        private void OnLabel_Drop(object sender, DragEventArgs e)
        {
            bool MultipleGeosets = this.MultipleGeosets.IsChecked == true;
            bool MutlipleBones = MultipleBones.IsChecked == true;
            bool MultipleMats = MultipleMaterials.IsChecked == true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (System.IO.Path.GetExtension(file).ToLower() == ".obj")
                    {
                        if (ObjValidator.Validate(file) == false) { continue; }
                        string targetPath = Path.Combine(Path.GetDirectoryName(file), $"{Path.GetFileNameWithoutExtension(file)}.mdl");
                        if (MultipleGeosets == false)
                        {
                            ObjFileParser.Parse(file);
                           ObjFileParser.Save(targetPath);
                        }
                        else
                        {
                            ObjFileParserExtended.Parse(file, MutlipleBones, MultipleMats);
                            ObjFileParserExtended.Save(targetPath, MutlipleBones, MultipleMats);
                        }
                    }
                }
            }
        }
        private void Checked_Geoseta(object sender, RoutedEventArgs e)
        {
            MultipleBones.IsEnabled = MultipleGeosets.IsChecked == true;
            MultipleMaterials.IsEnabled = MultipleGeosets.IsChecked == true;
            if (MultipleGeosets.IsChecked == false) { MultipleBones.IsChecked = false; }
            if (MultipleGeosets.IsChecked == false) { MultipleMaterials.IsChecked = false; }
        }
    }
}