using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http.Headers;

namespace BBLPay
{
    public static class WebApiConfig
    {
        /// <summary>
        /// ดำเนินการ SET CONFIG เพื่อที่จะ Call API 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            
            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );


            config.Routes.MapHttpRoute(
                name: "RoutesApisection",
                routeTemplate: "api/{controller}/{action}/{type}/dataid/{id}",
                defaults: new { action = "DefaultAction2", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "RoutesApisection1",
                routeTemplate: "api/{controller}/{action}/{type}/value/{id}",
                defaults: new { action = "DefaultAction3", id = RouteParameter.Optional }
            );


            //https://bblpayment.ningnongshoppingthailand.com/PaymentBBL/providerId/NNQR
            //https://bblpayment.ningnongshoppingthailand.com/PaymentBBL/api/payment/receivepayment/providerId/NNQR
            config.Routes.MapHttpRoute(
                name: "RoutesApisection2",
                routeTemplate: "api/{controller}/{action}/{type}/NNQR/{id}",
                defaults: new { action = "DefaultAction3", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
               name: "RoutesApisection3",
               routeTemplate: "api/{controller}/{action}/{type}/YUPINQR/{id}",
               defaults: new { action = "DefaultAction3", id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
                name: "ContactApi",
                routeTemplate: "api/{controller}/{action}/{type}/value1/{email}/value2/{password}",
                defaults: new { action = "DefaultAction4", type = RouteParameter.Optional, email = RouteParameter.Optional, password = RouteParameter.Optional }
            );




            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));



        }
    }
}
