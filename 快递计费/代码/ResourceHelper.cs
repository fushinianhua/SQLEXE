using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Reflection;

/// <summary>
/// 用于帮助获取程序集中资源信息的静态类。
/// </summary>
internal static class ResourceHelper
{
    /// <summary>
    /// 当前执行程序集的引用。
    /// </summary>
    public static Assembly assembly = Assembly.GetExecutingAssembly();

    /// <summary>
    /// 所有嵌入资源的名称。
    /// </summary>
    private static readonly string[] resourceNames = GetAllResourceNames();

    /// <summary>
    /// 获取当前程序集中所有嵌入资源的名称。
    /// </summary>
    /// <returns>包含所有嵌入资源名称的字符串数组，如果发生异常则返回空数组。</returns>
    public static string[] GetAllResourceNames()
    {
        try
        {
            return assembly.GetManifestResourceNames();
        }
        catch (Exception ex)
        {
            // 可以根据实际需求记录日志或进行其他处理
            Console.WriteLine($"获取资源名称时发生错误: {ex.Message}");
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// 根据资源名称获取图片资源。
    /// </summary>
    /// <param name="resourceName">资源的完整名称。</param>
    /// <returns>图片对象，如果发生异常或资源不存在则返回 null。</returns>
    public static Image GetImageResource(string resourceName)
    {
        if (string.IsNullOrEmpty(resourceName))
        {
            return null;
        }

        foreach (string name in resourceNames)
        {
            if (name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (Stream stream = assembly.GetManifestResourceStream(name))
                    {
                        if (stream != null)
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"获取图片资源时发生错误: {ex.Message}");
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 根据资源名称获取文本资源。
    /// </summary>
    /// <param name="resourceName">资源的完整名称。</param>
    /// <returns>文本内容，如果发生异常或资源不存在则返回 null。</returns>
    public static string GetTextResource(string resourceName)
    {
        if (string.IsNullOrEmpty(resourceName))
        {
            return null;
        }
        string[] resourceNames = GetAllResourceNames();
        foreach (string name in resourceNames)
        {
            if (name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (Stream stream = assembly.GetManifestResourceStream(name))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"获取文本资源时发生错误: {ex.Message}");
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 根据资源名称播放音频资源。
    /// </summary>
    /// <param name="resourceName">资源的完整名称。</param>
    public static void PlayAudioResource(string resourceName)
    {
        if (string.IsNullOrEmpty(resourceName))
        {
            return;
        }
        string[] resourceNames = GetAllResourceNames();
        foreach (string name in resourceNames)
        {
            if (name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (Stream stream = assembly.GetManifestResourceStream(name))
                    {
                        if (stream != null)
                        {
                            SoundPlayer player = new SoundPlayer(stream);
                            player.Play();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"播放音频资源时发生错误: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 根据资源名称获取二进制资源。
    /// </summary>
    /// <param name="resourceName">资源的完整名称。</param>
    /// <returns>二进制数据，如果发生异常或资源不存在则返回 null。</returns>
    public static byte[] GetBinaryResource(string resourceName)
    {
        if (string.IsNullOrEmpty(resourceName))
        {
            return null;
        }
        string[] resourceNames = GetAllResourceNames();
        foreach (string name in resourceNames)
        {
            if (name.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (Stream stream = assembly.GetManifestResourceStream(name))
                    {
                        if (stream != null)
                        {
                            byte[] buffer = new byte[stream.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            return buffer;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"获取二进制资源时发生错误: {ex.Message}");
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 根据图片名称查找并获取图片资源。
    /// </summary>
    /// <param name="imageName">图片的文件名，包含扩展名。</param>
    /// <returns>图片对象，如果未找到或发生异常则返回 null。</returns>
    public static Image FindImageByName(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
        {
            return null;
        }
        string[] resourceNames = GetAllResourceNames();
        foreach (string resourceName in resourceNames)
        {
            if (resourceName.EndsWith(imageName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"获取嵌入图片时出错: {ex.Message}");
                }
            }
        }
        return null;
    }
}