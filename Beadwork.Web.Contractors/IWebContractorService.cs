using System;

namespace Beadwork.Web.Contractors
{
    public interface IWebContractorService
    {
        string UniqueCode { get; }
        string GetUri { get; }

    }
}
