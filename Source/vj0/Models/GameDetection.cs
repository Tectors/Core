using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using vj0.Models.Profiles;
using vj0.Plugins.Interfaces;
using vj0.Services.Framework;
using vj0.Core.Framework.Base;

namespace vj0.Models;

public static class GameDetection
{
    public static List<Profile> LoadedProfiles = [];
    
    public static List<Profile> GetRecentlyUsedProfiles(int count)
    {
        return LoadedProfiles
            .Where(p => p.Display.LastUsed.HasValue)
            .OrderByDescending(p => p.Display.LastUsed!.Value)
            .Take(count)
            .ToList();
    }

    public static async Task LoadAllAsync()
    {
        LoadedProfiles = await Profile.LoadAllAsync();
    }
    
    public static void DetectAllProfilesAsync(Action<Profile>? onDetected = null)
    {
        var profiles = new List<BaseProfile>();

        Action<BaseProfile>? wrappedCallback = onDetected == null ? null : bp => { if (bp is Profile p) onDetected(p); };

        foreach (var Plugin in AppServices.Plugins.List)
        {
            if (Plugin is not IGameDetectionPlugin detectionPlugin) continue;
            detectionPlugin.Detect(profiles, wrappedCallback);
        }

        var profileList = profiles.Select(bp => new Profile
        {
            Name = bp.Name,
            ArchiveDirectory = bp.ArchiveDirectory,
            Version = bp.Version,
            AutoDetectedGameId = bp.AutoDetectedGameId
        }).ToList();
        
        foreach (var profile in profileList)
        {
            var existingProfile = LoadedProfiles.FirstOrDefault(p => p.AutoDetectedGameId == profile.AutoDetectedGameId);
            if (existingProfile is not null && existingProfile.SchemaVersion != -1)
            {
                continue;
            }
            
            _ = profile.Save(false);
            LoadedProfiles.Add(profile);

            onDetected?.Invoke(profile);
        }
    }

    public static void PostDetection()
    {
        foreach (var profile in LoadedProfiles)
        {
            profile.UpdateSchemaVersion();
            
            _ = profile.Save();
        }
    }
}
