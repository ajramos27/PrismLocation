using System;
using System.Collections.Generic;
using System.Text;

namespace LocationPrism.Services
{
    public interface ILocationService
    {
        void Start(int interval);
        void Stop();
        void ChangeInterval(int interval);
    }
}
