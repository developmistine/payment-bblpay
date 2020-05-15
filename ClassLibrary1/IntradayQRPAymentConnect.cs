using Jsonstructure;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PaymentProject;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VerifySlipClassConnect
{
    public class VerifySlipConnect
    {
        MySqlConnection connect = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();
        MySqlDataAdapter adap = new MySqlDataAdapter();
        MySQLHelper exmc = new MySQLHelper();
        MySqlTransaction Trans = null;

        public string VerifySlipData(string Type, VerifySlip objcus)
        {
            string lsreturn = string.Empty;
            try
            {
                lsreturn = VerifySlip(Type, objcus);
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


        public string VerifySlip(string Type, VerifySlip objcus)
        {
            string lsreturn = string.Empty;
            string response = string.Empty;

            var url = @"https://pspaymentgateway.bangkokbank.com:2443/qrservice/api/rss/bbl/v1/transType/QRC_PYMT_TXN/transCode/INQUIRY_PPBP/clientId/NNQR/providerId/QRCGW";


            var cli = new WebClient();
            //   cli.Headers[HttpRequestHeader.ContentType] = "application /json";
            string xx = "{\"billerId\":\"" + objcus.billerId + "\" , \"transDate\":\"" + objcus.transDate + "\",\"amount\":\"" + objcus.amount + "\",\"reference1\":\"" + objcus.reference1 + "\",\"reference2\":\"" + objcus.reference2 + "\"}";
            //   string xx = "{\"customerId\":\"" + objcus.BillerNo + "\" , \"token\":\"" + objcus.QRId + "\"}";

            LogHelper.WriteLog(xx);
            LogHelper.WriteLog(url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = xx.Length;
            try
            {
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                requestWriter.Write(xx);
                LogHelper.WriteLog("0");
                requestWriter.Close();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
                LogHelper.WriteLog(e.Message);
            }
            try
            {
                LogHelper.WriteLog("1");
                WebResponse webResponse = request.GetResponse();
                LogHelper.WriteLog("2");
                Stream webStream = webResponse.GetResponseStream();
                LogHelper.WriteLog("3");
                StreamReader responseReader = new StreamReader(webStream);
                response = responseReader.ReadToEnd();
                LogHelper.WriteLog(response);
                Console.Out.WriteLine(response);
                responseReader.Close();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
                LogHelper.WriteLog(e.Message);
            }
            //insertdata to database
            try
            {
                //  string json = "{\"responseCode\":\"000\",\"transAmount\":\"100000.00\",\"billReference1\":\"000\",\"billReference2\":\"002\"}";
                string json = response;

                CustomerPayment obj1 = JsonConvert.DeserializeObject<CustomerPayment>(json);
                connect = exmc.OpenConnection();
                command = connect.CreateCommand();
                command.Connection = connect;
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();

                string query = @"INSERT INTO db_receivepayment( BillerId, TransDate,TransTime,TermType,Amount,Reference1,Reference2,FromBank,FromName,approvalCode,RetryFlag)
                  VALUES (@BillerID, @TransDate,@TransTime,@TermType,@Amount,@Reference1,@Reference2,@FromBank,@FromName,@approvalCode,@RetryFlag);
            ";
                MySQLHelper Exm = new MySQLHelper();
                connect = Exm.OpenConnection();
                command = connect.CreateCommand();
                Trans = connect.BeginTransaction();
                command.Connection = connect;
                command.Parameters.Clear();
                command.CommandType = CommandType.Text;
                command.CommandText = query.ToLower();

                MySqlParameter BillerId = new MySqlParameter("@BillerId", obj1.BillerId);
                MySqlParameter Reference1 = new MySqlParameter("@Reference1", obj1.Reference1);
                MySqlParameter Reference2 = new MySqlParameter("@Reference2", obj1.Reference2);
                MySqlParameter TermType = new MySqlParameter("@TermType", obj1.TermType);
                MySqlParameter FromName = new MySqlParameter("@FromName", obj1.FromName);
                MySqlParameter FromBank = new MySqlParameter("@FromBank", obj1.FromBank);
                MySqlParameter TransTime = new MySqlParameter("@TransTime", obj1.TransTime);
                MySqlParameter Amount = new MySqlParameter("@Amount", Convert.ToDecimal(obj1.Amount));
                MySqlParameter approvalCode = new MySqlParameter("@approvalCode", obj1.ApprovalCode);
                MySqlParameter RetryFlag = new MySqlParameter("@RetryFlag", obj1.RetryFlag);
                MySqlParameter TransDate = new MySqlParameter("@TransDate", obj1.TransDate);


                command.Parameters.Add(BillerId);
                command.Parameters.Add(Reference1);
                command.Parameters.Add(Reference2);
                command.Parameters.Add(TermType);
                command.Parameters.Add(FromName);
                command.Parameters.Add(FromBank);
                command.Parameters.Add(TransTime);
                command.Parameters.Add(Amount);
                command.Parameters.Add(approvalCode);
                command.Parameters.Add(RetryFlag);
                command.Parameters.Add(TransDate);
                command.ExecuteNonQuery();

                LogHelper.WriteLog("insert success");
                // }
            }
            catch (Exception e2)
            {
                LogHelper.WriteLog("insert error massage : " + e2.Message);
            }
            //try
            //{

            //    string lb_ID = string.Empty;
            //    string lb_BillerID = string.Empty;
            //    string lb_MerchantID = string.Empty;
            //    string lb_Payment_Distict = string.Empty;
            //    string lb_Ref1 = string.Empty;
            //    string lb_Ref2 = string.Empty;
            //    string lb_Amount = string.Empty;
            //    string lb_NoPay = string.Empty;
            //    string lb_AmountPay = string.Empty;
            //    string lb_Status = string.Empty;
            //    string lb_InfoPM = string.Empty;
            //    string lb_CreateDate = string.Empty;

            //    MySQLHelper Ex = new MySQLHelper();
            //    DataSet DBset = new DataSet();
            //    string queryselect = string.Empty;
            //    queryselect = @"SELECT
            //        db_payment_hd.ID,
            //        db_payment_hd.BillerID,
            //        db_payment_hd.MerchantID,
            //        db_payment_hd.Payment_Distict,
            //        db_payment_hd.Ref1,
            //        db_payment_hd.Ref2,
            //        db_payment_hd.Amount,
            //        db_payment_hd.NoPay,
            //        db_payment_hd.AmountPay,
            //        db_payment_hd.`Status`,
            //        db_payment_hd.InfoPM,
            //        db_payment_hd.CreateDate,
            //        db_payment_hd.ModifyDate
            //        FROM
            //        db_payment_hd

            //    WHERE db_payment_hd.Ref1 = ";
            //    //+ obj1.billReference1;
            //    //                DataSet tdsdata = Ex.ExecuteQueryString(queryselect);
            //    //                ldamountrecrive = Convert.ToDecimal(objcus.Amount);
            //    //                ldamountpay = Convert.ToDecimal(tdsdata.Tables[0].Rows[0]["AmountPay"].ToString());
            //    //                ldamount = Convert.ToDecimal(tdsdata.Tables[0].Rows[0]["Amount"].ToString());
            //    //                linum = int.Parse(tdsdata.Tables[0].Rows[0]["NoPay"].ToString());
            //}
            //catch (Exception e2)
            //{
            //    LogHelper.WriteLog("insert error massage : " + e2.Message);
            //}
            return response;
        }

        public string IntradayQRPayment(string Type, string objcus)
        {
            string lsreturn = string.Empty;
            try
            {
                lsreturn = VerifyQRPayment(Type, objcus);
            }
            catch
            {
            }
            return lsreturn;
        }
        public string VerifyQRPayment(string BillerName2, string PeriodCode)
        {
            string P2 = string.Empty;
            string P3 = string.Empty;
            string P4 = string.Empty;
            string PeriodCode2 = string.Empty;
            try
            {

                MySQLHelper Ex = new MySQLHelper();
                DataSet DBset = new DataSet();
                string queryselect = string.Empty;
                queryselect = @" SELECT listofperiod.PeriodCode FROM listofperiod 
                WHERE TIME_FORMAT( now(),'%H:%i:%s') BETWEEN TIME_FORMAT(listofperiod.PeriodTimeStart,'%H:%i:%s')  AND TIME_FORMAT(listofperiod.PeriodTimeEnd,'%H:%i:%s') ";
                DataSet tdsdata = Ex.ExecuteQueryString(queryselect);
                if (tdsdata.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rt in tdsdata.Tables[0].Rows)
                    {
                        PeriodCode2 = rt["PeriodCode"].ToString();
                    }
                }
            }
            catch (Exception e2)
            {
                LogHelper.WriteLog("select error massage : " + e2.Message);
            }

            if (BillerName2.ToLower() == "NNQR")
            { BillerName2 = "010553102726422"; }
            else if (BillerName2.ToLower() == "YUPIN")
            { BillerName2 = "010553102726421"; }

            BBLPay objcus = new BBLPay();
            string lsreturn = string.Empty;
            string response = string.Empty;
            var url = @"https://pspaymentgateway.bangkokbank.com:2443/qrservice/api/rss/bbl/v1/transType/QRC_PYMT_TXN/transCode/INQUIRY_PPBP/clientId/NNQR/providerId/QRCGW";

            objcus.billerId = BillerName2;
            objcus.transDate = objcus.transDate;
            objcus.amount = objcus.amount;
            objcus.reference1 = objcus.reference1;
            objcus.reference2 = objcus.reference2;
            //  objcus.PeriodCode = "P01";
            var cli = new WebClient();
            //   cli.Headers[HttpRequestHeader.ContentType] = "application /json";
            string xx = "{\"billerId\":\"" + objcus.billerId + "\" , \"transDate\":\"" + objcus.transDate + "\", \"amount\":\"" + objcus.amount + "\" , \"reference1\":\"" + objcus.reference1 + "\", \"reference2\":\"" + objcus.reference2 + "\"}";
            //   string xx = "{\"customerId\":\"" + objcus.BillerNo + "\" , \"token\":\"" + objcus.QRId + "\"}";

            //LogHelper.WriteLog(xx);
            //LogHelper.WriteLog(url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = xx.Length;
            try
            {
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
                requestWriter.Write(xx);
                LogHelper.WriteLog("0");
                requestWriter.Close();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
                LogHelper.WriteLog(e.Message);
            }
            try
            {
                LogHelper.WriteLog("1");
                WebResponse webResponse = request.GetResponse();
                LogHelper.WriteLog("2");
                Stream webStream = webResponse.GetResponseStream();
                LogHelper.WriteLog("3");
                StreamReader responseReader = new StreamReader(webStream);
                response = responseReader.ReadToEnd();
                LogHelper.WriteLog(response);
                Console.Out.WriteLine(response);
                responseReader.Close();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(e.Message);
                LogHelper.WriteLog(e.Message);
            }
            //insertdata to database
            try
            {

                //    string json = "{\"responseCode\":\"000\",\"transAmount\":\"100000.00\",\"billReference1\":\"000\",\"billReference2\":\"002\"}";
                string json = response;
                string idbiller = objcus.billerId.ToString();
                OutputDABBL obj1 = JsonConvert.DeserializeObject<OutputDABBL>(json);

                //Loop thrrouch values and save the details into database
                foreach (var p in obj1.PTransaction)
                {


                    P4 = obj1.ResultCode.ToString();
                    P2 = p.PTransDate.ToString();
                    P3 = obj1.ResultDesc.ToString();

                    CustomerPayment objDiv = new CustomerPayment();
                    connect = exmc.OpenConnection();
                    command = connect.CreateCommand();
                    command.Connection = connect;
                    command.CommandType = CommandType.Text;
                    string sqlinsert = @"insert into db_modifypayment 
                        (ResultCode,ResultDesc,TransDate,PeriodTime,BankRef,AccountName,Ref1,Ref2,QRId,PayerName,PayerBank,Amount,Fee,PTransDate,Createdate)
                        values
                        (@ResultCode,@ResultDesc,@TransDate,@PeriodTime,@BankRef,@AccountName,@Ref1,@Ref2,@QRId,@PayerName,@PayerBank,@Amount,@Fee,@PTransDate,now())";
                    command.CommandText = sqlinsert.ToString();
                    command.Parameters.Clear();
                    MySqlParameter Parameter_ResultCode = new MySqlParameter("@ResultCode", obj1.ResultCode.ToString());
                    MySqlParameter Parameter_ResultDesc = new MySqlParameter("@ResultDesc", obj1.ResultDesc.ToString());
                    MySqlParameter Parameter_TransDate = new MySqlParameter("@TransDate", obj1.TransDate.ToString());
                    MySqlParameter Parameter_PeriodTime = new MySqlParameter("@PeriodTime", PeriodCode2);
                    MySqlParameter Parameter_BankRef = new MySqlParameter("@BankRef", p.BankRef.ToString());
                    MySqlParameter Parameter_AccountName = new MySqlParameter("@AccountName", obj1.BillerId.ToString());
                    MySqlParameter Parameter_Ref1 = new MySqlParameter("@Ref1", p.Reference1.ToString());
                    MySqlParameter Parameter_Ref2 = new MySqlParameter("@Ref2", p.Reference2.ToString());
                    MySqlParameter Parameter_PayerName = new MySqlParameter("@PayerName", p.PayerName.ToString());
                    MySqlParameter Parameter_PayerBank = new MySqlParameter("@PayerBank", p.PayerBank.ToString());
                    MySqlParameter Parameter_Amount = new MySqlParameter("@Amount", p.Amount.ToString());
                    MySqlParameter Parameter_Fee = new MySqlParameter("@Fee", p.Fee.ToString());
                    MySqlParameter Parameter_PTransDate = new MySqlParameter("@PTransDate", p.PTransDate.ToString());
                    //ทำการ Excute ข้อมูลเข้ามาในระบบ
                    command.Parameters.Add(Parameter_ResultCode);
                    command.Parameters.Add(Parameter_ResultDesc);
                    command.Parameters.Add(Parameter_TransDate);
                    command.Parameters.Add(Parameter_PeriodTime);
                    command.Parameters.Add(Parameter_BankRef);
                    command.Parameters.Add(Parameter_AccountName);
                    command.Parameters.Add(Parameter_Ref1);
                    command.Parameters.Add(Parameter_Ref2);
                    command.Parameters.Add(Parameter_PayerName);
                    command.Parameters.Add(Parameter_PayerBank);
                    command.Parameters.Add(Parameter_Amount);
                    command.Parameters.Add(Parameter_Fee);
                    command.Parameters.Add(Parameter_PTransDate);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception nn)
                    {

                    }
                    LogHelper.WriteLog("insert success");
                    try
                    {

                        objDiv = new CustomerPayment();
                        objDiv.BillerId = idbiller.ToString();
                        objDiv.Reference1 = p.Reference1.ToString();
                        objDiv.Reference2 = p.Reference2.ToString();
                        objDiv.TransDate = p.TransDate.ToString();
                        objDiv.FromName = p.PayerName.ToString();
                        objDiv.FromBank = p.PayerBank.ToString();
                        objDiv.Amount = p.Amount.ToString();
                        objDiv.ApprovalCode = obj1.ResultCode.ToString();
                        objDiv.RetryFlag = obj1.ResultDesc.ToString();
                        objDiv.TransDate = p.PTransDate.ToString();
                        response = JsonConvert.SerializeObject(objDiv);
                        //  RecivePaymentCopy(objDiv);
                        //RecivePayment(objDiv);
                    }
                    catch (Exception e2)
                    {
                        response = "insert error massage : " + e2.Message;
                    }

                }
            }
            catch (Exception e2)
            {
                CustomerPayment2 objDiv2 = new CustomerPayment2();
                if (P4 == "") { P4 = "001"; P3 = "Not found credit transaction"; }

                objDiv2.ResultCode = P4;
                objDiv2.ResultDesc = P3;
                objDiv2.TransDate = P2;
                response = JsonConvert.SerializeObject(objDiv2);

                CustomerPayment objDiv = new CustomerPayment();
                connect = exmc.OpenConnection();
                command = connect.CreateCommand();
                command.Connection = connect;
                command.CommandType = CommandType.Text;
                string sqlinsert = @"insert into db_modifypayment 
                        (ResultCode,ResultDesc,TransDate,PeriodTime,Createdate)
                        values
                        (@ResultCode,@ResultDesc,@TransDate,@PeriodTime,now())";
                command.CommandText = sqlinsert.ToString();
                command.Parameters.Clear();
                MySqlParameter Parameter_ResultCode = new MySqlParameter("@ResultCode", objDiv2.ResultCode.ToString());
                MySqlParameter Parameter_ResultDesc = new MySqlParameter("@ResultDesc", objDiv2.ResultDesc.ToString());
                MySqlParameter Parameter_TransDate = new MySqlParameter("@TransDate", P2);
                MySqlParameter Parameter_PeriodTime = new MySqlParameter("@PeriodTime", PeriodCode2);
                //ทำการ Excute ข้อมูลเข้ามาในระบบ
                command.Parameters.Add(Parameter_ResultCode);
                command.Parameters.Add(Parameter_ResultDesc);
                command.Parameters.Add(Parameter_TransDate);
                command.Parameters.Add(Parameter_PeriodTime);
                command.ExecuteNonQuery();

                //   LogHelper.WriteLog("");
            }

            return response;
        }
    }
}
