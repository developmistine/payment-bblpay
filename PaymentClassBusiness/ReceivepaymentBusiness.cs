using Jsonstructure;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using VerifySlipClassConnect;
using PaymentProject;
using System;
using System.Collections.Generic;
using System.Data;

namespace PaymentClassBusiness
{
    public class SetCustomerPay
    {
        public ObjectReturn GetReceivepayment(CustomerPayment ObjParam)
        {
            // string lsreturn = string.Empty;
            try
            {
                ObjectReturn result = new ObjectReturn();
                PaymentConnect ObjExcute = new PaymentConnect();
                result = ObjExcute.RecivePayment(ObjParam);
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}


