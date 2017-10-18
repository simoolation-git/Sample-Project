using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface IOpinionsTableStorage
    {
        Task UpdateOpinion(string userId, Dictionary<long, int> opinions);
        Task<Dictionary<long, int>> GetOpinion(string userId);
    }
}
