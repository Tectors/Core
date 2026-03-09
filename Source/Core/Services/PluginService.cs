using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Serilog;

using Core.Framework;
using Core.Plugins;

namespace Core.Services;

/* Loads Assemblies and stores Plugins */
public class PluginService : IService
{
    public List<IPlugin> List { get; } = [];
    
    public void Load()
    {
        LoadAssemblies();
        RegisterPlugins();
    }

    private readonly List<Assembly> PluginAssemblies = [];
    
    private void LoadAssemblies()
    {
        try
        {
            string pluginsPath = Path.Combine(AppContext.BaseDirectory, "Plugins");

            if (!Directory.Exists(pluginsPath))
                return;

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name?.StartsWith("Core.Plugins.") == true)
                .ToDictionary(a => a.GetName().Name!, StringComparer.OrdinalIgnoreCase);
            
            foreach (var pluginFile in Directory.GetFiles(pluginsPath, "Core.Plugins.*.dll"))
            {
                try
                {
                    string assemblyName = Path.GetFileNameWithoutExtension(pluginFile);

                    if (loadedAssemblies.ContainsKey(assemblyName))
                    {
                        PluginAssemblies.Add(loadedAssemblies[assemblyName]);
                        Log.Information($"Using already loaded plugin assembly: {assemblyName}");
                        continue;
                    }

                    var assembly = Assembly.LoadFrom(pluginFile);
                    PluginAssemblies.Add(assembly);

                    Log.Information($"Loaded plugin assembly: {assemblyName}");
                }
                catch (Exception ex)
                {
                    Log.Warning($"Failed to load plugin assembly {pluginFile}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warning($"Failed to load plugin assemblies: {ex.Message}");
        }
    }

    private void RegisterPlugins()
    {
        foreach (var assembly in PluginAssemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) &&
                                !t.IsAbstract &&
                                !t.IsInterface &&
                                t.GetConstructor(Type.EmptyTypes) != null);

                foreach (var plugin in types.Select(t => (IPlugin)Activator.CreateInstance(t)!))
                {
                    RegisterPlugin(plugin);
                    Log.Information($"Registered plugin: {plugin.Name}");
                }
            }
            catch (Exception ex)
            {
                Log.Warning($"Failed to scan assembly {assembly.GetName().Name}: {ex.Message}");
            }
        }

        Log.Information($"Registered {List.Count} plugins");
    }
    
    private void RegisterPlugin(IPlugin plugin)
    {
        List.Add(plugin);
    }
}
