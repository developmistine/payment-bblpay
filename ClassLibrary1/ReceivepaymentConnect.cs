using Jsonstructure;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PaymentProject;
using System;
using System.Collections.Generic;
using System.Data;

namespace VerifySlipClassConnect
{
    public class PaymentConnect
    {
        // ประกาศตัวแปลในส่วนของการบันทึกข้อมูลเข้ามาในระบบ 
        MySqlConnection connect = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();
        MySqlDataAdapter adap = new MySqlDataAdapter();
        MySQLHelper exmc = new MySQLHelper();
        MySqlTransaction Trans = null;


        public ObjectReturn RecivePayment(CustomerPayment objcus)
        {
            decimal D_Amount = decimal.Zero;
            string D_status = string.Empty;
            decimal D_sumpay = decimal.Zero;
            // string lsreturn = string.Empty;
            string lsBankRef = string.Empty;
            string lsResCode = string.Empty;
            string lsResDesc = string.Empty;
            string lsTransDate = string.Empty;
            string query = string.Empty;
            string paystatus = string.Empty;
            decimal ldamountpay = 0;
            decimal ldamountrecrive = 0;
            decimal ldamount = 0;
            decimal ldamountupdate = 0;
            string lsstatus = string.Empty;
            string lsstusinfopm = string.Empty;
            string lsflagload = "NR";
            int linum = 0;
            // เป็นตัวแปรที่ดำเนินการเก็บค่าของข้อมูลระบบ ที่หลังที่ทำการ Excute  แล้วทำการ Return ออกมา เพื่อทำาการ ส่งข้อมูลมาทำการงาน 
            ObjectReturn result = new ObjectReturn();
            List<logCustomerPayment> objDiv = new List<logCustomerPayment>();
            lsflagload = "NR";
            LogHelper.WriteLog("' BillerNo = '" + objcus.BillerId + "' Reference1 = '" + objcus.Reference1 + "' Ref2 = '" + objcus.Reference2 + "' QRId = '" + objcus.FromBank + "' PayerName = '" + objcus.FromName + "' PayerBank = '" + objcus.TermType + "' Filler = '" + objcus.RetryFlag + "' Amount = '" + objcus.Amount + "' ResultCode = '" + objcus.ApprovalCode + "' ResultDesc = '" + objcus.TransTime + "' TransDate = '" + objcus.TransDate + "'");

            try
            {
                MySQLHelper ObjExcute = new MySQLHelper();
                MySqlConnection connect = new MySqlConnection();
                MySqlCommand command = new MySqlCommand();
                connect = exmc.OpenConnection();
                command = connect.CreateCommand();
                command.Connection = connect;
                command.Parameters.Clear();
                Trans = connect.BeginTransaction();

                string logquery = @"INSERT INTO db_logreceivepayment(  BillerId, 
                                                                TransDate,
                                                                TermType,
                                                                Amount,
                                                                Reference1,
                                                                Reference2,
                                                                FromBank,
                                                                FromName,
                                                                approvalCode,
                                                                RetryFlag
                                                                )
                                                                VALUES 
                                                                (
                                                                @BillerID, 
                                                                @TransDate,
                                                                @TermType,
                                                                @Amount,
                                                                @Reference1,
                                                                @Reference2,
                                                                @FromBank,
                                                                @FromName,
                                                                @approvalCode,
                                                                @RetryFlag
                                                               
                                                                )";

                string logqueryqr = string.Empty;
                command.Parameters.Clear();
                command.CommandType = CommandType.Text;
                command.CommandText = logquery.ToLower();

                MySqlParameter logBillerId = new MySqlParameter("@BillerId", objcus.BillerId);
                MySqlParameter logReference1 = new MySqlParameter("@Reference1", objcus.Reference1);
                MySqlParameter logReference2 = new MySqlParameter("@Reference2", objcus.Reference2);
                MySqlParameter logTermType = new MySqlParameter("@TermType", objcus.TermType);
                MySqlParameter logFromName = new MySqlParameter("@FromName", objcus.FromName);
                MySqlParameter logFromBank = new MySqlParameter("@FromBank", objcus.FromBank);
                MySqlParameter logAmount = new MySqlParameter("@Amount", Convert.ToDecimal(objcus.Amount));
                MySqlParameter logapprovalCode = new MySqlParameter("@approvalCode", objcus.ApprovalCode);
                MySqlParameter logRetryFlag = new MySqlParameter("@RetryFlag", objcus.RetryFlag);
                MySqlParameter logTransDate = new MySqlParameter("@TransDate", objcus.TransDate);


                command.Parameters.Add(logBillerId);
                command.Parameters.Add(logReference1);
                command.Parameters.Add(logReference2);
                command.Parameters.Add(logTermType);
                command.Parameters.Add(logFromName);
                command.Parameters.Add(logFromBank);
                command.Parameters.Add(logAmount);
                command.Parameters.Add(logapprovalCode);
                command.Parameters.Add(logRetryFlag);
                command.Parameters.Add(logTransDate);
                command.ExecuteNonQuery();
                Trans.Commit();
            }
            catch (Exception ex)
            {
                result = new ObjectReturn();
                // ประกาศตัวแประเพื่อรัย
                // result.BankRef = lsBankRef.Trim().ToString();
                result.responseCode = "052";
                result.responseMesg = "Bill ID or Service Code not register";
                // result.TransDate = lsTransDate.Trim().ToString();
                // ดำเนินการ Return โดยที่ทำการ Convert ออกมาเป็น File json
                // lsreturn = JsonConvert.SerializeObject(result);

            }


            try
            {
                MySQLHelper ObjExcute = new MySQLHelper();
                MySqlConnection connect = new MySqlConnection();
                MySqlCommand command = new MySqlCommand();
                DataSet tdsdatas = new DataSet();
                connect = exmc.OpenConnection();
                command = connect.CreateCommand();
                command.Connection = connect;
                command.Parameters.Clear();

                string Qrpay = string.Empty;
                Qrpay = @"SELECT COUNT(ID) as 'countRow' FROM `db_payment_hd` WHERE `Reference1` = '" + objcus.Reference1 + "' AND  `Reference2` = '" + objcus.Reference2 + "'";
                tdsdatas = ObjExcute.ExecuteQueryString(Qrpay);

                command.Connection = connect;
                command.Parameters.Clear();

                string sCountRow = tdsdatas.Tables[0].Rows[0]["countRow"].ToString();

                if (sCountRow == "0")
                {

                    DataSet dataset = new DataSet();

                    string Queryup = @"Select * From db_genqrpayment  Where Reference2 ='" + objcus.Reference2 + "' And Reference1 = '" + objcus.Reference1 + "'";
                    dataset = ObjExcute.ExecuteQueryString(Queryup);

                    command.Connection = connect;
                    command.Parameters.Clear();

                    if (dataset.Tables[0].Rows.Count > 0)
                    {
                        logCustomerPayment ObjDiv = new logCustomerPayment();
                        foreach (DataRow dr in dataset.Tables[0].Rows)
                        {
                            List<logCustomerPayment> Detail = new List<logCustomerPayment>();


                            ObjDiv.BillerId = dr["BillerId"].ToString();
                            ObjDiv.Reference1 = dr["Reference1"].ToString();
                            ObjDiv.Reference2 = dr["Reference2"].ToString();
                            ObjDiv.Amount = dr["Amount"].ToString();
                            ObjDiv.Status = dr["Status"].ToString();
                            ObjDiv.MerchantID = dr["MerchantID"].ToString();

                        }
                        try
                        {
                            string lsQuery = @"INSERT INTO db_payment_hd(
                                                                BillerId, 
                                                                TransDate, 
                                                                Amount, 
                                                                Reference1, 
                                                                Reference2,
                                                                Status,
                                                                MerchantID
                                                                )
                                                                VALUES
                                                                (
                                                                @BillerId,
                                                                @TransDate,
                                                                @Amount,
                                                                @Reference1,
                                                                @Reference2,
                                                                @Status,
                                                                @MerchantID
                                                                )";

                            string logqueryqr = string.Empty;
                            command.Parameters.Clear();
                            command.CommandType = CommandType.Text;
                            command.CommandText = lsQuery.ToLower();

                            MySqlParameter para_BillerID = new MySqlParameter("@BillerID", ObjDiv.BillerId);
                            MySqlParameter para_Amount = new MySqlParameter("@Amount", Convert.ToDecimal(ObjDiv.Amount));
                            MySqlParameter para_Ref1 = new MySqlParameter("@Reference1", ObjDiv.Reference1);
                            MySqlParameter para_Ref2 = new MySqlParameter("@Reference2", ObjDiv.Reference2);
                            MySqlParameter para_Status = new MySqlParameter("@Status", ObjDiv.Status);
                            MySqlParameter para_TransDate = new MySqlParameter("@TransDate", DateTime.Now);
                            MySqlParameter para_MerchantID = new MySqlParameter("@MerchantID", ObjDiv.MerchantID);


                            command.Parameters.Add(para_BillerID);
                            command.Parameters.Add(para_Amount);
                            command.Parameters.Add(para_Ref1);
                            command.Parameters.Add(para_Ref2);
                            command.Parameters.Add(para_Status);
                            command.Parameters.Add(para_TransDate);
                            command.Parameters.Add(para_MerchantID);
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex) { }

                    }


                }


                try
                {

                    MySQLHelper Ex = new MySQLHelper();
                    DataSet DBset = new DataSet();
                    string queryselect = string.Empty;
                    queryselect = @"select pay.Amount , pay.AmountPay,pay.`Status`,pay.InfoPM,pay.NoPay
                From db_payment_hd as pay
                WHERE Reference1 = " + objcus.Reference1;
                    DataSet tdsdata = Ex.ExecuteQueryString(queryselect);
                    ldamountrecrive = Convert.ToDecimal(objcus.Amount);
                    ldamountpay = Convert.ToDecimal(tdsdata.Tables[0].Rows[0]["AmountPay"].ToString());
                    ldamount = Convert.ToDecimal(tdsdata.Tables[0].Rows[0]["Amount"].ToString());
                    linum = int.Parse(tdsdata.Tables[0].Rows[0]["NoPay"].ToString());
                    linum = linum + 1;
                    // จ่ายครั้งแรกกรณีที่ยอดเงินน้อยกว่า  
                    ldamountupdate = ldamountpay + ldamountrecrive;
                    if (ldamountpay == 0 && ldamountrecrive < ldamount)
                    {

                        lsstatus = "PL";
                        lsstusinfopm = "NR";
                    }
                    // จ่ายครั้งแรก แต่จ่ายมากกว่าหรือเท่ากับ 
                    else if (ldamountpay == 0 && ldamountrecrive >= ldamount)
                    {
                        lsstatus = "AP";

                        if (ldamountrecrive == ldamount)
                        {
                            lsstusinfopm = "NR";
                        }
                        else if (ldamountrecrive > ldamount)
                        {
                            lsstusinfopm = "PM";
                        }
                    }
                    // จ่ายครั้งที่ 2 
                    else if (ldamountpay > 0 && ldamountpay >= ldamount)
                    {
                        decimal ldamountup = 0;
                        ldamountup = ldamountpay + ldamountrecrive;
                        if (ldamountup >= ldamountpay)
                        {
                            lsstatus = "PM";
                            lsstusinfopm = "PM";
                            lsflagload = "PM";
                        }

                    }
                    // จ่ายครั้งที่ 2 .... แต่ยอดเงินที่จ่ายมาก่อนหน้า น้อยกว่ายอดเงินที่ต้องจ่าย
                    else if (ldamountpay > 0 && ldamountpay <= ldamount)
                    {
                        ldamountpay = ldamountpay + ldamountrecrive;
                        if (ldamountpay >= ldamount)
                        {
                            lsstatus = "AP";
                            if (ldamountpay == ldamount)
                            {
                                lsstusinfopm = "NR";
                            }
                            else if (ldamountpay > ldamount)
                            {
                                lsstusinfopm = "PM";
                            }
                        }
                        else if (ldamountpay < ldamount)
                        {
                            lsstatus = "PL";
                            lsstusinfopm = "NR";
                        }
                    }


                }
                catch (Exception ex)
                {
                    result = new ObjectReturn();
                    // ประกาศตัวแประเพื่อรัย
                    //result.BankRef = lsBankRef.Trim().ToString();
                    result.responseCode = "052";
                    result.responseMesg = "Bill ID or Service Code not register";
                    //  result.TransDate = lsTransDate.Trim().ToString();
                    // ดำเนินการ Return โดยที่ทำการ Convert ออกมาเป็น File json
                 //   lsreturn = JsonConvert.SerializeObject(result);

                }


                // เป็นตัวแปรที่ได้จากการ Excute 
              //  lsreturn = string.Empty;
                lsBankRef = objcus.FromBank;
                result.responseCode = "052";
                result.responseMesg = "Bill ID or Service Code not register";
                lsTransDate = objcus.TransDate;
                try
                {
                    MySQLHelper Excute = new MySQLHelper();
                    DataSet tdsdata = new DataSet();
                    string queryselect = string.Empty;
                    queryselect = @"SELECT * FROM `db_receivepayment` WHERE `Reference1` = '" + objcus.Reference1 + "' AND `TransDate` = '" + objcus.TransDate + "' AND `Reference2` = '" + objcus.Reference2 + "' AND `Amount` = '" + objcus.Amount + "'";
                    tdsdata = Excute.ExecuteQueryString(queryselect);

                    //  CustomerPayment objDiv = new CustomerPayment();
                   // List<CustomerPayment> objDivlist = new List<CustomerPayment>();
                    //if (tdsdata.Tables[0].Rows.Count > 0)
                    //{
                    //    // ประกาศตัแปรเพื่อทำการรับข้อมูลเข้ามาจากนั้นก็ทำการ Return ออกไปเป็น Json
                    //  //  result = new ObjectReturn(); // ประกาศตัวแประเพื่อรัย
                    //    //   result.BankRef = lsBankRef.Trim().ToString();
                    //   // result.responseCode = "052";
                    //    //result.responseMesg = "Bill ID or Service Code not register";
                    //    //result.TransDate = lsTransDate.Trim().ToString();
                    //    // ดำเนินการ Return โดยที่ทำการ Convert ออกมาเป็น File json
                    //    //lsreturn = JsonConvert.SerializeObject(result);
                    //}
                       
                  
                        try
                        {
                            query = @"INSERT INTO db_receivepayment( BillerId, TransDate,TransTime,TermType,Amount,Reference1,Reference2,FromBank,FromName,approvalCode,RetryFlag)
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

                            MySqlParameter BillerId = new MySqlParameter("@BillerId", objcus.BillerId);
                            MySqlParameter Reference1 = new MySqlParameter("@Reference1", objcus.Reference1);
                            MySqlParameter Reference2 = new MySqlParameter("@Reference2", objcus.Reference2);
                            MySqlParameter TermType = new MySqlParameter("@TermType", objcus.TermType);
                            MySqlParameter FromName = new MySqlParameter("@FromName", objcus.FromName);
                            MySqlParameter FromBank = new MySqlParameter("@FromBank", objcus.FromBank);
                            MySqlParameter TransTime = new MySqlParameter("@TransTime", objcus.TransTime);
                            MySqlParameter Amount = new MySqlParameter("@Amount", Convert.ToDecimal(objcus.Amount));
                            MySqlParameter approvalCode = new MySqlParameter("@approvalCode", objcus.ApprovalCode);
                            MySqlParameter RetryFlag = new MySqlParameter("@RetryFlag", objcus.RetryFlag);
                            MySqlParameter TransDate = new MySqlParameter("@TransDate", objcus.TransDate);


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

                            string queryupdate_hd = string.Empty;
                            queryupdate_hd = @" UPDATE db_payment_hd SET  AmountPay=@AmountPay, `Status` =@Status, InfoPM = @InfoPM ,NoPay = @NoPay  WHERE Reference1 = @Reference1";

                            command.Parameters.Clear();
                            command.CommandType = CommandType.Text;
                            command.CommandText = queryupdate_hd.ToLower();
                            MySqlParameter _paraStatus = new MySqlParameter("@Status", lsstatus);
                            MySqlParameter _paraInfoPM = new MySqlParameter("@InfoPM", lsstusinfopm);
                            MySqlParameter _paraNoPay = new MySqlParameter("@NoPay", linum);
                            MySqlParameter _paraRef1 = new MySqlParameter("@Reference1", objcus.Reference1);
                            MySqlParameter _AmountPay = new MySqlParameter("@AmountPay", ldamountupdate);
                            command.Parameters.Add(_paraStatus);
                            command.Parameters.Add(_paraInfoPM);
                            command.Parameters.Add(_paraNoPay);
                            command.Parameters.Add(_paraRef1);
                            command.Parameters.Add(_AmountPay);
                            command.ExecuteNonQuery();


                            string logquery = @"UPDATE db_genqrpayment SET Status = @Status 
                                                                                Where 
                                                                                Reference1 =@Reference1
                                                                                and
                                                                                Reference2 =@Reference2";
                            string logqueryqrpay = string.Empty;
                            command.Parameters.Clear();
                            command.CommandType = CommandType.Text;
                            command.CommandText = logquery.ToLower();

                            MySqlParameter paraStatus = new MySqlParameter("@Status", "Y");
                            MySqlParameter paraRef1 = new MySqlParameter("@Reference1", objcus.Reference1);
                            MySqlParameter paraRef2 = new MySqlParameter("@Reference2", objcus.Reference1);

                            command.Parameters.Add(paraStatus);
                            command.Parameters.Add(paraRef1);
                            command.Parameters.Add(paraRef2);

                            command.ExecuteNonQuery();
                            //Trans.Commit();

                            result = new ObjectReturn();
                            // ประกาศตัวแประเพื่อรัย
                            //result.BankRef = lsBankRef.Trim();
                            result.responseCode = "000";
                            result.responseMesg = "Success";
                            // result.TransDate = lsTransDate.Trim();
                            // ดำเนินการ Return โดยที่ทำการ Convert ออกมาเป็น File json
                            //lsreturn = JsonConvert.SerializeObject(result);


                        }
                        catch (Exception ex)
                        {
                            Trans.Rollback();
                            result = new ObjectReturn();
                            // ประกาศตัวแประเพื่อรัย
                            // result.BankRef = lsBankRef.Trim().ToString();
                            result.responseCode = "052";
                            result.responseMesg = "Bill ID or Service Code not register";
                            // result.TransDate = lsTransDate.Trim().ToString();
                            // ดำเนินการ Return โดยที่ทำการ Convert ออกมาเป็น File json

                        }

                        finally
                        {
                            connect.Close();
                            MySqlConnection.ClearPool(connect);
                        }

                    

                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    result = new ObjectReturn();
                    // ประกาศตัวแประเพื่อรัย
                    // result.BankRef = lsBankRef.Trim().ToString();
                    result.responseCode = "052";
                    result.responseMesg = "Bill ID or Service Code not register";
                    // result.TransDate = lsTransDate.Trim().ToString();
                    // ดำเนินการ Return โดยที่ทำการ Convert ออกมาเป็น File json
                    //lsreturn = JsonConvert.SerializeObject(result);
                }



            }
            catch (Exception ex)
            {

            }


            return result;


        }


    }
}
