using EpMon.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EpMon.Web.ViewModels
{
    public static class Mapper
    {
        public static EndpointDto ToDto(this Endpoint e)
        {
            if (e == null)
            {
                return new EndpointDto();
            }
            
            var responseTime = 0.0;
            var upTime = 0.0;

            if (e.Stats.Any())
            {
                responseTime = Math.Round(e.Stats.Average(x => x.ResponseTime), 2);
                upTime = Math.Round(((double)e.Stats.Count(x => x.IsHealthy) / (double)e.Stats.Count()) * 100.00, 2);
            }

            return new EndpointDto
            {
                CheckInterval = e.CheckInterval,
                Id = e.Id,
                IsCritical = e.IsCritical,
                CheckType = (CheckType)e.CheckType,
                IsActive = e.IsActive,
                Name = e.Name,
                PublishStats = e.PublishStats,
                ResponseTime = responseTime,
                Uptime = upTime,
                Stats = e.Stats is null ? new List<EndpointStatDto>() : e.Stats.Select(es => es.ToDto()).ToList(),
                Url = e.Url,
                Tags = e.Tags
            };
        }

        public static EndpointDto ToDto2(this Endpoint e)
        {
            if (e == null)
            {
                return new EndpointDto();
            }
            
            return new EndpointDto
            {
                CheckInterval = e.CheckInterval,
                Id = e.CheckInterval,
                IsCritical = e.IsCritical,
                CheckType = (CheckType)e.CheckType,
                IsActive = e.IsActive,
                Name = e.Name,
                PublishStats = e.PublishStats,
                Url = e.Url,
                Tags = e.Tags
            };
        }

        public static List<EndpointDto> ToDto(this IEnumerable<Endpoint> el)
        {
            if (el == null)
            {
                return new List<EndpointDto>();
            }

            return el.Select(e => e.ToDto()).ToList();
        }

        public static EndpointStatDto ToDto(this EndpointStat es)
        {
            if (es == null)
            {
                return new EndpointStatDto();
            }

            return new EndpointStatDto
            {
                ResponseTime = es.ResponseTime,
                Endpoint = es.Endpoint.ToDto2(),
                EndpointId = es.EndpointId,
                Id = es.Id,
                IsHealthy = es.IsHealthy,
                Message = es.Message,
                Status = es.Status,
                TimeStamp = es.TimeStamp
            };
        }
    }
}
