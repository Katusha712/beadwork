using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beadwork
{
    public interface IPictureRepository
    {
        Picture[] GetAllByTitle(string titlePart);
    }
}
