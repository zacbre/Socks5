using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace socks5.Plugin
{
    class PluginLoader
    {
        public static bool LoadPluginsFromDisk { get; set; }
        public static List<object> LoadPlugin(Type assemblytype)
        {
            List<object> types = new List<object>();
            try
            {
                foreach (Type f in Assembly.GetExecutingAssembly().GetTypes())
                {
                    try
                    {
                        if (assemblytype.IsAssignableFrom(f) && f != assemblytype)
                        {
                            //Load the module.
                            //Console.WriteLine("Loaded type {0}.", f.ToString());
                            object type = Activator.CreateInstance(f);
                            types.Push(type);
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                }
                //load plugins from disk?
                if (LoadPluginsFromDisk)
                {
                    string PluginPath = Path.Combine(Environment.CurrentDirectory, "Plugins");
                    if (!Directory.Exists(PluginPath)) { Directory.CreateDirectory(PluginPath); }
                    foreach (string filename in Directory.GetFiles(PluginPath))
                    {
                        if (filename.EndsWith(".dll"))
                        {
                            //Initialize unpacker.
                            Assembly g = Assembly.Load(File.ReadAllBytes(filename));
                            //Test to see if it's a module.
                            if (g != null)
                            {
                                foreach (Type f in g.GetTypes())
                                {
                                    if (assemblytype.IsAssignableFrom(f) && f != assemblytype)
                                    {
                                        object plug = Activator.CreateInstance(f);
                                        types.Push(plug);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (Exception p in e.LoaderExceptions)
                    Console.WriteLine(p.ToString());
            }
            return types;
        }
    }
}
