using Jsonstructure;
using PaymentProject;
using System;
using System.Data;

namespace PaymentClassConnect
{
    public class PaymentConnect
    {
        public DataSet GetPaymentConnect(PaymentJson ObjParam)
        {
            try
            {
                ReturnStatus Obj = new ReturnStatus();
                MySQLHelper ObjExcute = new MySQLHelper();
                DataSet TDS = new DataSet();

                string lsQuery = @"Select * From db_receivepayment Where BillerId = '" + ObjParam.BillerId + "' AND TransDate = '" + ObjParam.TransDate + "' AND Amount = '" + ObjParam.Amount + "' AND Reference1 = '" + ObjParam.Refernce1 + "' AND Reference2 = '" + ObjParam.Refernce2 + "' ";
                TDS = ObjExcute.ExecuteQueryString(lsQuery);

                return TDS;

            }
            catch (Exception ex)
            {
                ReturnStatus Obj = new ReturnStatus();
                Obj.ResponseCode = "341";
                Obj.ResponseMesg = "Service Provider not ready";
                throw ex;
            }

        }
    }
}
