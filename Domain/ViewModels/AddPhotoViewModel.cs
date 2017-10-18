using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace Domain.ViewModels
{
    public class AddPhotoViewModel : BaseViewModel
    {
        private Stream stream { get; set; }

        public string PhotoData { get; set; }

        public Stream StreamData
        {
            get
            {
                Stream memoryStream = null;

                if (String.IsNullOrEmpty(PhotoData))
                    return memoryStream;

                try
                {
                    memoryStream = new MemoryStream(Convert.FromBase64String(PhotoData));
                }
                catch (Exception)
                {
                    memoryStream = null;
                }

                stream = memoryStream;

                return memoryStream;
            }

            private set
            {
                stream = value;
            }
        }

        [Required(ErrorMessage = "Please add a title")]
        public string Title { get; set; }

        public string[] Tags { get; set; }

        public long? PostedId { get; set; }

        public AddPhotoViewModel()
        {
            //Tags = new List<string>();
        }
    }
}