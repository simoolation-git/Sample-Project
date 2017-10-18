using System.Collections.Generic;
using System.IO;

namespace Domain.RequestModels
{
    public class AddPhotoRequest
    {
        public IEnumerable<Stream> PhotoStreams { get; private set; }
        public IEnumerable<PhotoDimensionInfo> PhotoDimentionInfos { get; private set; }

        public string Slug { get; private set; }

        public AddPhotoRequest(IList<Stream> photoStreams, IList<PhotoDimensionInfo> photoDimentionInfos, string slug)
        {
            PhotoStreams = photoStreams;
            PhotoDimentionInfos = photoDimentionInfos;
            Slug = slug;
        }
    }
}