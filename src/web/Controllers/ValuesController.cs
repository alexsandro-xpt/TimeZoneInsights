using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using NodaTime.TimeZones;

namespace web.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var tzs = TimeZoneInfo.GetSystemTimeZones();
            var itens = tzs.Select(tz => new TimeZoneViewModel
            {
                Name = tz.DisplayName,
                Id = tz.Id
            }).ToArray();
            return new OkObjectResult(new { Total = itens.Length, Itens = itens });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]TestModel model)
        {
            DateTime dt = model.Date;//new DateTime(model.Date.Ticks, DateTimeKind.Utc);
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(model.TimeZoneId);

            DateTimeOffset utc = DateTime.UtcNow;

            bool isDST = tzi.IsDaylightSavingTime(dt);
            System.Console.WriteLine(dt);

            DateTimeOffset toTimeZone = TimeZoneInfo.ConvertTime(utc, tzi);
            return new OkObjectResult(new
            {
                UtcTimeFromServer = utc,
                UtcTimeFromServerToTimeZone = toTimeZone,
                IsDaylightSavingTime = isDST,
                DaylightName= tzi.DaylightName,
                DisplayName= tzi.DisplayName,
                StandardName= tzi.StandardName,
                BaseUtcOffset= tzi.BaseUtcOffset,
                SupportsDaylightSavingTime= tzi.SupportsDaylightSavingTime,
                Date= dt,
                DateUTC= dt.ToUniversalTime(),
                KindLocal= dt.Kind == DateTimeKind.Local,
                KindUnspecified= dt.Kind == DateTimeKind.Unspecified,
                KindUtc= dt.Kind == DateTimeKind.Utc,
                TimeZoneInfoLocalId= TimeZoneInfo.Local.Id,
                IsAmbiguousTime= tzi.IsAmbiguousTime(dt),
                LocalIsDaylightSavingTime= TimeZoneInfo.Local.IsDaylightSavingTime(dt),
                DtIsDaylightSavingTime = dt.IsDaylightSavingTime(),
                GetAdjustmentRules = tzi.GetAdjustmentRules(),
                NodaTime = TzdbDateTimeZoneSource.Default.ZoneLocations.Select(tz => new { Item = DateTimeZoneProviders.Tzdb[tz.ZoneId] }),
            });
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

    }

    public class TestModel
    {
        public string TimeZoneId { get; set; }
        public DateTime Date { get; set; }
    }

    public class TimeZoneViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
