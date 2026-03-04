using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using vj0.Models.Profiles;

namespace vj0.Models;

public static class SyncToFModel
{
    private static readonly string ApplicationDataFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

    private static readonly string AppSettingsPath = Path.Combine(ApplicationDataFolder, "FModel", "AppSettings.json");
    private static readonly string AppSettingsDebugPath = Path.Combine(ApplicationDataFolder, "FModel", "AppSettings_Debug.json");

    private static JsonNode? Node;
    public static List<Profile> LoadedProfiles = [];
    
    private static DateTime _lastLoadTime = DateTime.MinValue;

    private static async Task Load()
    {
        if ((DateTime.UtcNow - _lastLoadTime).TotalSeconds < 1)
        {
            return;
        }

        _lastLoadTime = DateTime.UtcNow;
        
        if (!File.Exists(AppSettingsPath))
        {
            return;
        }
        
        var json = await File.ReadAllTextAsync(AppSettingsPath);

        Node = JsonNode.Parse(json);
        LoadedProfiles = await Profile.LoadAllAsync();
   
        LoadedProfiles = Profile.SortProfiles(LoadedProfiles);
    }
    
    public static Task Wipe()
    {
        if (Node is null) return Task.CompletedTask;
        
        if (Node["PerDirectory"] is JsonObject perDir)
        {
            perDir.Clear();
        }
        else
        {
            Node["PerDirectory"] = new JsonObject();
        }

        return Task.CompletedTask;
    }
    
    public static async Task Save()
    {
        await Load();
        
        if (Node is null)
        {
            return;
        }
        
        await Wipe();
        
        /* Save Profiles ~~~~~~~~~~~~~~~~~> */
        var perDirectory = Node["PerDirectory"] as JsonObject ?? new JsonObject();

        foreach (var profile in LoadedProfiles)
        {
            if (profile.HasErrors || profile.IsAutoDetected) continue;
            
            var keys = new JsonArray();

            foreach (var key in profile.Encryption.Keys)
            {
                keys.Add(new JsonObject
                {
                    ["guid"] = key.Guid,
                    ["key"] = key.Key,
                });
            }

            var archiveDirectory = profile.ArchiveDirectory.Replace("/", "\\").TrimEnd('\\');
            
            perDirectory[archiveDirectory] = new JsonObject
            {
                ["GameName"] = profile.Name,
                ["GameDirectory"] = archiveDirectory,
                
                ["IsManual"] = true,
                ["UeVersion"] = profile.Version.ToString(),
                
                ["Versioning"] = new JsonObject
                {
                    ["CustomVersions"] = null,
                    ["Options"] = null,
                    ["MapStructTypes"] = null
                },
                
                ["Directories"] = new JsonArray(),
                
                ["Endpoints"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["Url"] = null,
                        ["Path"] = null,
                        ["Overwrite"] = false,
                        ["FilePath"] = null,
                        ["IsValid"] = false,
                    },
                    new JsonObject
                    {
                        ["Url"] = null,
                        ["Path"] = null,
                        ["Overwrite"] = profile.MappingsContainer.Override,
                        ["FilePath"] = profile.MappingsContainer.Override ? profile.MappingsContainer.Path : null,
                        ["IsValid"] = profile.MappingsContainer.Override
                    }
                },
                
                ["AesKeys"] = new JsonObject
                {
                    ["mainKey"] = profile.Encryption.MainKey,
                    ["dynamicKeys"] = keys
                }
            };
        }

        if (MainWM.CurrentProfile is not null)
        {
            var archiveDirectory = MainWM.CurrentProfile.ArchiveDirectory.Replace("/", "\\").TrimEnd('\\');

            Node["GameDirectory"] = archiveDirectory;
        }

        /* Save Profiles <~~~~~~~~~~~~~~~~~ */

        await SaveFile();
    }

    private static async Task SaveFile()
    {
        if (Node is null)
        {
            return;
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var output = Node.ToJsonString(options);

        await File.WriteAllTextAsync(AppSettingsPath, output);
        await File.WriteAllTextAsync(AppSettingsDebugPath, output);
    }
}