using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentClassBusiness.QRClass
{
    public class CreateFolder : Exception
    {

        /// <summary>
        /// ทำการ Create Folder
        /// </summary>
        /// <param name="lsfilename">ชื่อ File ที่ต้องการที่จะสร้าง</param>
        /// <returns>true เป็นตัวที่ทำการ Folder File </returns>
        public bool Create_Folder(string lsfilename)
        {
            try
            {
                // ทำการสร้าง FOLDER เพื่อที่จะทำการ 
                if (!Directory.Exists(lsfilename))  // ถ้ามีอยู่แล้วไม่ต้องสร้าง
                    Directory.CreateDirectory(lsfilename);
            }
            catch (Exception ex)
            {
                var w32ex = ex as Win32Exception;
                int code = 0;
                if (w32ex == null)
                {
                    w32ex = ex.InnerException as Win32Exception;
                }
                if (w32ex != null)
                {
                    code = w32ex.ErrorCode;
                    // do stuff
                }
                throw ex;
            }
            return true;
        }
    }
}
