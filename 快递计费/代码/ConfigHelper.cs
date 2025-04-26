using System;
using System.Configuration;

/// <summary>
/// 管理应用程序配置的辅助类（包括 appsettings、connectionStrings 和自定义部分）。
/// </summary>
internal static class ConfigHelper
{
    // 静态配置对象
    private static readonly Configuration configuration;

    // 配置文件映射
    private static ExeConfigurationFileMap execonfig = new ExeConfigurationFileMap();

    /// <summary>
    /// 静态构造函数 - 初始化配置文件路径。
    /// </summary>
    static ConfigHelper()
    {
        //execonfig.ExeConfigFilename = @"APP.config"; // 设置配置文件的相对路径
        configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    }

    /// <summary>
    /// 获取 connectionStrings 配置节中的连接字符串。
    /// </summary>
    /// <param name="name">连接字符串名称</param>
    /// <returns>连接字符串</returns>
    public static string GetConnectionString(string name)
    {
        return configuration.ConnectionStrings.ConnectionStrings[name]?.ConnectionString;
    }

    /// <summary>
    /// 获取自定义配置节的值。
    /// </summary>
    /// <param name="sectionName">配置节名称</param>
    /// <returns>配置节的值</returns>
    public static object GetCustomSection(string sectionName)
    {
        return configuration.GetSection(sectionName);
    }

    /// <summary>
    /// 设置 connectionStrings 配置节中的连接字符串。
    /// </summary>
    /// <param name="name">连接字符串名称</param>
    /// <param name="connectionString">连接字符串</param>
    public static void SetConnectionString(string name, string connectionString)
    {
        if (configuration.ConnectionStrings.ConnectionStrings[name] != null)
        {
            configuration.ConnectionStrings.ConnectionStrings[name].ConnectionString = connectionString;
        }
        else
        {
            var cs = new ConnectionStringSettings(name, connectionString);
            configuration.ConnectionStrings.ConnectionStrings.Add(cs);
        }
        configuration.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("connectionStrings");
    }

    /// <summary>
    /// AppSettings的相关操作
    /// </summary>
    public static class Appsettings
    {
        /// <summary>
        /// AppSettings键的数量
        /// </summary>
        public static int KeyCount = 0;

        // 存储 AppSettings 键值对的集合
        private static readonly KeyValueConfigurationCollection KeyValues;

        /// <summary>
        /// APPsettings的节点
        /// </summary>
        private static AppSettingsSection appSettingsSection;

        static Appsettings()
        {
            appSettingsSection = configuration.AppSettings;
            KeyCount = appSettingsSection.Settings.Count;
            KeyValues = appSettingsSection.Settings;
        }

        /// <summary>
        /// 增加新的键值对可以直接向 NameValueCollection 中添加新项，并确保将其保存到配置文件中
        /// </summary>
        /// <param name="key">增加的键名</param>
        /// <param name="value">增加的值</param>
        public static void AddAppSetting(string key, string value)
        {
            if (KeyValues[key] == null)
            {
                configuration.AppSettings.Settings.Add(key, value);
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                KeyCount += 1;
            }
        }

        /// <summary>
        /// 获取AppSettings的所有键
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllkeys()
        {
            try
            {
                if (KeyCount == 0)
                {
                    return new string[] { };
                }
                string[] str = new string[KeyCount];
                int index = 0;
                foreach (KeyValueConfigurationElement item in KeyValues)
                {
                    str[index] = item.Key;
                    index++;
                }
                return str;
            }
            catch (Exception ex)
            {
                // 记录异常信息，方便调试
                Console.WriteLine($"Error getting all keys: {ex.Message}");
                return new string[] { };
            }
        }

        /// <summary>
        /// 获取AppSettings的所有值
        /// </summary>
        /// <returns>返回一个string数组</returns>
        public static string[] GetAllValues()
        {
            try
            {
                if (KeyCount == 0)
                {
                    return new string[] { };
                }
                string[] str = new string[KeyCount];
                int index = 0;
                foreach (KeyValueConfigurationElement item in KeyValues)
                {
                    str[index] = item.Value;
                    index++;
                }
                return str;
            }
            catch (Exception ex)
            {
                // 记录异常信息，方便调试
                Console.WriteLine($"Error getting all values: {ex.Message}");
                return new string[] { };
            }
        }

        /// <summary>
        /// 获取键值对的值
        /// </summary>
        /// <param name="key">需要查找的键名</param>
        /// <returns>返回键名对应的值</returns>
        public static string GetValue(string key)
        {
            try
            {
                return KeyValues[key]?.Value;
            }
            catch (Exception ex)
            {
                // 记录异常信息，方便调试
                Console.WriteLine($"Error getting value for key {key}: {ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// 移出删除某个键值对
        /// </summary>
        /// <param name="key">需要移出的键名</param>
        public static void RemoveAppSetting(string key)
        {
            if (configuration.AppSettings.Settings[key] != null)
            {
                configuration.AppSettings.Settings.Remove(key);
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                KeyCount -= 1;
            }
        }

        /// <summary>
        /// 修改现有的键值对
        /// </summary>
        /// <param name="key">要修改的键名</param>
        /// <param name="value">要修改的值</param>
        public static void UpdateAppSetting(string key, string value)
        {
            if (configuration.AppSettings.Settings[key] != null)
            {
                configuration.AppSettings.Settings[key].Value = value;
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }
    }

    /// <summary>
    /// ConnectionStrings 的相关操作
    /// </summary>
    public static class ConnectionStrings
    {
        // connetionstring的配置节对象
        private static ConnectionStringsSection ConnectionStringsSection;

        // 连接字符串设置集合
        private static ConnectionStringSettingsCollection connectionStringSettings;

        static ConnectionStrings()
        {
            ConnectionStringsSection = configuration.ConnectionStrings;
            connectionStringSettings = ConnectionStringsSection.ConnectionStrings;
        }

        /// <summary>
        /// 添加新的连接字符串
        /// </summary>
        /// <param name="name">连接字符串名称</param>
        /// <param name="value">连接字符串值</param>
        /// <param name="providername">数据提供程序名称</param>
        public static void Add(string name, string value, string providername)
        {
            if (connectionStringSettings[name] == null)
            {
                ConnectionStringSettings connectionStringsSection = new ConnectionStringSettings(name, value, providername);
                connectionStringSettings.Add(connectionStringsSection);
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
        }

        /// <summary>
        /// 更新现有的连接字符串
        /// </summary>
        /// <param name="name">连接字符串名称</param>
        /// <param name="value">连接字符串值</param>
        public static void Update(string name, string value)
        {
            if (connectionStringSettings[name] != null)
            {
                connectionStringSettings[name].ConnectionString = value;
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
        }

        /// <summary>
        /// 获取指定名称的连接字符串设置
        /// </summary>
        /// <param name="name">连接字符串名称</param>
        /// <returns>连接字符串设置，如果不存在则返回 null</returns>
        public static string Get(string name)
        {
            return connectionStringSettings[name].ToString();
        }

        /// <summary>
        /// 删除指定名称的连接字符串
        /// </summary>
        /// <param name="name">连接字符串名称</param>
        public static void Remove(string name)
        {
            if (connectionStringSettings[name] != null)
            {
                connectionStringSettings.Remove(name);
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
        }
    }
}