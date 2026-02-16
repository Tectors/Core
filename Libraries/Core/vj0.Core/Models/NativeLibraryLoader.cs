using System.IO;
using System.Threading.Tasks;

using CUE4Parse_Conversion.Textures;
using CUE4Parse_Conversion.Textures.BC;
using CUE4Parse.Compression;

namespace vj0.Core.Models;

public abstract class NativeLibraryLoader
{
    public static async Task Load(DirectoryInfo RuntimeFolder)
    {
        Directory.CreateDirectory(RuntimeFolder.ToString());

        var oodlePath = Path.Combine(RuntimeFolder.FullName, OodleHelper.OodleFileName);
        if (!File.Exists(oodlePath)) await OodleHelper.DownloadOodleDllAsync(ref oodlePath);
        OodleHelper.Initialize(oodlePath);
        
        var zlibPath = Path.Combine(RuntimeFolder.FullName, ZlibHelper.DLL_NAME);
        if (!File.Exists(zlibPath)) await ZlibHelper.DownloadDllAsync(zlibPath);
        ZlibHelper.Initialize(zlibPath);
        
        TextureDecoder.UseAssetRipperTextureDecoder = true;
        var detexPath = Path.Combine(RuntimeFolder.FullName, DetexHelper.DLL_NAME);
        if (!File.Exists(detexPath)) await DetexHelper.LoadDllAsync(detexPath);
        DetexHelper.Initialize(detexPath);
    }
}
