using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace QRcodeGen
{
    public class GenQR
    {

        public void QRcodeGen(string path, string content, string strRep)
        {
            BarcodeWriter obj = new BarcodeWriter();
            obj.Options.Height = 330;
            obj.Options.Width = 330;
            obj.Format = BarcodeFormat.QR_CODE;
            obj.Write(content)
                .Save(path + strRep + ".bmp");
        }

    }
}
