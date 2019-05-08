using System.IO;

namespace UsbKeyManager.Model
{
    /// <summary>
    /// Removable driver (USB).
    /// </summary>
    class Driver
    {
        public const string KeyFileName = "usb.key";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of driver.</param>
        /// <param name="path">Path to driver.</param>
        public Driver(string name, string path)
        {
            Name = name;
            Path = path;

            FullPath = System.IO.Path.Combine(path, KeyFileName);
        }

        /// <summary>
        /// Full path to file.
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// Name of driver.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Path to driver.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Determines whether the key is in the driver.
        /// </summary>
        public bool ContainsKeyFile { get; private set; }

        /// <summary>
        /// Remove key from the driver.
        /// </summary>
        public void RemoveKey()
        {
            File.Delete(FullPath);

            ContainsKeyFile = false;
        }

        /// <summary>
        /// Create new key in the driver.
        /// </summary>
        /// <param name="key">Key.</param>
        public void CreateKey(byte[] key)
        {
            File.WriteAllBytes(FullPath, key);

            ContainsKeyFile = true;
        }

        /// <summary>
        /// Load info from driver.
        /// </summary>
        public void LoadInfo()
        {
            ContainsKeyFile = File.Exists(FullPath);
        }
    }
}
