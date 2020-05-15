using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonstructure
{
    public class GenQRPaymentS
    {
        public string QRBillerId { get; set; }
        public string QRTransDate { get; set; }
        public string QRTransTime { get; set; }
        public string QRTermType { get; set; }
        public string QRAmount { get; set; }
        public string QRReference1 { get; set; }
        public string QRReference2 { get; set; }
        public string QRFromBank { get; set; }
        public string QRFromName { get; set; }
        public string QRApprovalCode { get; set; }
        public string QRRetryFlag { get; set; }
    }

    public class QRReturnStatusS
    {
        public string QRResponseCode { get; set; }
        public string QRResponseMesg { get; set; }
    }

    public class QRDataPayment
    {
        public string QRBillerID { get; set; }
        public string QRMerchantID { get; set; }
        public string QRRefType { get; set; }
        public string QRReference1 { get; set; }
        public string QRReference2 { get; set; }
        public string QRAmount { get; set; }
        public string QRTransDate { get; set; }
    }

    public class QRCodeFormat
    {
        public string QRresponseCode { get; set; }
        public string QRresponseMasg { get; set; }
        public string QRqrcode { get; set; }
        public string QRcode { get; set; }
    }
}
