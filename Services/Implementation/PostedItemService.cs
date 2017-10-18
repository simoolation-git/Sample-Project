using Domain.Interfaces.Services;
using Domain.Models;
using Domain.ModelsHelpers;
using Domain.RequestModels;
using Domain.ResponseModels;
using Domain.Services.Interfaces;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wedding.Repository;

namespace Services.Implementation
{
    public class PostedItemService : IPostedItemService
    {
        private struct SaveImageWrapper
        {
            internal string Url { get; set; }
            internal string ErrorMessage { get; set; }
        }


        private IPhotoService _photoService;
        private ApplicationDbContext _applicationDbContext;

        //private string ImageName_original_StringFormat = "{0}.{1}";
        private string ImageName_450x450_StringFormat = "{0}_{1}-450x450.{2}";
        private readonly string _azureBaseBlobUrl;
        private ICacheService _cacheService;

        public PostedItemService(IPhotoService photoService, ICacheService cacheService)
        {
            _photoService = photoService;
            _cacheService = cacheService;

            _applicationDbContext = new ApplicationDbContext();

            _azureBaseBlobUrl = ConfigurationManager.AppSettings["AzureBaseBlobUrl"];
        }

        public async Task<SavePostedItemResponse> SavePostedItem(AddPhotoViewModel addPhotoViewModel, string userId)
        {
            var response = new SavePostedItemResponse();

            var user = _applicationDbContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                response.Success = false;
                response.ErrorMessage = "Problem saving your post.";
                return response;
            }

            var postedItem = await SavePostedItemToDb(addPhotoViewModel);

            var saveImageWrapper = await SavePhotos(addPhotoViewModel, postedItem);


            postedItem.ApplicationUser = user;
            postedItem.ApplicationUserId = user.Id;

            if (postedItem.Id > 0 && postedItem.Photo != null && !string.IsNullOrEmpty(postedItem.PhotoUrl))
            {
                await _applicationDbContext.SaveChangesAsync();
                response.PostedItem = postedItem;
                response.Success = true;
            }
            //If image is not saved lets not create the posted item
            else if (!String.IsNullOrEmpty(saveImageWrapper.Url) && String.IsNullOrEmpty(saveImageWrapper.ErrorMessage))
            {
                postedItem.PhotoUrl = saveImageWrapper.Url;

                //Save the url as well
                await _applicationDbContext.SaveChangesAsync();
                response.PostedItem = postedItem;
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.ErrorMessage = saveImageWrapper.ErrorMessage;
            }

            await _cacheService.RemovePostedItemFromCache(postedItem.Id.ToString());
            var postedItemViewModel = ConvertFromPostedItemToViewModel.Convert(postedItem, userId);
            await _cacheService.AddPostedItemToCache(postedItem.Id.ToString(), postedItemViewModel);

            return response;
        }

        private async Task<SaveImageWrapper> SavePhotos(AddPhotoViewModel addPhotoViewModel, PostedItem postedItem)
        {
            var saveImageWrapper = new SaveImageWrapper();

            if (addPhotoViewModel.StreamData == null)
            {
                return saveImageWrapper;
            }

            var photoStreams = new List<Stream> { addPhotoViewModel.StreamData };

            var photoDimentionInfos = new List<PhotoDimensionInfo>() {
                new PhotoDimensionInfo { Width = 450, Height = 450, IsOriginal = false, ReSizeAndCrop = true}
            };

            var addPhotoRequest = new AddPhotoRequest(photoStreams, photoDimentionInfos, postedItem.Slug);

            var results = await _photoService.AddPhotoAsync(addPhotoRequest);

            //If any error found during uploading process we will not add anything to database. (ATOMIC)
            if (results.NewlyAddedPhotoErrorInfos != null && results.NewlyAddedPhotoErrorInfos.Count == 0)
            {

                //2. Add photo objects to News Feed object
                if (results != null && results.NewlyAddedPhotoInfoDetails.Count > 0)
                {
                    foreach (var photo in results.NewlyAddedPhotoInfoDetails)
                    {
                        var newPhoto = new Photo
                        {
                            Name = photo.AddedPhotoName,
                            FileExtention = photo.ImageFormat,
                            Width = photo.Width,
                            Height = photo.Height,
                            Inserted = DateTime.UtcNow,
                            PhotoType = PhotoType.PostedItem,
                            Container = "photo"
                        };

                        _applicationDbContext.Photos.Add(newPhoto);

                        postedItem.Photo = newPhoto;
                    }
                }

                //5. Save them to database
                var saveResults = await _applicationDbContext.SaveChangesAsync();
            }
            else if (results.NewlyAddedPhotoErrorInfos.Count > 0)
            {
                saveImageWrapper.ErrorMessage = results.NewlyAddedPhotoErrorInfos[0].ErrorMessage;
            }

            //var response = new AddUserAvatarResponse();

            //6. Lets get the full path on the blob for the new photo
            var originalPhotoDetails = results.NewlyAddedPhotoInfoDetails.FirstOrDefault(p => p.IsOriginal);
            var photoName = String.Format(ImageName_450x450_StringFormat, originalPhotoDetails.AddedPhotoName, postedItem.Slug, originalPhotoDetails.ImageFormat);

            var fullPhotoUrlOnAzureBlobServer = _photoService.GetFullPath(photoName);

            saveImageWrapper.Url = fullPhotoUrlOnAzureBlobServer;

            return saveImageWrapper;
        }

        private async Task<PostedItem> SavePostedItemToDb(AddPhotoViewModel addPhotoViewModel)
        {
            if (addPhotoViewModel == null)
                return null;

            PostedItem postedItem = null;
            var timeNow = DateTime.UtcNow;

            //if we already have it in database
            if (addPhotoViewModel.PostedId != null)
            {
                postedItem = await _applicationDbContext.PostedItems.FirstOrDefaultAsync(p => p.Id == addPhotoViewModel.PostedId.Value);

                if (postedItem != null)
                {
                    postedItem.Title = addPhotoViewModel.Title;

                    postedItem.Tags.Clear();
                    await AddTagsToPostedItem(postedItem, addPhotoViewModel.Tags);

                    //If a new photo is uploaded lets remove the old one.
                    if (!String.IsNullOrEmpty(addPhotoViewModel.PhotoData) && postedItem.Photo != null)
                    {
                        _applicationDbContext.Photos.Remove(postedItem.Photo);
                    }
                }
            }
            else
            {
                postedItem = new PostedItem { Title = addPhotoViewModel.Title, Inserted = timeNow, PostedItemType = PostedItemType.UserPosted };
                await AddTagsToPostedItem(postedItem, addPhotoViewModel.Tags);
                _applicationDbContext.PostedItems.Add(postedItem);
            }

            if (!String.IsNullOrEmpty(postedItem.Title))
                postedItem.Slug = postedItem.Title.Trim().Replace(' ', '-').Replace('.', ' ').Replace('&', '_').TrimEnd();

            postedItem.Updated = timeNow;

            return postedItem;
        }


        private async Task AddTagsToPostedItem(PostedItem postedItem, string[] tags)
        {
            if (tags == null || tags.Length == 0)
                return;

            List<string> tagsList = new List<string>();

            foreach (var tagViewModel in tags)
            {
                var tag = await GetTagByName(tagViewModel);
                postedItem.Tags.Add(tag);
                tagsList.Add(string.Format("\"{0}\"", tagViewModel));
            }

            var tagsTokenized = String.Format("[{0}]", String.Join(",", tagsList));

            postedItem.TagsTokenized = tagsTokenized.ToString().TrimEnd();
        }

        private async Task<Tag> GetTagByName(string name)
        {
            var tag = await _applicationDbContext.Tags.FirstOrDefaultAsync(t => t.Name == name);

            if (tag == null)
            {
                tag = new Tag() { Name = name };
                _applicationDbContext.Tags.Add(tag);
            }

            return tag;
        }

        public async Task<ICollection<PostedItem>> GetPostedItemByUserId(string userId)
        {
            var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return new List<PostedItem>();

            return user.PostedItems.Where(pi => pi.IsDeleted == false).OrderByDescending(p => p.Id).ToList();
        }

        public string GetAzureBaseBlobUrl()
        {
            return _azureBaseBlobUrl;
        }

        public async Task<bool> RemovePostedItem(long id)
        {
            var postedItem = await GetPostedItemById(id);

            if (postedItem != null)
            {
                //if (postedItem.Photo != null)
                //{
                //    var results = await _photoService.DeletePhotoByNameAsync(String.Format(ImageName_original_StringFormat, postedItem.Photo.Name, postedItem.Photo.FileExtention));
                //    var results2 = await _photoService.DeletePhotoByNameAsync(String.Format(ImageName_300x300_StringFormat, postedItem.Photo.Name, postedItem.Photo.FileExtention));

                //    if (!results || !results2)
                //        return false;

                //    _applicationDbContext.Photos.Remove(postedItem.Photo);
                //}
                //postedItem.Tags.Clear();

                postedItem.IsDeleted = true;
                postedItem.Updated = DateTime.UtcNow;

                //_applicationDbContext.PostedItems.Remove(postedItem);
                await _applicationDbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        private async Task<PostedItem> GetPostedItemById(long id)
        {
            var postedItem = await _applicationDbContext.PostedItems.FirstOrDefaultAsync(p => p.Id == id);

            return postedItem;
        }


        public async Task<PostedItemViewModel> GetPostedItemViewModelById(long id, string userId = null)
        {
            var cachedPostedItem = await _cacheService.GetPostedItemFromCache(id.ToString(), userId);

            if (cachedPostedItem != null)
                return cachedPostedItem;

            var postedItem = await GetPostedItemById(id);

            var postedItemViewModel = ConvertFromPostedItemToViewModel.Convert(postedItem, userId);

            await _cacheService.AddPostedItemToCache(id.ToString(), postedItemViewModel);

            return postedItemViewModel;
        }

        public async Task<ICollection<PostedItem>> GetPostedItems(int numberOfPostedItems)
        {
            var postedItems = await _applicationDbContext.PostedItems.Where(p => !p.IsDeleted).OrderByDescending(p => p.Inserted).Take(numberOfPostedItems).ToListAsync();
            return postedItems;
        }

        public async Task<ICollection<PostedItem>> GetPostedItemsByPageNumber(int pageNumber, int numberOfPostedItems)
        {
            var skip = 0;

            if (pageNumber > 1)
            {
                skip = (pageNumber - 1) * numberOfPostedItems;
            }

            var postedItems = await _applicationDbContext.PostedItems.OrderByDescending(p => p.Inserted).Skip(skip).Take(numberOfPostedItems).ToListAsync();

            return postedItems;
        }
    }


}