using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace IM.Common
{
    public class HttpResponse
    {
        public static void BadRequest()
        {
           // HttpResponseMessage response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "your message");
           // throw new HttpResponseException(response);
        }
    }
}