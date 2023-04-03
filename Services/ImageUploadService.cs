using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Forage.Services
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(byte[] imageBytes, string fileName);
    }

    public class ImageUploadService : IImageUploadService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public ImageUploadService(CloudinaryDotNet.Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadImageAsync(byte[] imageBytes, string fileName)
        {
            using (var memoryStream = new MemoryStream(imageBytes))
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, memoryStream)
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.Url.ToString();
            }
        }
    }
}
