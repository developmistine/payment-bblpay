using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonstructure
{
        public class CustomerPayment
        {
            public string BillerId { get; set; }
            public string TransDate { get; set; }
            public string TransTime { get; set; }
            public string TermType { get; set; }
            public string Amount { get; set; }
            public string Reference1 { get; set; }
            public string Reference2 { get; set; }
            public string FromBank { get; set; }
            public string FromName { get; set; }
            public string ApprovalCode { get; set; }
            public string RetryFlag { get; set; }
            public string Status { get; set; }
        }

    public class logCustomerPayment
    {
        public string BillerId { get; set; }
        public string TransDate { get; set; }
        public string TransTime { get; set; }
        public string TermType { get; set; }
        public string Amount { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string FromBank { get; set; }
        public string FromName { get; set; }
        public string MerchantID { get; set; }
        public string RetryFlag { get; set; }
        public string Status { get; set; }
    }
    public class ObjectReturn
        {
         //   public string BankRef { get; set; }
            public string responseCode { get; set; }
            public string responseMesg { get; set; }
          //  public string TransDate { get; set; }
        }

        public class ObjBBLPay
        {
            public string BillerId { get; set; }
            public string TransDate { get; set; }
            public string Amount { get; set; }
            public string Refernce1 { get; set; }
            public string Refernce2 { get; set; }
            public List<CustomerPayment> CustomerPayment { get; set; }
        }
}
