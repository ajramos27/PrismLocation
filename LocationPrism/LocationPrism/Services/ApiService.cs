using LocationPrism.Models;
using LocationPrism.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LocationPrism.Services
{
    public class ApiService : IApiService
    {
        private IPositionRepository _positionRepository;
        public ApiService(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        public async Task UpdateLocation(Position position)
        {
            position.Time = DateTimeOffset.Now.ToString();
            await _positionRepository.Save(position);
            Console.WriteLine("Saved");
        }
    }
}
