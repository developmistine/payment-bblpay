

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Globalization;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using MySql.Data.MySqlClient;
namespace PaymentProject 
{
  public  class SetCustomerPay
    {
    // ประกาศตัวแปลในส่วนของการบันทึกข้อมูลเข้ามาในระบบ 
    MySqlConnection connect = new MySqlConnection();
    MySqlCommand command = new MySqlCommand();
    MySqlDataAdapter adap = new MySqlDataAdapter();
      public string CustomerGenQRCode(CustomerClassObject objcus)
      {
          string lsreturn = string.Empty;
          try
          {

              //ดำเนินการ SET DATA JSON
              objcus.provider = "MISTINE";
              Sender obj_sender = new Sender();
              obj_sender.senderRole = "MISTINE";
              objcus.sender = obj_sender;
              objcus.productCode = "MERPRD";
              objcus.command = "QR_GEN";
              objcus.transactionType = "RQT";
              objcus.customerRefNo = "312018810392401";
              objcus.bankRefNo = "TMB20171221111111";
              objcus.transmitDateTime = DateTime.Now.ToString("yyyyMMddhhmmss");
              //  Ref จะถูกส่งมาจากด้านนอก        
              lsreturn = JsonConvert.SerializeObject(objcus);

              // จากนั้นทำการ CallService เพื่อที่จะทำการ Get Data เพื่อ Gen QR CODE 
              // ทำการ GET DATA มาจาก INI
              // ------> string lsurl = string.Empty;
              // ดำเนินการ Call Service 
              // ------>   string lsdata = CreateObject(lsurl, lsreturn);
              // จากนั้นเอา  JSON มาทำการ Convert ที่ได้ 
              string response  = System.IO.File.ReadAllText(@"C:\APP\QRCODETMB\Responsepay.txt");


              // เอา JSON มาทำการ CONVERT เป็น  OBJECT 
              ResponsePay obj = JsonConvert.DeserializeObject<ResponsePay>(response);
              // จากนั้นก็จะได้ PATH ของ BITMAP ออกมาก  จากนั้นก็ดำเนินการเปลี่ยนเป็นรูปภาพ 
              string lspathqrcode = GenQRCode(obj.responseDesc);
          }
          catch
          {
          }
          return lsreturn;
      }

      /// <summary>
      ///  ระบบในส่วนของ Gen
      /// </summary>
      /// <param name="objcus"></param>
      /// <returns></returns>
      public string CustomerPayData(DataPayment objcus)
      {
          string lsreturn = string.Empty;
          try
          {
              // ระบบดำเนินการ GenQRCode เพื่อส่งให้ Service 
              lsreturn = GenQRCode(objcus);
          }
          catch
          {
          }
          return lsreturn;
      }


      /// <summary>
      /// ในส่วนที่ทำการ Gen qrcode ออกมาจากระบบ 
      /// </summary>
      /// <param name="objcus"></param>
      /// <returns></returns>
      public string GenQRCode(DataPayment objcus)
      {
          string lsreturn = string.Empty;
          string lscrc = string.Empty;
          
          StringBuilder stradd = new StringBuilder();
          
            // Parameter ตัวที่ 1
            // string  P1_ID  =  "00";
            // string  P1_Length =  "02";
            // string  P1_Input_Value =  "01";
            string  P1_Payload_Format_Indicator = "000201";
            stradd.Append(P1_Payload_Format_Indicator);

            // Parameter ตัวที่ 2 
           // string   P2_ID  =   "01";
           // string   P2_Length =  "02";
           // string   P2_Input_Value = "12";
            string   P2_Point_of_Initiation_method = "010212";
            stradd.Append(P2_Point_of_Initiation_method);
            

            //string P3_ID = "00";
            //string P3_Length = "16";
            //string P3_Input_Value = "A000000677010112";
            string  P3_AID = "0016A000000677010112";
            


            string P4_ID = "01";
            string P4_Length = "15";
            string P4_Input_Value = objcus.BillerID.ToString();
            string P4_Biller_ID = P4_ID + P4_Length + P4_Input_Value; 


            string P5_ID = "02";
            string P5_Length = "07";
            string P5_Input_Value = objcus.Ref1.ToString();
            string P5_Reference1 = P5_ID + P5_Length + P5_Input_Value; //  "02071010015";


            string P6_ID = "03";
            string P6_Length = "07";
            string P6_Input_Value = objcus.Ref2.ToString();
            string P6_Reference2 = P6_ID + P6_Length + P6_Input_Value;  // "03071020015";

            // กรณีที่ทำการ Set Parameter ในส่วนที่่เป็นตัว String ต่อยาว
            string P7_ID = "30";
            string P7_Length = "61";
            StringBuilder str  = new StringBuilder();
                str.Append(P7_ID);
                str.Append(P7_Length);
                str.Append(P3_AID);
                str.Append(P4_Biller_ID);
                str.Append(P5_Reference1);
                str.Append(P6_Reference2);
                string P7_Merchant_identifier = str.ToString();
                stradd.Append(P7_Merchant_identifier);


            //    string P8_ID = "53";
            //    string P8_Length = "03";
            //    string P8_Input_Value = "764"; 
            string   P8_TransactionCurrencyCode = "5303764";
            stradd.Append(P8_TransactionCurrencyCode);

            string P9_ID = "54";
            string P9_Length = "07";
            string P9_Input_Value = objcus.Amount.ToString();
            string P9_TransactionAmount = P9_ID + P9_Length + P9_Input_Value;  //"54075000.77";
            stradd.Append(P9_TransactionAmount);

            // string P10_ID = "58";
            // string P10_Length = "02";
            // string P10_Input_Value = "TH";
            string   P10_CountryCode = "5802TH";
            stradd.Append(P10_CountryCode);

            string P11_ID = "59";
            string P11_Length = "03";
            string P11_Input_Value = objcus.MerchantID.ToString();
            string P11_MerchantName = P11_ID + P11_Length + P11_Input_Value;  //"5903SHS";
            stradd.Append(P11_MerchantName);
            // string P12_ID = "62";
            // string P12_Length = "11";
            // string P12_Input_Value = ""; 
            //*** ในส่วนนี้ทำการบวกกับ P13_TerminalID เลย  
            string P12_AdditionalDataFieldTemplate = "6211";
            stradd.Append(P12_AdditionalDataFieldTemplate);

             string P13_ID = "07";
             string P13_Length = "07D";
             string P13_Input_Value = objcus.MerchantID.ToString() + "005";
             string P13_TerminalID = P13_ID + P13_Length + P13_Input_Value;
            stradd.Append(P13_TerminalID);
            
            string   P14_CheckSum = "6304";
            stradd.Append(P14_CheckSum);
            lsreturn = stradd.ToString();
            
            
          try
          {
              // ประกาศตัวแประเพื่อรัย
              byte[] bytes = Encoding.ASCII.GetBytes(lsreturn);
              //   byte[] bytes = HexStringToByteArray(strInput);
              //  ushort xxx = CrcHelper.ToUInt16(bytes);
              Crc16Ccitt Test = new Crc16Ccitt(InitialCrcValue.NonZero1);
              byte[] returnt = Test.CodeChecksumBytes(bytes,ref lscrc);
              string s2 = BitConverter.ToString(bytes);
              // ดำเนินการส่ง DATA กลับคืนในส่วนของ SRVICE 
              QRCodeFormat genqr = new QRCodeFormat();
              // ทำการ Return QR Code ออกมากจากระบบ 
              genqr.qrcode = lsreturn + lscrc;
              try
              {
                  //  ส่งไปทำการบันทึกข้อมูลลงในระบบก่อน  ก่่อนที่่จะ

                  
                  MySQLHelper Exmc = new MySQLHelper();
                  string query = string.Empty;
                  connect = Exmc.OpenConnection();
                  command = connect.CreateCommand();
                  command.Connection = connect;
                    query = @"
                               INSERT INTO db_genqrpayment(BillerID, MerchantID, RefType, Ref1, Ref2, Amount, TransDate,CreateDate)
                                    VALUES (@BillerID,@MerchantID,@RefType,@Ref1,@Ref2,@Amount,@TransDate,@CreateDate);
                        ";
                  command.Parameters.Clear();
                  command.CommandType = CommandType.Text;
                  command.CommandText = query.ToLower();
                  MySqlParameter para_BillerID = new MySqlParameter("@BillerID", objcus.BillerID);
                  MySqlParameter para_MerchantID = new MySqlParameter("@MerchantID", objcus.MerchantID);
                  MySqlParameter para_RefType = new MySqlParameter("@RefType", objcus.RefType);
                  MySqlParameter para_Ref1 = new MySqlParameter("@Ref1", objcus.Ref1);
                  MySqlParameter para_Ref2 = new MySqlParameter("@Ref2", objcus.Ref2);
                  MySqlParameter para_Amount = new MySqlParameter("@Amount", Convert.ToDecimal(objcus.Amount));
                  MySqlParameter para_TransDate = new MySqlParameter("@TransDate", objcus.TransDate);
                  MySqlParameter para_CreateDate = new MySqlParameter("@CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm", new CultureInfo("en-US")));
                  command.Parameters.Add(para_BillerID);
                  command.Parameters.Add(para_MerchantID);
                  command.Parameters.Add(para_RefType);
                  command.Parameters.Add(para_Ref1);
                  command.Parameters.Add(para_Ref2);
                  command.Parameters.Add(para_Amount);
                  command.Parameters.Add(para_TransDate);
                  command.Parameters.Add(para_CreateDate);
                  command.ExecuteNonQuery();
                  lsreturn = JsonConvert.SerializeObject(genqr);
              }
              catch
              {
                  genqr = new QRCodeFormat();
                  // ทำการ Return QR Code ออกมากจากระบบ 
                  genqr.qrcode =  "999";
                  lsreturn = JsonConvert.SerializeObject(genqr);
              }
          }
          catch
          {
          }
          return lsreturn;
      }




      /// <summary>
      ///  ดำเนินการสร้าง object จากนั้นก็ ก็ทำการ Return Code ที่เป็นการทำงานที่สำเร็จออกมาจากระบบ
      /// </summary>
      /// <param name="objcus"></param>
      /// <returns></returns>
      public string RecivePayment(CustomerPayment objcus)
      {
          // เป็นตัวแปรที่ดำเนินการเก็บค่าของข้อมูลระบบ ที่หลังที่ทำการ Excute  แล้วทำการ Return ออกมา เพื่อทำาการ ส่งข้อมูลมาทำการงาน 
          ObjectReturn result = new ObjectReturn();
          string lsreturn  = string.Empty;
          string lsBankRef = string.Empty;
          string lsResCode = string.Empty;
          string lsResDesc = string.Empty;
          string lsTransDate = string.Empty;
          string query = string.Empty;
           
           // เป็นตัวแปรที่ได้จากการ Excute 
           lsreturn = string.Empty;
           lsBankRef = objcus.BankRef;
           lsResCode = "000";
           lsResDesc = "Success";
           lsTransDate = objcus.TransDate;
           try
           {
               query = @"INSERT INTO db_receivepayment(BankRef, BillerNo, Ref1, Ref2, QRId, PayerName, PayerBank,Filler,
                         Amount,ResultCode,ResultDesc,TransDate,CreateDate,Status)
                  VALUES (@BankRef, @BillerNo, @Ref1, @Ref2, @QRId, @PayerName, @PayerBank,@Filler,
                         @Amount,@ResultCode,@ResultDesc,@TransDate,@CreateDate,@Status);
            ";
           MySQLHelper Exmc = new MySQLHelper();
           connect = Exmc.OpenConnection();
           command = connect.CreateCommand();
           command.Connection = connect;
           command.Parameters.Clear();
           command.CommandType = CommandType.Text;
           command.CommandText = query.ToLower();
           MySqlParameter para_BankRef = new MySqlParameter("@BankRef", objcus.BankRef);
           MySqlParameter para_BillerNo = new MySqlParameter("@BillerNo", objcus.BillerNo);
           MySqlParameter para_Ref1 = new MySqlParameter("@Ref1", objcus.Ref1);
           MySqlParameter para_Ref2 = new MySqlParameter("@Ref2", objcus.Ref2);
           MySqlParameter para_QRId = new MySqlParameter("@QRId", objcus.QRId);
           MySqlParameter para_PayerName = new MySqlParameter("@PayerName", objcus.PayerName);
           MySqlParameter para_PayerBank = new MySqlParameter("@PayerBank", objcus.PayerBank);
           MySqlParameter para_Filler = new MySqlParameter("@Filler", objcus.Filler);
           MySqlParameter para_Amount = new MySqlParameter("@Amount", Convert.ToDecimal(objcus.Amount));
           MySqlParameter para_ResultCode = new MySqlParameter("@ResultCode", objcus.ResultCode);
           MySqlParameter para_ResultDesc = new MySqlParameter("@ResultDesc", objcus.ResultDesc);
           MySqlParameter para_TransDate = new MySqlParameter("@TransDate", objcus.TransDate);
           MySqlParameter para_CreateDate = new MySqlParameter("@CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm", new CultureInfo("en-US")));
           MySqlParameter para_Status = new MySqlParameter("@Status", "NM");
            command.Parameters.Add(para_BankRef);
            command.Parameters.Add(para_BillerNo);
            command.Parameters.Add(para_Ref1);
            command.Parameters.Add(para_Ref2);
            command.Parameters.Add(para_QRId);
            command.Parameters.Add(para_PayerName);
            command.Parameters.Add(para_PayerBank);
            command.Parameters.Add(para_Filler);
            command.Parameters.Add(para_Amount);
            command.Parameters.Add(para_ResultCode);
            command.Parameters.Add(para_ResultDesc);
            command.Parameters.Add(para_TransDate);
            command.Parameters.Add(para_CreateDate);
            command.Parameters.Add(para_Status);
            command.ExecuteNonQuery();
            // ประกาศตัแปรเพื่อทำการรับข้อมูลเข้ามาจากนั้นก็ทำการ Return ออกไปเป็น Json
            result = new ObjectReturn();
            // ประกาศตัวแประเพื่อรัย
            result.BankRef  = lsBankRef.Trim().ToString();
            result.ResCode = lsResCode.Trim().ToString();
            result.ResDesc = lsResDesc.Trim().ToString();
            result.TransDate = lsTransDate.Trim().ToString();
            // ดำเนินการ Return โดยที่ทำการ Convert ออกมาเป็น File json
            lsreturn = JsonConvert.SerializeObject(result);
          }
          catch(Exception ex)
          {
              result = new ObjectReturn();
              // ประกาศตัวแประเพื่อรัย
              result.BankRef = lsBankRef.Trim().ToString();
              result.ResCode = "999";
              result.ResDesc = ex.ToString();
              result.TransDate = lsTransDate.Trim().ToString();
              // ดำเนินการ Return โดยที่ทำการ Convert ออกมาเป็น File json
              lsreturn = JsonConvert.SerializeObject(result);
          }
          return lsreturn;
      }





















      // จากนั้นทำการ GEN IMG FILE ออกมา 
      private string GenQRCode(string lsbitmat)
      {
          string lspath = string.Empty;
          string pathsave = string.Empty;
          try
          {
              CreateFolder cx = new CreateFolder();
              byte[] imageBytes = Convert.FromBase64String(lsbitmat);
              MemoryStream ms = new MemoryStream(imageBytes, 0,
                imageBytes.Length);
              // Convert byte[] to Image
              ms.Write(imageBytes, 0, imageBytes.Length);
              Image image = Image.FromStream(ms, true);
              pathsave = @"C:\APP\QRCODETMB\qrcodepay\pay\";
              string PathFolder = pathsave + @"IMG\" + DateTime.Now.ToString("yyyyMMdd", new CultureInfo("en-US")) + "\\";
              pathsave = PathFolder + @"\" + DateTime.Now.ToString("yyyyMMdd", new CultureInfo("en-US")) + ".jpg";
              cx.Create_Folder(PathFolder);
              FileInfo file = new FileInfo(pathsave);
              if (!file.Exists)
              {
                  image.Save(pathsave);
              }
          }
          catch
          {

          }

          // ทำการ Return Path รูปภาพออกมา 
          return lspath;
      }


      public string CreateObject(string URL, string DATA)
      {
          string response = string.Empty;
          HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
          request.Method = "POST";
          request.ContentType = "application/json";
          request.ContentLength = DATA.Length;
          StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
          requestWriter.Write(DATA);
          requestWriter.Close();

          try
          {
              WebResponse webResponse = request.GetResponse();
              Stream webStream = webResponse.GetResponseStream();
              StreamReader responseReader = new StreamReader(webStream);
              response = responseReader.ReadToEnd();
              responseReader.Close();
          }
          catch (Exception e)
          {
          }
          finally
          {
          }
          return response;
      }
    }
}
