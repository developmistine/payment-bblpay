using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// เป็นส่วนที่ระบบทำการ Payment
namespace PaymentProject 
{
    public class Sender
    {
        public string senderRole { get; set; }
    }

    public class Ref
    {
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public string ref3 { get; set; }
        public string ref4 { get; set; }
    }

    public class Amount
    {
        public string amount1 { get; set; }
    }

    /// <summary>
    ///  ในส่วนหลักของการ Set Customer เพื่อที่จะส่งข้อมูลรายละเอียดเพื่อทำการรับชำระ 
    /// </summary>
    public class CustomerClassObject
    {
        public string provider { get; set; }
        public Sender sender { get; set; }
        public string productCode { get; set; }
        public string command { get; set; }
        public string transactionType { get; set; }
        public string customerRefNo { get; set; }
        public string bankRefNo { get; set; }
        public Ref @ref { get; set; }
        public Amount amount { get; set; }
        public string transmitDateTime { get; set; }
    }
    
    /// <summary>
    /// ดำเนินการในส่วนของการ Payment 
    /// </summary>
 
    public class CustomerPayment
    {
        public string BankRef { get; set; }
        public string BillerNo { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string QRId { get; set; }
        public string PayerName { get; set; }
        public string PayerBank { get; set; }
        public string Filler { get; set; }
        public string Amount { get; set; }
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public string TransDate { get; set; }
    }


 

    /// <summary>
    ///  กรณีที่ Data ของ Customer ที่ทำการส่งเข้ามาเพื่อที่จะทำการส่งเข้ามาทำการ Gen QR CODE 
    /// </summary>
    public class DataPayment
    {
        public string BillerID { get; set; }
        public string MerchantID { get; set; }
        public string RefType { get; set; }
        public string Ref1 { get; set; }
        public string Ref2 { get; set; }
        public string Amount { get; set; }
        public string TransDate { get; set; }
    }




    /// <summary>
    ///  กรณีที่ Data ของ Customer ที่ทำการส่งเข้ามาเพื่อที่จะทำการส่งเข้ามาทำการ Gen QR CODE 
    ///  โดยข้อมูลที่ส่งเข้ามานั้น  จะเป็นข้อมูลที่ส่งมาทำการ Gen
    /// </summary>
    public class GenQRCode
    {

        public string BillerID { get; set; }
        public string MerchantID { get; set; }
        public string ID1 { get; set; }
        public string Ref1 { get; set; }
        public string ID2 { get; set; }
        public string Ref2 { get; set; }
    }



    public class QRCodeFormat
    {
        public string qrcode { get; set; }
    }


    // กรณีที่ทำการ 
    public class ObjectReturn
    {
        public string BankRef { get; set; }
        public string ResCode { get; set; }
        public string ResDesc { get; set; }
        public string TransDate { get; set; }
    }

}
