using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Ecat.Web.Controllers
{
    public class ErrorController : ApiController
    {
        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpHead, HttpOptions]
        public IHttpActionResult NotFound(string path)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new HttpException(404, "404 Not Found: /" + path));

            return NotFound();
        }
        
    }
}
