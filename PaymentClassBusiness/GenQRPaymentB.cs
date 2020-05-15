using VerifySlipClassConnect;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Jsonstructure;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PaymentClassBusiness.QRClass;
using PaymentProject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PaymentClassBusiness
{


    public class GenQRPaymentB
    {
        // ประกาศตัวแปลในส่วนของการบันทึกข้อมูลเข้ามาในระบบ 
        //MySqlConnection connect = new MySqlConnection();
        //MySqlCommand command = new MySqlCommand();
        //MySqlTransaction Trans = null;


        public string QRCustomerPayData(QRDataPayment objcus)
        {
            string lsreturn = string.Empty;
            try
            {
                GenQRConnect ObjConnect = new GenQRConnect();
                // ระบบดำเนินการ GenQRCode เพื่อส่งให้ Service 
                lsreturn = ObjConnect.GenQRCode(objcus);

            }
            catch (Exception ex)
            {
            }
            return lsreturn;
        }
    }
}


