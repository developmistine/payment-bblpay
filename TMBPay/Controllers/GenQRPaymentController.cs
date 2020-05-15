using Jsonstructure;
using Newtonsoft.Json;
using PaymentClassBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BBLPay.Controllers
{
    public class GenQRPaymentController : ApiController
    {

        //
        // [api/genqrpayment/QRgenqrpayment/type/value]
        [HttpPost]
        public HttpResponseMessage QRgenqrpayment(string Type, [FromBody]QRDataPayment objcustomer)
        {
            GenQRPaymentB ex = new GenQRPaymentB();
            var res = Request.CreateResponse(HttpStatusCode.OK);
            //ดำเนินการ SET และ ExcuteData เพื่อทำการเก็บ Data เข้ามาในระบบก่อน
            string lsjson = ex.QRCustomerPayData(objcustomer);
            res.Content = new StringContent(lsjson, System.Text.Encoding.UTF8, "text/html");
            return res;
        }

    }
}
