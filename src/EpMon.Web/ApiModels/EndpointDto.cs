
namespace EpMon.Web.ApiModels
{
    public class EndpointDto
    {
        public int? Id { get; set; }
        public int CheckInterval { get; set; }
        public int CheckType { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Tags { get; set; }
        public bool IsActive { get; set; }
        public bool IsCritical { get; set; }
        public bool PublishStats { get; set; }
    }

}
