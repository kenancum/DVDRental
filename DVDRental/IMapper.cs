using System;
using System.Collections.Generic;
using System.Text;

namespace DVDRental
{
    interface IMapper<T>
    {
        T GetByID(int id);
        void Save(T t);
        void Delete(T t);
    }
}
