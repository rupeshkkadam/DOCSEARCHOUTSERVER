using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;

namespace DOCSEARCHOUTSERVER.SearchLibrary
{
    public class DocSearch
    {
        public Search GetDocuments(string ID)
        {
            try
            {
                Search model = new Search(ID);
                //            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://ftp.miracompdev.com/web/content/Documents/Discipline/" + ID + "/");
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(System.Web.Configuration.WebConfigurationManager.AppSettings["FTPLocation"] + ID + "/");
                //          ftpRequest.Credentials = new NetworkCredential("appdev2015", "D0cF1nder");
                ftpRequest.Credentials = new NetworkCredential(System.Web.Configuration.WebConfigurationManager.AppSettings["FTPUN"], System.Web.Configuration.WebConfigurationManager.AppSettings["FTPPWD"]);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    model.docs.Add(line);
                    line = streamReader.ReadLine();
                }
                return model;
            }
            catch (Exception Ex)
            {
                return null;
            }
        }
    }
}