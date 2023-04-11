using System;
using System.Collections.Generic;

namespace Beadwork.Web.Contractors
{
    public interface IWebContractorService
    {
        string Name { get; }
        Uri StartSession(IReadOnlyDictionary<string, string> parameters, Uri returnUri);

    }
}
