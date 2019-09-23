using System;

namespace EpMon.Helpers
{
    public static class UriHelper
    {
        public static string GetBaseUri(string address)
        {
            var uri = new Uri(address);
            var baseUri = uri.GetLeftPart(UriPartial.Authority);
            return baseUri;
        }
    }
}