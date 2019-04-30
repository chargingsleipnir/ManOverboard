using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions to help with IO stuff
    /// </summary>
    public static class FileSystemExtensions
    {

        /// <summary>
        /// Retrieves all of the files within the specified folder, with the option to descend
        /// into the subfolders
        /// </summary>
        /// <param name="Folder">The folder to retrieve the files from</param>
        /// <param name="SearchSubFolders">True to search subdirectories, false if not... note that this goes
        /// all the way through the folder tree, not just to the next level</param>
        /// <returns>A collection of filepaths found in the folder, or an empty collection if none were found</returns>
        public static IEnumerable<string> GetAllFiles(string Folder, bool SearchSubFolders = false)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(Folder);

            while (queue.Count > 0)
            {
                string path = queue.Dequeue();

                try
                {
                    if (SearchSubFolders)
                    {
                        foreach (string subDir in Directory.GetDirectories(path))
                        {
                            queue.Enqueue(subDir);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }

                string[] files = null;

                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.Message);
                }

                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i].Replace('\\', '/');
                    }
                }
            }
        }

        /// <summary>
        /// Converts the path to be relative to the provided source directory
        /// 
        /// Taken from https://iandevlin.com/blog/2010/01/csharp/generating-a-relative-path-in-csharp/
        /// </summary>
        /// <param name="absPath">The absolute path to be converted</param>
        /// <param name="relTo">The path to make the absolute path relative to</param>
        /// <returns>The relative path</returns>
        public static string GetRelativePath(string absPath, string relTo)
        {
            absPath = absPath.Replace("\\", "/");
            relTo = relTo.Replace("\\", "/");

            string[] absDirs = absPath.Split('/');
            string[] relDirs = relTo.Split('/');

            // Get the shortest of the two paths
            int len = absDirs.Length < relDirs.Length ? absDirs.Length :
            relDirs.Length;

            // Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            // Find common root
            for (index = 0; index < len; index++)
            {
                if (absDirs[index] == relDirs[index]) lastCommonRoot = index;
                else break;
            }

            // If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
            {
                throw new ArgumentException("Paths do not have a common base");
            }

            // Build up the relative path
            StringBuilder relativePath = new StringBuilder();

            // Add on the ..
            for (index = lastCommonRoot + 1; index < absDirs.Length; index++)
            {
                if (absDirs[index].Length > 0) relativePath.Append("..\\");
            }

            // Add on the folders
            for (index = lastCommonRoot + 1; index < relDirs.Length - 1; index++)
            {
                relativePath.Append(relDirs[index] + Path.DirectorySeparatorChar);
            }
            relativePath.Append(relDirs[relDirs.Length - 1]);

            return relativePath.ToString().
                Replace('\\', Path.DirectorySeparatorChar).
                Replace('/', Path.DirectorySeparatorChar);
        }
    }
}