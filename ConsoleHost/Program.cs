using System.Diagnostics;
using ConsoleHost;
using Minecraft;
using SixLabors.ImageSharp;
using Utility;

var parsedArgs = new Dictionary<string, string>();

var i = 0;
while (i < args.Length)
{
    if (!args[i].StartsWith("-"))
    {
        i++;
        continue;
    }

    var name = args[i][1..];
    parsedArgs[name] = args[i + 1];
    i += 2;
}

var inputFolder = parsedArgs.GetValueOrDefault("i") ?? parsedArgs.GetValueOrDefault("input");
var outputFolder = parsedArgs.GetValueOrDefault("o") ?? parsedArgs.GetValueOrDefault("output") ?? inputFolder;

if (inputFolder is null)
{
    Console.Write("Regions path:");
    inputFolder = Console.ReadLine();

    if (inputFolder is null)
    {
        Console.WriteLine("No input folder specified. Use parameters -i or -input");
        return;
    }

    Console.WriteLine($"> {inputFolder}");
}

if (outputFolder is null)
{
    Console.Write("Output path (defaults to regions path):");
    outputFolder = Console.ReadLine() ?? inputFolder;
    Console.WriteLine($"> {outputFolder}");
}

Console.WriteLine("Loading");

var generator = new MapGenerator(inputFolder);

ProgressManager.Progress += ProgressReporter.ReportProgress;

Console.WriteLine("Generating");

var stopwatch = Stopwatch.StartNew();

var image = generator.Generate();

stopwatch.Stop();

Console.WriteLine("Generation complete");
Console.WriteLine("Total time:      " + stopwatch.Elapsed);
Console.WriteLine("Time per region: " + ProgressManager.TimePerRegion);
Console.WriteLine("Time per chunk:  " + ProgressManager.TimePerChunk);
Console.WriteLine("Avg memory usage [MB]: {0:F5}", ProgressManager.AvgMemoryInMb);
Console.WriteLine("Max memory usage [MB]: {0:F5}", ProgressManager.MaxMemory);

_ = bool.TryParse(parsedArgs.GetValueOrDefault("dry-run") ?? "false", out var dryRun);

if (!dryRun)
{
    var regionName = new DirectoryInfo(inputFolder).Name;
    var imagePath = Path.Combine(outputFolder, MapFileName.Get(regionName));

    Console.WriteLine($"Saving image at {imagePath}");

    image.SaveAsPng(imagePath);
}

Console.WriteLine("Completed");
