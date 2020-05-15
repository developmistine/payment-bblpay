using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentProject
{
     public class Senderres
     {
         public string senderRole { get; set; }
     }

     public class ResponsePay
     {
         public string provider { get; set; }
         public Senderres sender { get; set; }
         public string productCode { get; set; }
         public string command { get; set; }
         public string transactionType { get; set; }
         public string bankRefNo { get; set; }
         public string responseCode { get; set; }
         public string responseDesc { get; set; }
         public string transmitDateTime { get; set; }
     }
}
