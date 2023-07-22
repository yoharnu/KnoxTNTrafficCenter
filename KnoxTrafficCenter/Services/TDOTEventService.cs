using KnoxTrafficCenter.Models;

namespace KnoxTrafficCenter.Services
{
    public class TDOTEventService
    {
        public TDOTEventService()
        {
            HttpClient = new HttpClient();
        }

        public HttpClient HttpClient { get; }

        public IEnumerable<TDOTEvent> GetEvents()
        {
            return null;
        }
    }
}
