using KnoxTrafficCenter.Models.TDOT;

namespace KnoxTrafficCenter.Services
{
    public class TDOTEventService
    {
        public TDOTEventService()
        {
            HttpClient = new HttpClient();
        }

        public HttpClient HttpClient { get; }

        public IEnumerable<Event> GetEvents()
        {
            return null;
        }
    }
}
