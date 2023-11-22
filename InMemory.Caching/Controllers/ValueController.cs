using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValueController : ControllerBase
    {
        private IMemoryCache _memoryCache;

        public ValueController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet("set/{name}")]
        public void SetName(string name)
        {
            _memoryCache.Set("name", name);
        }

        [HttpGet]
        public string GetName()
        {
            if (_memoryCache.TryGetValue<string>("name", out string name))
            {
                return name;
            }
            throw new FileNotFoundException("Not Found");
        }

        [HttpGet("setDate")]
        public void SetDate()
        {
            _memoryCache.Set<DateTime>("date", DateTime.Now, options: new()
            {
                // set certain end of the life of cache
                AbsoluteExpiration = DateTime.Now.AddSeconds(30),
                
                // set period for reach cache 
                SlidingExpiration = TimeSpan.FromSeconds(5)
            });
        }
        [HttpGet("getDate")]
        public DateTime GetDate()
        {
            return _memoryCache.Get<DateTime>("date");
        }
    }
}