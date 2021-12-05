using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace Scripts.BundleHashCalculator
{
    public enum NameDeveloper
    {
        None,
        Developer1,
        Developer2,
        Developer3
    }

    [CreateAssetMenu(fileName = "BundleHashCalculatorConfig", menuName = "Bundle/Hash calculator")]
    public sealed class BundleHashCalculator : ScriptableObject
    {
        public List<InfoDeveloper> Developers;
        public NameDeveloper SelectedDeveloperPreset;
        public string NameBundle;
        public string HashAndroid;
        public string HashIOS;
        public string HashWindows;

        [ContextMenu("Apply All")]
        public bool ApplyAll()
        {
            try
            {
                return CalculateHash() && CopyFileToFolderByPresetDeveloper();
            }
            catch (Exception e)
            {

                return false;
            }
        }

        [ContextMenu("Calculate Hash")]
        public bool CalculateHash()
        {
            try
            {
                var tempPathFileUnity3d = GetPathFileUnity3d(RuntimePlatform.Android, NameBundle.ToLower());
                HashAndroid = ComputeMD5Checksum(tempPathFileUnity3d);

                tempPathFileUnity3d = GetPathFileUnity3d(RuntimePlatform.IPhonePlayer, NameBundle.ToLower());
                HashIOS = ComputeMD5Checksum(tempPathFileUnity3d);

                tempPathFileUnity3d = GetPathFileUnity3d(RuntimePlatform.WindowsEditor, NameBundle.ToLower());
                HashWindows = ComputeMD5Checksum(tempPathFileUnity3d);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        public bool CheckPresetDeveloper()
        {
            var presetDeveloper = Developers.Find(developer => developer.NameDeveloper == SelectedDeveloperPreset);
            return presetDeveloper != null;
        }

        public bool CheckPresetPathToGitFolder()
        {
            var presetDeveloper = Developers.Find(developer => developer.NameDeveloper == SelectedDeveloperPreset);
            return presetDeveloper != null && !string.IsNullOrEmpty(presetDeveloper.PathToGitPublicAssetBundle);
        }

        [ContextMenu("Copy File")]
        public bool CopyFileToFolderByPresetDeveloper()
        {
            try
            {
                var presetDeveloper = Developers.Find(developer => developer.NameDeveloper == SelectedDeveloperPreset);

                if (!CheckPresetDeveloper())
                {
                    Debug.LogError("Not find preset developer with name: " + SelectedDeveloperPreset.ToString());
                    return false;
                }

                if (!CheckPresetPathToGitFolder())
                {
                    Debug.LogError("Not set path to folder with git bundles, please check preset developer with name: " + SelectedDeveloperPreset.ToString());
                    return false;
                }

                CopyFilesToFolder(presetDeveloper.PathToGitPublicAssetBundle, RuntimePlatform.Android);
                CopyFilesToFolder(presetDeveloper.PathToGitPublicAssetBundle, RuntimePlatform.IPhonePlayer);
                CopyFilesToFolder(presetDeveloper.PathToGitPublicAssetBundle, RuntimePlatform.WindowsEditor);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void CopyFilesToFolder(string pathToFolder, RuntimePlatform platform)
        {
            var tempPathFileUnity3d = GetPathFileUnity3d(RuntimePlatform.Android, NameBundle.ToLower());
            string destFile = System.IO.Path.Combine(pathToFolder, (GetPathBundle(platform) + NameBundle).ToLower());

            System.IO.Directory.CreateDirectory(pathToFolder);
            System.IO.File.Copy(tempPathFileUnity3d, destFile, true);

            tempPathFileUnity3d = GetPathFileUnity3d(RuntimePlatform.Android, NameBundle.ToLower() + ".manifest");
            destFile = System.IO.Path.Combine(pathToFolder, (GetPathBundle(platform) + NameBundle).ToLower() + ".manifest");

            System.IO.Directory.CreateDirectory(pathToFolder);
            System.IO.File.Copy(tempPathFileUnity3d, destFile, true);
        }

        private string ComputeMD5Checksum(string path)
        {
            try
            {
                using (FileStream fs = System.IO.File.OpenRead(path))
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] fileData = new byte[fs.Length];
                    fs.Read(fileData, 0, (int)fs.Length);
                    byte[] checkSum = md5.ComputeHash(fileData);
                    string result = BitConverter.ToString(checkSum).Replace(" - ", String.Empty);
                    return result;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return "";
            }
        }

        private string GetPathBundle(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android\\";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS\\";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                    return "Windows\\";
                default: throw new Exception($"Domain.name for {Application.platform} is not found");
            }
        }

        private string GetPathFileUnity3d(RuntimePlatform pltafrom, string bundleName) =>
            Path.Combine(@"C:\Users\" + Environment.UserName + @"\Desktop\AssetBundles\", GetPathBundle(pltafrom) + bundleName);
    }

    [Serializable]
    public sealed class InfoDeveloper
    {
        public NameDeveloper NameDeveloper;
        public string PathToGitPublicAssetBundle;
    }
}
