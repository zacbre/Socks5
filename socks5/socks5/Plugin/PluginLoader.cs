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
        //load plugin staticly.
        public static List<object> Plugins = new List<object>();
        private static void LoadPlugins()
        {
            try
            {
                foreach (Type f in Assembly.GetExecutingAssembly().GetTypes())
                {
                    try
                    {
                        if (!CheckType(f))
                        {
                            object type = Activator.CreateInstance(f);
                            Plugins.Push(type);
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                }
                foreach (Type f in Assembly.GetEntryAssembly().GetTypes())
                {
                    try
                    {
                        if (!CheckType(f))
                        {
                            //Console.WriteLine("Loaded type {0}.", f.ToString());
                            object type = Activator.CreateInstance(f);
                            Plugins.Push(type);
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
                                    if (!CheckType(f))
                                    {
                                        object plug = Activator.CreateInstance(f);
                                        Plugins.Push(plug);
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
            loaded = true;
        }
        static List<Type> pluginTypes = new List<Type>(){ typeof(LoginHandler), typeof(DataHandler), typeof(ConnectHandler)};
        private static bool CheckType(Type p)
        {
            foreach(Type x in pluginTypes)
            {
                if (x.IsAssignableFrom(p) && p != x)
                    continue;
                else
                    return true;
            }
            return false;
        }
        static bool loaded = false;
        public static List<object> LoadPlugin(Type assemblytype)
        {
            //make sure plugins are loaded.
            if (!loaded) LoadPlugins();
            List<object> list = new List<object>();
            foreach (object x in Plugins)
            {
                if (assemblytype.IsAssignableFrom(x.GetType()))
                {
                    list.Push(x);
                }
            }
            return list;
        }
    }
}
