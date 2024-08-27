using System.Windows;
using System.IO;
namespace obj2mdl_batch_converter
{
    // (C) 2024 stan0033
    public partial class MainWindow : Window
    {
        public MainWindow()
        { InitializeComponent(); }
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