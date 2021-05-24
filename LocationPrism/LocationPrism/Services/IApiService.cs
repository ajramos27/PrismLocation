using LocationPrism.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LocationPrism.Services
{
    public interface IApiService
    {
        Task UpdateLocation(Position position);
    }
}
