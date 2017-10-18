using Domain.RequestModels;
using Domain.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<AddPhotoResponse> AddPhotoAsync(AddPhotoRequest addPhotoRequest);
        string GetFullPath(string fileName);
        Task<bool> DeletePhotoByNameAsync(string fileName);
        long GetFileSize(string fileName);
    }
}
