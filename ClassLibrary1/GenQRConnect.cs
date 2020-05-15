using iTextSharp.text;
using iTextSharp.text.pdf;
using Jsonstructure;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PaymentClassBusiness.QRClass;
using PaymentProject;
using QRcodeGen;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VerifySlipClassConnect
{
    public class GenQRConnect
    {
        // parameter เรียก method การเชื่อต่อข้อมูลและการใช้คำสั่งใน Database
        MySqlConnection connect = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();
        //MySqlTransaction Trans = null;                        // Trans ใช้ในกรณีทำงานร่วมมากกว่า 1 table ใน Database

        // method สร้าง QR code
        public string GenQRCode(QRDataPayment objcus)
        {
            string lsreturn = string.Empty;
            string lscrc = string.Empty;

            StringBuilder stradd = new StringBuilder();

            // Parameter หมายเลขเวอร์ชั่น ใช้ตามหลักสากล
            string P1_Payload_Format_Indicator = "000201";
            stradd.Append(P1_Payload_Format_Indicator);

            // Parameter ประเภทของ QR ใช้ตามหลักสากล
            string P2_Point_of_Initiation_method = "010212";
            stradd.Append(P2_Point_of_Initiation_method);

            // Parameter หมายเลขแอปพลิเคชั่น ใช้ตามหลักสากล
            string P3_ID = "00";
            string P3_Length = "16";
            string P3_Input_Value = "A000000677010112";
            string P3_AID = P3_ID + P3_Length + P3_Input_Value;

            // Parameter Ref 1 บัญชีร้านค้า (บริษัทBW)
            string P4_ID = "01";
            string P4_Length = (objcus.QRBillerID.ToString().Length).ToString("0#");
            string P4_Input_Value = objcus.QRBillerID.ToString();
            string P4_Biller_ID = P4_ID + P4_Length + P4_Input_Value;

            // Parameter Ref 2 ตัวเลขRefอ้างอิงที่1
            string P5_ID = "02";
            string P5_Length = (objcus.QRReference1.ToString().Length).ToString("0#");
            string P5_Input_Value = objcus.QRReference1.ToString();
            string P5_Reference1 = P5_ID + P5_Length + P5_Input_Value;

            // Parameter Ref 3 ตัวเลขRefอ้างอิงที่2
            string P6_ID = "03";
            string P6_Length = (objcus.QRReference2.ToString().Length).ToString("0#");
            string P6_Input_Value = objcus.QRReference2.ToString();
            string P6_Reference2 = P6_ID + P6_Length + P6_Input_Value;


            // Parameter ข้อมูลผู้ขาย 
            string P7_ID = "30";
            StringBuilder str = new StringBuilder();
            str.Append(P3_AID);
            str.Append(P4_Biller_ID);
            str.Append(P5_Reference1);
            str.Append(P6_Reference2);
            string m = P3_AID + P4_Biller_ID + P5_Reference1 + P6_Reference2;
            string P7_Length_2 = (m.Length.ToString());
            string P7_Merchant_identifier = P7_ID + P7_Length_2 + str.ToString();
            stradd.Append(P7_Merchant_identifier);

            // Parameter สกุลเงิน "53" ใช้ตามหลักสากล
            string P8_TransactionCurrencyCode = "5303764";
            stradd.Append(P8_TransactionCurrencyCode);

            // Parameter จำนวนเงิน "54" ใช้ตามหลักสากล

            //เอา comma ออก
            string lsamountreplace = objcus.QRAmount.ToString();
            lsamountreplace = lsamountreplace.Replace(",", string.Empty);

            string P9_ID = "54";
            string P9_Length = (lsamountreplace.Length).ToString("0#");
            string P9_Input_Value = lsamountreplace.ToString();
            string P9_TransactionAmount = P9_ID + P9_Length + P9_Input_Value;
            stradd.Append(P9_TransactionAmount);

            // Parameter ประเทศ "58" ใช้ตามหลักสากล
            string P10_CountryCode = "5802TH";
            stradd.Append(P10_CountryCode);

            // Parameter ชื่อผู้ขาย "58" ใช้ตามหลักสากล 
            string P11_ID = "59";
            string P11_Length = (objcus.QRMerchantID.ToString().Length).ToString("0#");
            string P11_Input_Value = objcus.QRMerchantID.ToString();
            string P11_MerchantName = P11_ID + P11_Length + P11_Input_Value;
            stradd.Append(P11_MerchantName);

            // Parameter ข้อมูลเพิ่มเติม โดย code นี้ นำเอา ชื่อผู้ขาย บวกกับ Ref
            string P13_ID2 = "07";
            string P13_Input_Value2 = "D" + objcus.QRMerchantID.ToString() + P5_Input_Value;
            string P13_Length2 = (P13_Input_Value2.ToString().Length).ToString("0#");
            string P13_TerminalID2 = P13_ID2 + P13_Length2 + P13_Input_Value2;
            string P13_Input_Value_2 = P13_TerminalID2;
            string P12_ID = "62";
            string P12_Length = (P13_Input_Value_2.ToString().Length).ToString("0#");
            string P12_Input_Value = P12_ID + P12_Length;
            //stradd.Append(P12_Input_Value);
            string P13_ID = "07";
            string P13_Input_Value = "D" + objcus.QRMerchantID.ToString() + P5_Input_Value;
            string P13_Length = (P13_Input_Value.ToString().Length).ToString("0#");
            string P13_TerminalID = P13_ID + P13_Length + P13_Input_Value;
            //stradd.Append(P13_TerminalID);

            // Code CheckSum ใช้ตามหลักสากล
            string P14_CheckSum = "6304";
            stradd.Append(P14_CheckSum);


            lsreturn = stradd.ToString();
            string lscodetext = string.Empty;
            QRCodeFormat genqr = new QRCodeFormat();

            try
            {
                // โค้ด GenCheckSum ขอบังคับการGenQR
                byte[] bytes = Encoding.ASCII.GetBytes(lsreturn);
                Crc16Ccitt Test = new Crc16Ccitt(InitialCrcValue.NonZero1);
                byte[] returnt = Test.CodeChecksumBytes(bytes, ref lscrc);
                //string s2 = BitConverter.ToString(bytes);

                if (lscrc.Length == 3)
                {
                    lscrc = '0' + lscrc;
                }
                // ทำการ Return QR Code ออกมากจากระบบ 
                genqr.QRqrcode = lsreturn + lscrc;
                lscodetext = genqr.QRqrcode;
                string url_qrcode = GenQRCodePaymentPath(genqr.QRqrcode, P4_Input_Value, P5_Input_Value, P9_Input_Value, P11_Input_Value, objcus.QRReference1);


                try
                {
                    // Code เชื่อมต่อ Database
                    #region 
                    MySQLHelper Exmc = new MySQLHelper();       // parameter เรียกเซิฟเวอร์
                    string query = string.Empty;
                    connect = Exmc.OpenConnection();            // ทำการเปิดการเชื่อมต่อเซิฟเวอร์
                    command = connect.CreateCommand();          // ทำการสร้างการเชื่อมต่อชุดคำสั่งที่จะนำไปใช้ใน Database
                    //Trans = connect.BeginTransaction();       // ทำการเชื่อต่อเซิฟเวอร์
                    command.Connection = connect;               // ทำการเชื่อมต่อ code ชุดคำสั่ง กับ Database
                    string QR_Status = "N";                     // Parameter ที่สร้างขึ้นเอง ไม่ใช่ parameter ที่มากับ JSON


                    // ชุดคำสั่งป้อนข้อมูลลงไปใน Table
                    query = @"
                               INSERT INTO db_genqrpayment(MerchantID,BillerID, TransDate, Amount, Reference1, Reference2,Status)
                               VALUES (@MerchantID,@BillerID,@TransDate,@Amount,@Reference1,@Reference2,@Status)";


                    command.Parameters.Clear();                     // ทำการลบข้อมูลเก่าก่อนที่จะทำการป้อนข้อมูลชุดใหม่ลงไปในชุดคำสั่ง
                    command.CommandType = CommandType.Text;         // ทำการเปลี่ยนชุดคำสั่งเป็นชนิด string(text)
                    command.CommandText = query.ToLower();          // เปลี่ยนข้อมูลที่เข้าเป็นตัวอักษรพิมเล็ก

                    // parameter เรียกข้อมูล
                    MySqlParameter para_QRMerchantIDD = new MySqlParameter("@MerchantID", objcus.QRMerchantID);
                    MySqlParameter para_BillerID = new MySqlParameter("@BillerID", objcus.QRBillerID);
                    MySqlParameter para_Amount = new MySqlParameter("@Amount", Convert.ToDecimal(objcus.QRAmount));
                    MySqlParameter para_Ref1 = new MySqlParameter("@Reference1", objcus.QRReference1);
                    MySqlParameter para_Ref2 = new MySqlParameter("@Reference2", objcus.QRReference2);
                    MySqlParameter para_Status = new MySqlParameter("@Status", QR_Status);
                    MySqlParameter para_TransDate = new MySqlParameter("@TransDate", DateTime.Now);

                    // code ที่ทำการป้อนข้อมูล
                    command.Parameters.Add(para_QRMerchantIDD);
                    command.Parameters.Add(para_BillerID);
                    command.Parameters.Add(para_Amount);
                    command.Parameters.Add(para_Ref1);
                    command.Parameters.Add(para_Ref2);
                    command.Parameters.Add(para_Status);
                    command.Parameters.Add(para_TransDate);

                    // run code 
                    command.ExecuteNonQuery();


                    //Trans.Commit();                                   // ทำการตัดการเชื่อมต่อ
                    genqr = new QRCodeFormat();
                    genqr.QRresponseCode = "000";
                    genqr.QRqrcode = url_qrcode;
                    genqr.QRcode = lscodetext;
                    lsreturn = JsonConvert.SerializeObject(genqr);
                    #endregion
                }
                catch (Exception ex)
                {
                    genqr = new QRCodeFormat();
                    // ทำการ Return QR Code ออกมากจากระบบ 
                    genqr.QRqrcode = "999";
                    genqr.QRqrcode = ex.ToString();
                    lsreturn = JsonConvert.SerializeObject(genqr);
                    //Trans.Rollback();                                     
                }

                finally
                {
                    connect.Close();
                    MySqlConnection.ClearPool(connect);
                }



            }
            catch (Exception ex)
            {
                genqr = new QRCodeFormat();
                // ทำการ Return QR Code ออกมากจากระบบ 
                genqr.QRqrcode = "999";

                genqr.QRqrcode = ex.ToString();
                lsreturn = JsonConvert.SerializeObject(genqr);

            }

            return lsreturn;
        }

        // method บันทึกและส่ง code QR ไปตาม path ต่างๆ
        private string GenQRCodePaymentPath(string lsbitmat, string Bill_id, string P5_Input_Value, string P9_Input_Value, string P11_Input_Value, string strRep)
        {
            CreateFolder cx = new CreateFolder();               //parameter เรียก method สร้างโฟรเดอร์ ปล.ไม่สามารถสร้างโฟรเดอร์ผ่านโค้ดได้ในเซิฟเวอร์จริง ต้องสร้างโฟร์เดอร์เอง เนื่องจากติดสิทธิ์การใช้งาน
            string lsWidth = string.Empty;
            string pathsave = string.Empty;
            string pathsave2 = string.Empty;
            string pathsave3 = string.Empty;
            string lsHeight = string.Empty;
            string lspath = string.Empty;
            string lsdate = string.Empty;
            string path = string.Empty;

            try
            {






                //
                lsdate = DateTime.Now.ToString("yyyyMMdd");
                path = @"C:\inetpub\wwwroot\PaymentBBL\IMG\" + lsdate + @"\";

                if (!Directory.Exists(path))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }


                GenQR obj = new GenQR();
                obj.QRcodeGen(path, lsbitmat, strRep);
                Bitmap bitmap = new Bitmap(path + strRep + ".bmp");

                // code รูป QR 

                string imageURL3_logo = @"C:\inetpub\wwwroot\PaymentBBL\Thai_QR_Payment_Logo-031.png";          // path รูปโลโก้
                System.Drawing.Image logo = System.Drawing.Image.FromFile(imageURL3_logo);


                // แก้ไข path ทุกครั้งที่มีการนำ re-coding ไปใช้
                pathsave = @"C:\inetpub\wwwroot\PaymentBBL\";
                //string s_date = DateTime.Now.ToString("yyyyMMdd", new CultureInfo("en-US"));
                string s_date1 = DateTime.Now.ToString("hhmmss", new CultureInfo("en-US"));
                string PathFolder = pathsave + @"IMG\" + lsdate + "\\";
                string PathFolder2 = pathsave + @"PDF\" + lsdate + "";
                string PathFolder3 = pathsave + @"IMGQR\" + lsdate + "";
                pathsave = PathFolder + @"\" + P5_Input_Value + s_date1 + ".jpg";
                pathsave2 = PathFolder2 + @"\" + P5_Input_Value + s_date1 + ".pdf";
                pathsave3 = PathFolder3 + @"\" + P5_Input_Value + s_date1 + ".jpg";
                string pathsave_url = pathsave.Substring(2);

                //แก้ไข path เชื่อมต่อเซิฟเวอร์ ทุกครั้งที่มีการนำ re-coding ไปใช้
                string url_promptpay = @"https:" + "//" + "bblpayment.ningnongshoppingthailand.com" + @"/PaymentBBL/IMGQR/" + lsdate + "/" + P5_Input_Value + s_date1 + ".jpg";
                //string url_promptpay = @"devapp.ningnongmistine.com" + @"\PaymentBBL\IMGQR\" + s_date + "\\" + P5_Input_Value + s_date1 + ".jpg";
                string namefile = P5_Input_Value + s_date1 + ".jpg";
                cx.Create_Folder(PathFolder);
                FileInfo file = new FileInfo(pathsave);
                //img.Save(pathsave);

                // ทำการนำโลโก้มาใส่ในรูปQR
                using (bitmap)
                {
                    Graphics g = Graphics.FromImage(bitmap);

                    // ปรับตำแหน่งโลโก้ QR ใช้ กราฟ X Y 
                    int left = (bitmap.Width - (logo.Width - 42)) / 2;
                    int top = (bitmap.Height - (logo.Height - 31)) / 2;
                    g.DrawImage(logo, new Point(left + 12, top + 6));         //155 155 จุดกึ่งกลางQR ขนาด 330*330 ของโลโก้ BBL

                    bitmap.Save(pathsave);
                }


                //เอารูป qrcode มาแปลงเป็น pdf เพิ่มหัวกับท้าย
                convertQRtoPDF(pathsave, pathsave2, PathFolder2, P9_Input_Value, P5_Input_Value, PathFolder3, pathsave3, P11_Input_Value);
                //เอารูป qrcode pdf  มาแปลงเป็น jpg
                convertPDFTOJPG(pathsave, pathsave2, PathFolder2, P9_Input_Value, P5_Input_Value, PathFolder3, pathsave3, namefile);

                lspath = url_promptpay;

            }
            catch (Exception ex)
            {
                lspath = string.Empty;
            }

            return lspath;


        }

        // method การเปลี่ยนชนิดข้อมูลและบันทึก
        static string convertQRtoPDF(string pathsave, string pathsave2, string PathFolder2, string P9_Input_Value, string P5_Input_Value, string PathFolder3, string pathsave3, string P11_Input_Value)
        {
            string result = "12";
            CreateFolder cx = new CreateFolder();
            cx.Create_Folder(PathFolder2);
            cx.Create_Folder(PathFolder3);

            /// แก้ไข path สำหรับอักษร ตรวจเช็คโฟรเดอร์ต้องมีตามที่ coding ไว้เสมอ
            string pathfont_1 = @"C:\inetpub\wwwroot\PaymentBBL\font";
            string pathfont = pathfont_1 + @"\THSarabun.ttf";
            string pathfont2 = pathfont_1 + @"\THSarabun Bold.ttf";
            DateTime dt = DateTime.Now;
            String strDate = string.Empty;
            strDate = dt.ToString("dd");   //Saturday, 21 July 2007
            String strMM = string.Empty;
            strMM = dt.ToString("MM");
            String strYY = string.Empty;
            strYY = dt.AddYears(543).ToString("yyyy");
            if (strMM == "01") strMM = "มกราคม";
            else if (strMM == "02") strMM = "กุมภาพันธ์";
            else if (strMM == "03") strMM = "มีนาคม";
            else if (strMM == "04") strMM = "เมษายน";
            else if (strMM == "05") strMM = "พฤษภาคม";
            else if (strMM == "06") strMM = "มิถุนายน";
            else if (strMM == "07") strMM = "กรกฎาคม";
            else if (strMM == "08") strMM = "สิงหาคม";
            else if (strMM == "09") strMM = "กันยายน";
            else if (strMM == "10") strMM = "ตุลาคม";
            else if (strMM == "11") strMM = "พฤศจิกายน";
            else if (strMM == "12") strMM = "ธันวาคม";
            #region Print PDF
            try
            {
                // 
                FileInfo file = new FileInfo(PathFolder2);
                // if (!file.Exists)
                {
                    // Step 1: Creating System.IO.FileStream object
                    using (FileStream fs = new FileStream(pathsave2, FileMode.Create, FileAccess.Write, FileShare.None))
                    // Step 2: Creating iTextSharp.text.Document object
                    using (Document doc = new Document(PageSize.A6, 20, 20, 20, 20))
                    // Step 3: Creating iTextSharp.text.pdf.PdfWriter object
                    // It helps to write the Document to the Specified FileStream
                    using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                    {
                        // Step 4: Openning the Document
                        doc.Open();
                        BaseFont baseFont = BaseFont.CreateFont(pathfont, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, true);
                        BaseFont baseFont2 = BaseFont.CreateFont(pathfont2, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, true);
                        string MerchantID = string.Empty;
                        if (P11_Input_Value == "YUI") MerchantID = "YUPIN";
                        else if (P11_Input_Value == "DSM") MerchantID = "DSM";
                        else if (P11_Input_Value == "NIN") MerchantID = "NING NONG";
                        else MerchantID = "";
                        try
                        {
                            Paragraph paragraph = new Paragraph();

                            string imageURL = @"C:\inetpub\wwwroot\PaymentBBL\promptpayBBL.jpg";
                            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
                            //Resize image depend upon your need
                            jpg.ScaleToFit(180f, 160f);
                            //Give space before image
                            jpg.SpacingBefore = 10f;
                            //Give some space after the image
                            jpg.SpacingAfter = 1f;
                            jpg.Alignment = Element.ALIGN_CENTER;
                            doc.Add(paragraph);
                            doc.Add(jpg);


                        }
                        catch (Exception ex)
                        {
                        }


                        Paragraph paragraph5 = new Paragraph();
                        paragraph5.Font.Size = 60;
                        paragraph5.Add(new Paragraph("                             "));
                        paragraph5.Add(new Paragraph("                            Batch No :  " + P5_Input_Value, new iTextSharp.text.Font(baseFont2)));
                        paragraph5.Add(new Paragraph("                            วันที่  " + strDate + " " + strMM + " " + strYY, new iTextSharp.text.Font(baseFont2)));
                        paragraph5.Add(new Paragraph("                            จำนวนเงิน  :  " + P9_Input_Value + "  บาท", new iTextSharp.text.Font(baseFont2)));
                        doc.Add(paragraph5);

                        try
                        {
                            Paragraph paragraph2 = new Paragraph();
                            string imageURL2 = pathsave;
                            iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(imageURL2);
                            //Resize image depend upon your need
                            jpg2.ScaleToFit(240f, 220f);
                            //Give space before image
                            jpg2.SpacingBefore = 20f;
                            //Give some space after the image
                            jpg2.SpacingAfter = 2f;
                            jpg2.Alignment = Element.ALIGN_CENTER;
                            doc.Add(paragraph2);
                            doc.Add(jpg2);


                        }
                        catch (Exception ex)
                        {
                        }




                    }
                }
            }
            catch (Exception Ex_pdf)
            {
            }
            #endregion
            return result;
        }

        // method  การเปลี่ยนชนิดข้อมูลและบันทึก
        static string convertPDFTOJPG(string pathsave, string pathsave2, string PathFolder2, string P9_Input_Value, string P5_Input_Value, string PathFolder3, string pathsave3, string namefile)
        {
            string result = "";
            try
            {

                //Pdf file
                String file = pathsave2;
                //Open pdf document
                Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                doc.LoadFromFile(file);

                //Save to images
                for (int i = 0; i < doc.Pages.Count; i++)
                {
                    String fileName = String.Format(PathFolder3 + '\\' + namefile, i);
                    using (System.Drawing.Image image = doc.SaveAsImage(i, 300, 300))
                    {
                        image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //System.Diagnostics.Process.Start(fileName);
                    }
                }

                doc.Close();


            }

            catch (Exception Ex_pdf)
            {

            }
            return result;
        }


    }
}
