using Domain.RequestModels;
using Domain.ResponseModels;
using Domain.Services.Interfaces;
using ImageMagick;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Services.Implementation
{
    public class PhotoService : IPhotoService
    {
        private CloudBlobContainer _container;
        protected string _invalidPhotoMessage = "Invalid Image";
        protected string _invalidPhotoFormatMessage = "Lets use jpeg, gif, png and bmp Image formats.";
        protected string _invalidPhotoDimensionMessage = "Image dimension is either too small or too large.";
        protected string _invalidPhotoSizeMessage = "Image size is either too small or too large.";
        protected string[] _validImageTypes = new[] { "jpeg", "gif", "png", "bmp" };

        public PhotoService()
        {
            var storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            _container = blobClient.GetContainerReference("photo");

            _container.CreateIfNotExists();

            var blobContainerPermission = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
            _container.SetPermissions(blobContainerPermission);
        }


        public virtual async Task<AddPhotoResponse> AddPhotoAsync(AddPhotoRequest addPhotoRequest)
        {
            var addPhotoResponse = new AddPhotoResponse();

            foreach (var photoStream in addPhotoRequest.PhotoStreams)
            {

                var result = ValidateImage(photoStream);

                if (result.IsValid)
                {
                    var image = GetImage(photoStream);

                    var fileExtension = image.Format.ToString().ToLower();
                    var fileName = Guid.NewGuid().ToString();
                    var fileNameAzure = String.Format("{0}.{1}", fileName, fileExtension);

                    addPhotoResponse.NewlyAddedPhotoInfoDetails.Add(new PhotoInfoDetails
                    {
                        AddedPhotoName = fileName,
                        ImageFormat = fileExtension,
                        Width = image.Width,
                        Height = image.Height,
                        IsOriginal = true,
                        PhotoNameInAzure = fileNameAzure
                    });

                    //1. Lets add the original image first
                    await UploadToBlobAzureStorage(fileNameAzure, image);

                    foreach (var photoDimentionInfo in addPhotoRequest.PhotoDimentionInfos)
                    {
                        var clonedImage = image.Clone();


                        if (photoDimentionInfo.ReSizeAndCrop)
                        {

                            //1.Resize

                            var newImageInfo = CalculateAspectRatioFit(clonedImage.Width, clonedImage.Height, photoDimentionInfo.Width, photoDimentionInfo.Height);

                            if (Math.Abs(photoDimentionInfo.Height - (int)newImageInfo.height) <= 1)
                                clonedImage.Resize(0, photoDimentionInfo.Height);
                            else
                                clonedImage.Resize(photoDimentionInfo.Width, 0);


                            //2. Crop it
                            //var newImageInfo = CalculateAspectRatioFit(clonedImage.Width, clonedImage.Height, photoDimentionInfo.Width, photoDimentionInfo.Height);

                            var x = Math.Abs((int)((photoDimentionInfo.Width - clonedImage.Width) / 2));
                            var y = Math.Abs((int)((photoDimentionInfo.Height - clonedImage.Height) / 2));

                            var mG = new MagickGeometry(x, y, photoDimentionInfo.Width, photoDimentionInfo.Height);


                            clonedImage.Crop(mG);


                        }
                        else
                        {
                            clonedImage.Resize(photoDimentionInfo.Width, 0);
                        }


                        //2. lets add the resized images here
                        var fileNameWithDimension = String.Format("{0}_{1}-{2}x{3}.{4}", fileName, addPhotoRequest.Slug, photoDimentionInfo.Width, photoDimentionInfo.Height, fileExtension);                        

                        await UploadToBlobAzureStorage(fileNameWithDimension, clonedImage);
                    }
                }
                else
                {
                    addPhotoResponse.NewlyAddedPhotoErrorInfos.Add(result);
                }
            }

            return addPhotoResponse;
        }

        private dynamic CalculateAspectRatioFit(decimal srcWidth, decimal srcHeight, decimal maxWidth, decimal maxHeight)
        {

            var ratio = Math.Max(maxWidth / srcWidth, maxHeight / srcHeight);

            return new { width = srcWidth * ratio, height = srcHeight * ratio };
        }

        private async Task UploadToBlobAzureStorage(string fileName, MagickImage image)
        {
            var blockBlob = _container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromByteArrayAsync(image.ToByteArray(), 0, image.ToByteArray().Length);
        }

        private MagickImage GetImage(Stream stream)
        {
            var image = new MagickImage(stream);
            return image;
        }

        protected virtual NewlyAddedPhotoErrorInfo ValidateImage(Stream stream)
        {
            //check if stream is empty or bigger than 5 MegaBytes
            if (stream == null || stream.Length == 0 || ((stream.Length / 1024f) / 1024f) > 5)
            {
                return new NewlyAddedPhotoErrorInfo { IsValid = false, ErrorMessage = _invalidPhotoSizeMessage };
            }

            var magickImageInfo = new MagickImageInfo(stream);
            stream.Position = 0;

            //Lets check the image format... it has to be either jpeg, bmp, png or gif
            if (!_validImageTypes.Contains(magickImageInfo.Format.ToString().ToLower()))
            {
                return new NewlyAddedPhotoErrorInfo { IsValid = false, ErrorMessage = _invalidPhotoFormatMessage };
            }


            //5000 width or height
            if (magickImageInfo.Width > 5000 || magickImageInfo.Height > 5000)
            {
                return new NewlyAddedPhotoErrorInfo { IsValid = false, ErrorMessage = _invalidPhotoDimensionMessage };
            }

            return new NewlyAddedPhotoErrorInfo { IsValid = true };
        }


        public string GetFullPath(string fileName)
        {
            var result = _container.GetBlockBlobReference(fileName);
            if (result.Exists())
            {
                return result.Uri.AbsoluteUri;
            }
            return String.Empty;
        }

        public long GetFileSize(string fileName)
        {
            var blob = _container.GetBlockBlobReference(fileName);
            if (blob.Exists())
            {
                blob.FetchAttributes();
                return blob.Properties.Length;
            }
            return 0;
        }

        public async Task<bool> DeletePhotoByNameAsync(string fileName)
        {
            var cloudBlob = _container.GetBlockBlobReference(fileName);
            var result = await cloudBlob.DeleteIfExistsAsync();

            return result;
        }
    }
}