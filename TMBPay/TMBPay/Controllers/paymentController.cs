using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PaymentProject;

namespace TMBPay.Controllers
{

    /// <summary>
    /// 1. ดำเนินการในส่วนของการ Gen QR Code
    /// 2. ดำเนินการในส่วนของการส่งข้อมูลมาทำการรับชำระ 
    /// </summary>
    public class paymentController : ApiController
    {


        // GET api/payment/5
        public string Get(int id)
        {
            return "value";
        }
       
        // ดำเนินการในส่วนของ Service ที่ทาง ธนาคารส่งมาเพื่อที่จะทำการ Excute 
        /// <summary>
        /// POST api/payment/receivepayment/V1/value
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="objcustomer"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage receivepayment(string Type, [FromBody]CustomerPayment objcustomer)
        {
            SetCustomerPay ex = new SetCustomerPay();
            var res = Request.CreateResponse(HttpStatusCode.OK);
            // ดำเนินการในส่วนของการ Convert To Json   
            string lsjson = ex.RecivePayment(objcustomer);
            res.Content = new StringContent(lsjson, System.Text.Encoding.UTF8, "text/html");
            return res;
        }




        // POST api/payment/genqrpayment/V1/value
        /// <summary>
        ///  ดำเนินการรับข้อมูลที่ดำเนินการทาง Excute เรียบร้อยแล้วส่งมาทางการทำการบันทึกข้อมูล
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="objcustomer"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage genqrpayment(string Type, [FromBody]DataPayment objcustomer)
        {
            SetCustomerPay ex = new SetCustomerPay();
            var res = Request.CreateResponse(HttpStatusCode.OK);
            //ดำเนินการ SET และ ExcuteData เพื่อทำการเก็บ Data เข้ามาในระบบก่อน 
            string lsjson = ex.CustomerPayData(objcustomer);
            res.Content = new StringContent(lsjson, System.Text.Encoding.UTF8, "text/html");
            return res;
        }
    }
}
