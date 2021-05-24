using LocationPrism.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LocationPrism.Repositories
{
    public interface IPositionRepository
    {
        Task Save(Position position);
        Task<List<Position>> GetAll();
        Task Clear();
    }

}
