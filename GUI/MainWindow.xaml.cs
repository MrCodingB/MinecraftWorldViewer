using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Minecraft;
using SixLabors.ImageSharp;

namespace GUI;

public partial class MainWindow
{
    private string? RegionFolder { get; set; }

    private string? OutputFolder { get; set; }

    public MainWindow()
    {
        InitializeComponent();

        ProgressManager.Progress += Progress;
    }

    private static string? OpenFolderSelect()
    {
        var dlg = new CommonOpenFileDialog
        {
            Title = "Select regions folder",
            IsFolderPicker = true,
            AddToMostRecentlyUsedList = false,
            AllowNonFileSystemItems = false,
            EnsureFileExists = true,
            EnsurePathExists = true,
            EnsureReadOnly = false,
            EnsureValidNames = true,
            Multiselect = false,
            ShowPlacesList = true
        };

        return dlg.ShowDialog() == CommonFileDialogResult.Ok
            ? dlg.FileName
            : null;
    }

    private void SelectRegionFolderButton_OnClick(object sender, RoutedEventArgs e)
    {
        var folder = OpenFolderSelect();

        if (folder is null)
        {
            return;
        }

        RegionFolder = folder;
        RegionFolderTextBox.Text = folder;
        OutputFolder ??= folder;
        OutputFolderTextBox.Text = OutputFolder;
        UpdateGenerateButtonVisibility();
    }

    private void SelectOutputFolderButton_OnClick(object sender, RoutedEventArgs e)
    {
        var folder = OpenFolderSelect();

        if (folder is null)
        {
            return;
        }

        OutputFolder = folder;
        OutputFolderTextBox.Text = folder;
        UpdateGenerateButtonVisibility();
    }

    private void CreateMapButton_OnClick(object sender, RoutedEventArgs e)
    {
        CreateMapButton.IsEnabled = false;
        CreateMapButton.Opacity = 0.5;

        ProgressManager.Reset();

        Task.Factory.StartNew(GenerateMap);
    }

    private void GenerateMap()
    {
        if (RegionFolder is not null && OutputFolder is not null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var generator = new MapGenerator(RegionFolder);

            var map = Profiler.MeasureExecutionDuration("MapGenerator.Generate", () => generator.Generate());

            map.SaveAsPng(Path.Combine(OutputFolder, $"Map-{DateTime.Now.ToFileTime()}.png"));

            stopwatch.Stop();
            MessageBox.Show($"Map generation complete - {stopwatch.Elapsed}", "Generation completed");

            ProgressManager.Reset();
        }
        else
        {
            UpdateGenerateButtonVisibility();
        }

        Dispatcher.Invoke(() =>
        {
            CreateMapButton.IsEnabled = true;
            CreateMapButton.Opacity = 1;
            MapGenerationRegionProgressBar.Maximum = 100;
            MapGenerationChunkProgressBar.Maximum = 100;
        });
    }

    private void Progress(object? _, ProgressEventArgs args)
    {
        Dispatcher.Invoke(() =>
        {
            MapGenerationRegionProgressBar.Maximum = args.TotalRegions;
            MapGenerationChunkProgressBar.Maximum = args.TotalChunks;

            MapGenerationRegionProgressBar.Value = args.CompletedRegions;
            MapGenerationChunkProgressBar.Value = args.CompletedChunks;

            MapGenerationRegionLabel.Content = $"Regions: {args.CompletedRegions}/{args.TotalRegions}";
            MapGenerationChunkLabel.Content = $"Chunks: {args.CompletedChunks}/{args.TotalChunks}";
        });
    }

    private void UpdateGenerateButtonVisibility()
    {
        Dispatcher.Invoke(() =>
        {
            CreateMapButton.Visibility = OutputFolder is not null && RegionFolder is not null
                ? Visibility.Visible
                : Visibility.Hidden;
        });
    }
}
