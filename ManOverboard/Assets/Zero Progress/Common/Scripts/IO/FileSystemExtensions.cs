using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZeroProgress.Common
{
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
    }
}