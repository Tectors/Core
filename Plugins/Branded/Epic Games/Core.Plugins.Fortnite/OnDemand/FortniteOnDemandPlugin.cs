using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Core.Plugins.Interfaces;
using Core.Plugins.OnDemand;
using Core.Resources.Framework.Base;
using CUE4Parse.Compression;
using CUE4Parse.UE4.IO;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;
using EpicManifestParser;
using UE4Config.Parsing;

namespace Core.Plugins.Fortnite.OnDemand;

public sealed class FortniteOnDemandPlugin : IOnDemandPlugin, IGameIdPlugin
{
    public string Name => "Fortnite On Demand";
    public string GameId => "Fortnite";
    
    public bool DoesCharacteristicsMatch(BaseProfile Profile)
    {
        return false;
        return !string.IsNullOrEmpty(Profile.ArchiveDirectory) && Profile.ArchiveDirectory.Contains("Fortnite", StringComparison.OrdinalIgnoreCase) && Regex.IsMatch(Profile.Name, @"^\d+(\.\d+){0,2}$");
    }
    
    public Task SetupProvider(BaseProfile Profile)
    {
        Profile.Provider = 
            /* Latest On Demand */
            Profile.IsAutoDetected ? new BaseOnDemandProvider(Profile.ArchiveDirectory, [
                new DirectoryInfo
                    (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "FortniteGame", "Saved", "PersistentDownloadDir", "GameCustom", "InstalledBundles")
                    )
            ], new VersionContainer(Profile.Version))

            /* Latest Installed */
            : new BaseOnDemandProvider(Profile.ArchiveDirectory, new VersionContainer(Profile.Version, Profile.TexturePlatform));

        Profile.Provider.OnDemandOptions = new IoStoreOnDemandOptions
        {
            ChunkHostUri = new Uri("https://download.epicgames.com/", UriKind.Absolute),
            ChunkCacheDirectory = Resources.Globals.OnDemandChunksFolder,
            Authorization = new AuthenticationHeaderValue("Bearer", EpicGames.Globals.EpicAuth?.Token),
            Timeout = TimeSpan.FromSeconds(60000)
        };
        
        return Task.CompletedTask;
    }
    
    public async Task PreInitialize(BaseProfile Profile)
    {
        await EpicGames.Globals.API.VerifyAuthAsync();
    }

    public async Task Initialize(BaseProfile Profile)
    {
        var manifestInfo = await EpicGames.Globals.API.GetManifestInfoAsync();
        if (manifestInfo is null) return;

        var options = new ManifestParseOptions
        {
            ChunkBaseUrl = "http://download.epicgames.com/Builds/Fortnite/CloudDir/",
            ChunkCacheDirectory = Resources.Globals.OnDemandChunksFolder.FullName,
            ManifestCacheDirectory = Resources.Globals.OnDemandManifestFolder.FullName,
            Decompressor = ManifestZlibStreamDecompressor.Decompress,
            DecompressorState = ZlibHelper.Instance,
            CacheChunksAsIs = true
        };
        
        var provider = Profile.Provider as BaseOnDemandProvider;
                
        var (manifest, _) = await manifestInfo.DownloadAndParseAsync(options);
        provider!.LiveManifest = manifest;

        provider!.RegisterFiles(manifest);
    }
    
    public async Task InitializeStreaming(BaseProfile Profile)
    {
        var tocPath = await GetTocPath(Profile);
        if (string.IsNullOrEmpty(tocPath)) return;
            
        var tocName = tocPath.SubstringAfterLast("/");
        var onDemandFile = new FileInfo(Path.Combine(Resources.Globals.OnDemandFolder.FullName, tocName));
        if (!onDemandFile.Exists || onDemandFile.Length == 0)
        {
            await API.Globals.API.DownloadFileAsync($"https://download.epicgames.com/{tocPath}", onDemandFile.FullName);
        }
            
        await Profile.Provider.RegisterVfsAsync(new IoChunkToc(onDemandFile.FullName, Profile.Provider.Versions));
        await Profile.Provider.MountAsync();
    }
    
    private async Task<string> GetTocPath(BaseProfile Profile)
    {
        var onDemandText = string.Empty;
        var provider = Profile.Provider as BaseOnDemandProvider;

        if (Profile.IsAutoDetected)
        {
            var onDemandFile = provider!.LiveManifest?.Files.FirstOrDefault(x => x.FileName.Equals("Cloud/IoStoreOnDemand.ini", StringComparison.OrdinalIgnoreCase));
            if (onDemandFile is not null)
            {
                using var reader = new StreamReader(onDemandFile.GetStream(), System.Text.Encoding.UTF8, true);
                onDemandText = reader.ReadToEnd();
            }
        }
        else
        {
            var onDemandPath = Path.Combine(Profile.ArchiveDirectory, @"..\..\..\Cloud\IoStoreOnDemand.ini");
            if (File.Exists(onDemandPath)) onDemandText = await File.ReadAllTextAsync(onDemandPath);
        }

        if (string.IsNullOrEmpty(onDemandText)) return string.Empty;

        var onDemandIni = new ConfigIni();
        onDemandIni.Read(new StringReader(onDemandText));
        return onDemandIni
            .Sections.FirstOrDefault(section => section.Name?.Equals("Endpoint") ?? false)?
            .Tokens.OfType<InstructionToken>().FirstOrDefault(token => token.Key.Equals("TocPath"))?
            .Value.Replace("\"", string.Empty) ?? string.Empty;
    }
}