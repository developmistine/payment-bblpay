using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonstructure
{
    public class PaymentJson
    {
        public string BillerId { get; set; }
        public string TransDate { get; set; }
        public string Amount { get; set; }
        public string Refernce1 { get; set; }
        public string Refernce2 { get; set; }
    }

    public class ReturnStatus
    {
        public string ResponseCode { get; set; }
        public string ResponseMesg { get; set; }
    }

    
    public class BBLPay
    {
        public string billerId { get; set; }
        public string transDate { get; set; }
        public string amount { get; set; }
        public string reference1 { get; set; }
        public string reference2 { get; set; }
    }
    public class OutputDABBL
    {
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public string TransDate { get; set; }
        public string PeriodTime { get; set; }
        public string BillerId { get; set; }
        public List<PTransaction> PTransaction { get; set; }
    }
    public class PTransaction
    {
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string TransDate { get; set; }
        public string PayerName { get; set; }
        public string PayerBank { get; set; }
        public string BankRef { get; set; }
        public string Amount { get; set; }
        public string Fee { get; set; }
        public string PTransDate { get; set; }
    }
    public class CustomerPayment2
    {

        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public string TransDate { get; set; }
    }

    public class VerifySlip
    {
        public string billerId { get; set; }
        public string transDate { get; set; }
        public string amount { get; set; }
        public string reference1 { get; set; }
        public string reference2 { get; set; }
    }
    public class Verify
    {
        public string BillerId { get; set; }
        public string responseCode { get; set; }
        public string Amount { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string tags { get; set; }
        public string CreateDate { get; set; }
    }

}
