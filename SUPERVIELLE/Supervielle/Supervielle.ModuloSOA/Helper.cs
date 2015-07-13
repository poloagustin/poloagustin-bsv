using Accendo.DynamicsIntegration.Crm2015;
using Microsoft.Web.Services3.Security.Tokens;
using ModuloSoa.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;


namespace ModuloSoa
{
    public class Helper
    {
        public static CrmHelper instance = null;

        public static CrmHelper Connect()
        {
            try { 

            return new Accendo.DynamicsIntegration.Crm2015.CrmHelper();

                }
            catch(Exception ex)
            {
                throw;
            }
        }

        internal static string SendXMLtoCreate(XmlDocument xml, string _url, string _action)
        {  
            try
            {
                XmlDocument soapEnvelopeXml = xml;
                HttpWebRequest webRequest = CreateWebRequest(_url, _action);
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();

                // get the response from the completed web request.
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                    }
                }

                return soapResult;
            }

            catch (Exception ex)
            {
                throw new Exception("Fallo Consulta SOA - " + DateTime.Today.ToShortDateString());
            }


        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        internal static Models.ServiceSOA GetDataDirectionSOA(string nameSOA)
        {
            Models.ServiceSOA soa = new Models.ServiceSOA();

            try
            {
                XmlDocument xmlDoc = LoadXmlDocument("\\Configuration\\DirectionSOA.xml");

                XmlNodeList Consultas = (xmlDoc.SelectSingleNode("Consultas")).ChildNodes;

                foreach (XmlNode item in Consultas)
                {
                    XmlNodeList values = item.ChildNodes;
                    if (values[0].InnerText.ToString().ToUpper() == nameSOA.ToUpper())
                    {
                        soa.nombre = values[0].InnerText.ToString();
                        soa.page = values[1].InnerText.ToString();
                        soa.url = values[2].InnerText.ToString();
                        soa.action = values[3].InnerText.ToString();


                        return soa;
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public static XmlDocument LoadXmlDocument(string xmlPath)
        {
            string xmlPathEntero = HttpRuntime.AppDomainAppPath + xmlPath;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPathEntero);
            return xmlDoc;
        }

        internal static Token GenerateTokenWssUser(string Username, string Password)
        {
            //********************************** TOKEN *****************************************************
            UsernameToken token = new UsernameToken(Username, Password, PasswordOption.SendPlainText);

            XmlElement securityToken = token.GetXml(new XmlDocument());

            Token _token = new Token();

            _token.user = securityToken["wsse:Username"].InnerText;
            _token.pass = securityToken["wsse:Password"].InnerText;
            _token.encoding = securityToken["wsse:Nonce"].InnerText;
            _token.created = securityToken["wsu:Created"].InnerText;

            //*********************************************************************************************
            return _token;        
        }

        internal static void ReadXMLResponse(string result, ref DataTable dt)
        {
            //Response.ClienteResponse cliente = new Response.ClienteResponse();

            List<string> columns = new List<string>();

            for (int i = 0; i < dt.Columns.Count; i++)
                columns.Add(dt.Columns[i].ColumnName);


            XmlDocument _xml = new XmlDocument();           
            _xml.LoadXml(result);

            XmlNodeList nodo = _xml.GetElementsByTagName("Row");

            foreach (XmlElement item in nodo)
            {
                DataRow row = dt.NewRow();

                for (int i = 0; i < dt.Columns.Count; i++)
                    row[i] = item.GetElementsByTagName(dt.Columns[i].ColumnName)[0].InnerText;

                dt.Rows.Add(row);
            }

           /* cliente.nombreApellido = _xml.GetElementsByTagName("nombreApellido")[0].InnerText;
             cliente.descripcion = _xml.GetElementsByTagName("descripcion")[0].InnerText;
             cliente.codigo = _xml.GetElementsByTagName("codigo")[0].InnerText;*/

          //return cliente;
        }

        internal static Microsoft.Xrm.Sdk.EntityCollection ReturnDataToCRM(string entidad, List<string> atributos_retorno, List<string> condicion_atributo = null, List<string> condicion_operador = null, List<string> condicion_valor = null, bool filter_and = true, List<string> orden_atributo = null, List<bool> orden_descending = null)
        {
            try 
            { 
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='" + entidad + "'>";

            if(atributos_retorno != null)
                for (int i = 0; i < atributos_retorno.Count; i++)
                {
                    fetch += @"<attribute name='" + atributos_retorno[i] + "' />";
                }
            else
                fetch += @"<all-attributes/>";

            if(orden_atributo != null && orden_descending != null && orden_atributo.Count == orden_descending.Count)
                for (int i = 0; i < orden_atributo.Count; i++)
                {
                    fetch += @"<order attribute='" + orden_atributo[i] + "' descending='" + orden_descending[i] + "' />";
                }


            if (condicion_atributo != null && condicion_valor != null && condicion_operador != null && condicion_operador.Count > 0 && condicion_operador[0] == "in")
            {
                fetch += @"<filter type=" + (filter_and ? "'and'" : "'or'") + ">";
                fetch += "<condition attribute='" + condicion_atributo[0] + "' operator='" + condicion_operador[0] + "'>";

                for (int i = 0; i < condicion_atributo.Count; i++)
                {                    
                    fetch += @"<value>"+  condicion_valor[i]  +@"</value>";
                }
                fetch += "</condition>";
                fetch += @"</filter>";
            }
            else if(condicion_atributo != null && condicion_valor != null && condicion_operador != null && condicion_atributo.Count == condicion_valor.Count && condicion_atributo.Count == condicion_operador.Count)
            {
                    fetch += @"<filter type=" + (filter_and ? "'and'" : "'or'") + ">";

                    for (int i = 0; i < condicion_atributo.Count; i++)
                    {
                        fetch += "<condition attribute='"+ condicion_atributo[i] +"' operator='" + condicion_operador[i] +"' value='" + condicion_valor[i ]+ "' />";
                    }

                    fetch += @"</filter>";
            }

            fetch += @"</entity>
                        </fetch>";

            var _help = Helper.Connect();

            var coll = _help.Service.RetrieveMultiple(new Microsoft.Xrm.Sdk.Query.FetchExpression(fetch));
            
            return coll;

            }
            catch(Exception ex)
            {
                throw ex;
            }           
        }

        /// <summary>
        /// Se le pasa Fetch y devuelve la collection indicada
        /// </summary>
        /// <param name="fetch"></param>
        /// <returns></returns>
        internal static Microsoft.Xrm.Sdk.EntityCollection ReturnDataToCRM(string fetch)
        {
            try
            {
             var _help = Helper.Connect();

            var coll = _help.Service.RetrieveMultiple(new Microsoft.Xrm.Sdk.Query.FetchExpression(fetch));
            
            return coll;
            }
            catch(Exception ex)
            {
                throw ex;
            }           
        }
    }
}