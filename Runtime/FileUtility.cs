using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Utilities for manipulating files from Unity's Persistent Data Path.
/// </summary>
public static class FileUtility
{
    static string rootPath => Path.GetFullPath(Application.persistentDataPath); // GetFullPath eensures the path is normalized.

    /// <summary>
    ///  Writes the given file to the file system. File extension required!
    /// </summary>
    /// <param name="filePath">Relative path to the file. File extension required.</param>
    /// <param name="fileContents"></param>
    /// <returns></returns>
    public static bool WriteText(string filePath, string fileContents)
    {
        var fullPath = Path.Combine(rootPath, filePath);

        // Ensure the directory exists before writing the file
        try
        {
            var directory = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to ensure directory for path {fullPath} with exception {e}");
        }

        try
        {
            File.WriteAllText(fullPath, fileContents);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static async Task<bool> WriteTextAsync(string filePath, string fileContents)
    {
        var fullPath = Path.Combine(rootPath, filePath);

        // Ensure the directory exists before writing the file
        try
        {
            var directory = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to ensure directory for path {fullPath} with exception {e}");
        }

        try
        {
            await File.WriteAllTextAsync(fullPath, fileContents);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    /// <summary>
    /// Reads the given file from the file system. File extension required!
    /// </summary>
    /// <param name="filePath">Relative path to the file. File extension required.</param>
    /// <param name="fileContents"></param>
    /// <returns></returns>
    public static bool ReadText(string filePath, out string fileContents)
    {
        var fullPath = Path.Combine(rootPath, filePath);

        try
        {
            fileContents = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            fileContents = "";
            return false;
        }
    }

    public static async Task<(bool success, string fileContents)> ReadTextAsync(string filePath)
    {
        var fullPath = Path.Combine(rootPath, filePath);
        try
        {
            var fileContents = await File.ReadAllTextAsync(fullPath);
            return (true, fileContents);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            return (false, "");
        }
    }

    /// <summary>
    /// Checks if the given file exists the file system. File extension required!
    /// </summary>
    /// <param name="filePath">Relative path to the file. File extension required.</param>
    /// <returns></returns>
    public static bool Exists(string filePath)
    {
        var fullPath = Path.Combine(rootPath, filePath);

        try
        {
            return File.Exists(fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to check if {fullPath} exists with exception {e}");
            return false;
        }
    }

    /// <summary>
    /// Deletes the given file from the file system. File extension required!
    /// </summary>
    /// <param name="filePath">Relative path to the file. File extension required.</param>
    /// <returns></returns>
    public static void Delete(string filePath)
    {
        // Delete the file
        var fullPath = Path.Combine(rootPath, filePath);

        try
        {
            File.Delete(fullPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete {fullPath} with exception {e}");
        }
    }
}