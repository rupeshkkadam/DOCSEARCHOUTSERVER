using DOCSEARCHOUTSERVER.SearchLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DOCSEARCHOUTSERVER.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/
        public ActionResult Index(string ID)
        {
            DocSearch doc = new DocSearch();
            var model = doc.GetDocuments(ID);
            ViewBag.ID = ID;
            return View(model);
        }

        public JsonResult DownloadandShowPdf(string ID, string FileName)
        {
            var sucess = DownloadFile(System.Web.Configuration.WebConfigurationManager.AppSettings["FTPLocation"] + ID + "/", FileName, System.Web.Configuration.WebConfigurationManager.AppSettings["FTPUN"], System.Web.Configuration.WebConfigurationManager.AppSettings["FTPPWD"], Server.MapPath("~/Content/Download"));
            return Json(new { Downloaded = sucess }, JsonRequestBehavior.AllowGet);
        }
        private bool DownloadFile(string FtpUrl, string FileNameToDownload,
                        string userName, string password, string tempDirPath)
        {
            bool result = true;
            string ResponseDescription = "";
            string PureFileName = new FileInfo(FileNameToDownload).Name;
            string DownloadedFilePath = tempDirPath + "/" + PureFileName;
            string downloadUrl = String.Format("{0}/{1}", FtpUrl, FileNameToDownload);
            FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(downloadUrl);
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.Credentials = new NetworkCredential(userName, password);
            req.UseBinary = true;
            req.Proxy = null;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[2048];
                FileStream fs = new FileStream(DownloadedFilePath, FileMode.Create);
                int ReadCount = stream.Read(buffer, 0, buffer.Length);
                while (ReadCount > 0)
                {
                    fs.Write(buffer, 0, ReadCount);
                    ReadCount = stream.Read(buffer, 0, buffer.Length);
                }
                ResponseDescription = response.StatusDescription;
                fs.Close();
                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = false;
            }
            return result;
        }

        public ActionResult DownloadUsingFTPJSONNew(string ID, string FileName)
        {

            string FtpUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["FTPLocation"] + ID + "/";
            string FileNameToDownload = FileName;
            string userName = System.Web.Configuration.WebConfigurationManager.AppSettings["FTPUN"];
            string password = System.Web.Configuration.WebConfigurationManager.AppSettings["FTPPWD"];
            string ResponseDescription = "";
            string PureFileName = new FileInfo(FileNameToDownload).Name;
            string DownloadedFilePath = Server.MapPath("~/Content/Download/" + PureFileName);
            string downloadUrl = String.Format("{0}/{1}", FtpUrl, FileNameToDownload);
            FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(downloadUrl);
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.Credentials = new NetworkCredential(userName, password);
            req.UseBinary = true;
            req.Proxy = null;

            MemoryStream outputStream = new MemoryStream();

            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                Stream stream = response.GetResponseStream();
                byte[] buffer = new byte[2048];
                FileStream fs = new FileStream(DownloadedFilePath, FileMode.Create);
                int ReadCount = stream.Read(buffer, 0, buffer.Length);
                while (ReadCount > 0)
                {
                    fs.Write(buffer, 0, ReadCount);
                    ReadCount = stream.Read(buffer, 0, buffer.Length);
                }
                ResponseDescription = response.StatusDescription;
                fs.Close();
                stream.Close();

                if (System.IO.File.Exists(DownloadedFilePath))
                {
                    byte[] fileBytes = ReadFully(DownloadedFilePath);

                    outputStream.Write(fileBytes, 0, fileBytes.Length);
                    outputStream.Position = 0;

                    Response.ContentType = "application/pdf";
                    //Response.AddHeader("Content-Disposition", "inline;test.pdf");
                    Response.AddHeader("content-disposition", "inline; \"" + FileName + "\"");
                    Response.Buffer = true;
                    Response.Clear();
                    Response.OutputStream.Write(outputStream.GetBuffer(), 0, outputStream.GetBuffer().Length);
                    Response.OutputStream.Flush();
                    Response.End();
                    System.IO.File.Delete(DownloadedFilePath);
                    return new FileStreamResult(Response.OutputStream, "application/pdf");

                }
                else
                    return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private byte[] ReadFully(string input)
        {
            FileStream sourceFile = new FileStream(input, FileMode.Open); //Open streamer
            BinaryReader binReader = new BinaryReader(sourceFile);
            byte[] output = new byte[sourceFile.Length]; //create byte array of size file
            for (long i = 0; i < sourceFile.Length; i++)
                output[i] = binReader.ReadByte(); //read until done
            sourceFile.Close(); //dispose streamer
            binReader.Close(); //dispose reader
            return output;
        }
    }
}
