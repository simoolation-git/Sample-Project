using System.Collections.Generic;

namespace Domain.ResponseModels
{
    public class AddPhotoResponse : ServiceResponseBase
    {
        public IList<PhotoInfoDetails> NewlyAddedPhotoInfoDetails { get; set; }
        public IList<NewlyAddedPhotoErrorInfo> NewlyAddedPhotoErrorInfos { get; set; }

        public AddPhotoResponse()
        {
            NewlyAddedPhotoInfoDetails = new List<PhotoInfoDetails>();
            NewlyAddedPhotoErrorInfos = new List<NewlyAddedPhotoErrorInfo>();
        }
    }
}