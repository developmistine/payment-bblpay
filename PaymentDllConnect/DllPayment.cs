using PaymentProject;
using PaymentProject.jsonstructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentDllConnect
{
    public class DllPayment
    {
        public DataSet GetPaymentConnect(PaymentJson ObjParam)
        {
            try
            {
                ReturnStatus Obj = new ReturnStatus();
                MySQLHelper ObjExcute = new MySQLHelper();
                DataSet TDS = new DataSet();

                string lsQuery = @" Select * From db_receivepayment Where BillerId = '" + ObjParam.BillerId + "' AND TransDate = '" + ObjParam.TransDate + "' AND Amount = '" + ObjParam.Amount + "' AND Reference1 = '" + ObjParam.Refernce1 + "' AND Reference2 = '" + ObjParam.Refernce2 + "' ";
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
