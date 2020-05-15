using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Jsonstructure;
using Newtonsoft.Json;
using PaymentClassBusiness;

namespace BBLPay.Controllers
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
            string lsjson = ex.RecivePaymentCopy(objcustomer);
            res.Content = new StringContent(lsjson, System.Text.Encoding.UTF8, "application/json");
            return res;
        }

        
        [HttpPost]
        /// [api/payment/BBLPayMentController/type/value]
        public HttpResponseMessage BBLPayMentController(string type, [FromBody]PaymentJson ObjParam)
        {
            string resultjson = string.Empty;
            var res = Request.CreateResponse(HttpStatusCode.OK);
            string lstagAction = string.Empty;
            string lsstring = string.Empty;

            object obj = new object();

            try
            {
                SetCustomerPay ObjExcute = new SetCustomerPay();
                obj = ObjExcute.GetBBLPayment(ObjParam);
                resultjson = JsonConvert.SerializeObject(obj);
                res.Content = new StringContent(resultjson, System.Text.Encoding.UTF8, "application/json");
                return res;
            }
            catch (Exception ex)
            {
                lstagAction = "Error";
                string result = "{\"" + ex.ToString() + "\": " + JsonConvert.SerializeObject("false") + " }";
                res.Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json");
                return res;
            }

        }


    }
}
