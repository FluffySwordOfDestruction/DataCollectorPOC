using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataWriter
{
    public interface IDataWriter
    {
        bool ProcessData(byte[] data);
    }
}
