using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Core
{
    /// <summary>
    /// Static helper class for all RecipeBox.
    /// </summary>
    public static class CoreUtility
    {
        /// <summary>
        /// Crop to create a square image.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static Image CropToSquareImage(Image image)
        {
            if (image.Height == image.Width) { return image; }

            Rectangle rectangle;
            if (image.Height > image.Width)
            {
                rectangle = new Rectangle(new Point(0, Convert.ToInt32((image.Height - image.Width) / 2)), new Size(image.Width, image.Width));
            }
            else
            {
                rectangle = new Rectangle(new Point(Convert.ToInt32((image.Width - image.Height) / 2), 0), new Size(image.Height, image.Height));
            }
            Bitmap bitmap = new Bitmap(image);
            image = bitmap.Clone(rectangle, image.PixelFormat);
            return image;
        }

        /// <summary>
        /// Method which one-way encrypts and returns the input password string using the input salt value.
        /// </summary>
        /// <param name="password">Password to encrypt.</param>
        /// <param name="salt">Salt added to the password when hashing.</param>
        /// <returns>Hashed and salted password.</returns>
        public static string EncryptPassword(
            string password,
            string salt)
        {
            // Use PBKDF2 to hash and salt the password
            using (var cryptoProvider = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 2500))
            {
                return Convert.ToBase64String(cryptoProvider.GetBytes(24));
            }
        }

        /// <summary>
        /// Method to generate a short salt string for encryption salting.
        /// </summary>
        /// <returns></returns>
        public static string GenerateSalt()
        {
            char[] characters = new char[CoreUtility.Random.Next(10, 20)];
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i] = Convert.ToChar(CoreUtility.Random.Next(33, 126));
            }
            return new String(characters);
        }

        /// <summary>
        /// The technique here was found at http://www.codeproject.com/Articles/191424/Resizing-an-Image-On-The-Fly-using-NET.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        public static Image ProcessImage(FileInfo file, Size finalSize)
        {
            Image image = null;
            using (FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                image = Image.FromStream(stream);
            }
            image = CropToSquareImage(image);
            Image newImage = new Bitmap(finalSize.Width, finalSize.Height);
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, finalSize.Width, finalSize.Height);
            }
            return newImage;
        }


        /// <summary>
        /// Static Random instance for randomization efforts.
        /// </summary>
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// The SQL server version application setting.
        /// </summary>
        public static readonly string SqlServerVersion = ConfigurationManager.AppSettings[SqlServerVersionName];

        /// <summary>
        /// The name for the SQL server version setting.
        /// </summary>
        private const string SqlServerVersionName = "SqlServerVersion";

        /// <summary>
        /// The key name to be used in all config files for storage account connection strings, i.e. "RecipeBoxStorage".
        /// </summary>
        public static readonly string StorageConnectionStringKey = "RecipeBoxStorage";
    }
}
