using Microsoft.Xrm.Sdk;
using ModuloSoa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ModuloSoa
{
    public class Builder
    {
        public static XmlDocument ArmaXMLCreate(string pais, string tipoDoc, string nroDoc, string user, string pass, string encoding, string created )
        {
            
            XmlDocument _TemplateXml = Helper.LoadXmlDocument("\\Requests\\Cliente.xml");
            string xmlDoc = _TemplateXml.InnerXml;

            xmlDoc = xmlDoc.Replace("us3r", user);
            xmlDoc = xmlDoc.Replace("passw0rd", pass);
            xmlDoc = xmlDoc.Replace("3nc0ding", encoding);
            xmlDoc = xmlDoc.Replace("cr3at3d", created);
            xmlDoc = xmlDoc.Replace("pai5", pais);
            xmlDoc = xmlDoc.Replace("tip0d0c", tipoDoc);
            xmlDoc = xmlDoc.Replace("numd0c", nroDoc);

            XmlDocument soapEnvelop = new XmlDocument();

            soapEnvelop.LoadXml(xmlDoc);

            return soapEnvelop;          
        }


        internal static void MappConfiguracionServiciosSoa(EntityCollection coll_bsv_configuracinserviciossoa, ref Dto.bsv_configuracinserviciossoa dto)
        {
            try
            {
                var ent = coll_bsv_configuracinserviciossoa.Entities[0];

                if (ent.Id != null && ent.Id != Guid.Empty)
                    dto.id = ent.Id;

                if (ent.Attributes.Contains("bsv_name"))
                    dto.bsv_name = ent.Attributes["bsv_name"].ToString();

                if (ent.Attributes.Contains("bsv_action"))
                    dto.bsv_action = ent.Attributes["bsv_action"].ToString();

                if (ent.Attributes.Contains("bsv_url"))
                    dto.bsv_url = ent.Attributes["bsv_url"].ToString();

                if (ent.Attributes.Contains("bsv_xml"))
                    dto.bsv_xml = ent.Attributes["bsv_xml"].ToString();

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        internal static void MappEnvioSoa(EntityCollection coll_bsv_enviosoa, ref List<Dto.bsv_enviosoa> list)
        {
            try
            {
                list = new List<Dto.bsv_enviosoa>();

                for (int i = 0; i < coll_bsv_enviosoa.Entities.Count(); i++)
                {
                    Dto.bsv_enviosoa env = new Dto.bsv_enviosoa();

                    var ent = coll_bsv_enviosoa.Entities[i];

                    if (ent.Id != null && ent.Id != Guid.Empty)
                        env.id = ent.Id;

                    if (ent.Attributes.Contains("bsv_name"))
                        env.bsv_name = ent.Attributes["bsv_name"].ToString();

                    if (ent.Attributes.Contains("bsv_atributo"))
                        env.bsv_atributo = ent.Attributes["bsv_atributo"].ToString();

                    if (ent.Attributes.Contains("bsv_consultasoa"))
                        env.bsv_consultasoa = ((EntityReference)(ent.Attributes["bsv_consultasoa"])).Id;

                    if (ent.Attributes.Contains("bsv_entityattribute"))
                        env.bsv_entityattribute = ent.Attributes["bsv_entityattribute"].ToString();

                    if (ent.Attributes.Contains("bsv_tipoatributo"))
                        env.bsv_tipoatributo =  ((OptionSetValue)(ent.Attributes["bsv_tipoatributo"]));

                    list.Add(env);

                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        internal static void MappRecepcionSoa(EntityCollection coll_bsv_recepcionsoa, ref List<Dto.bsv_recepcionsoa> list)
        {
            try
            {
                list = new List<Dto.bsv_recepcionsoa>();

                for (int i = 0; i < coll_bsv_recepcionsoa.Entities.Count; i++)
                {
                    Dto.bsv_recepcionsoa rec = new Dto.bsv_recepcionsoa();

                    var ent = coll_bsv_recepcionsoa.Entities[i];

                    if (ent.Id != null && ent.Id != Guid.Empty)
                        rec.id = ent.Id;

                    if (ent.Attributes.Contains("bsv_name"))
                        rec.bsv_name = ent.Attributes["bsv_name"].ToString();

                    if (ent.Attributes.Contains("bsv_ancho"))
                        rec.bsv_ancho = Convert.ToInt32(ent.Attributes["bsv_ancho"].ToString());

                    if (ent.Attributes.Contains("bsv_consultasoa"))
                        rec.bsv_consultasoa = ((EntityReference)(ent.Attributes["bsv_consultasoa"])).Id;

                    if (ent.Attributes.Contains("bsv_label"))
                        rec.bsv_label = ent.Attributes["bsv_label"].ToString();

                    if (ent.Attributes.Contains("bsv_orden"))
                        rec.bsv_orden = Convert.ToInt32(ent.Attributes["bsv_orden"].ToString());

                    list.Add(rec);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        internal static object ConstruyeXMLaEnviar(string sXml, List<Dto.bsv_enviosoa> list_envio_soa)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sXml);

            XmlReader xmlReader = new XmlNodeReader(xmlDoc);

            var doc = XElement.Load(xmlReader);

            foreach (var item in list_envio_soa)
            {
                doc.XPathSelectElement("//" + item.bsv_name).Add(item.valor);
            }

            return doc.ToString();
        }

        /// <summary>
        /// Se le pasa el XML y lo devuelve con el token de seguridad agregado.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        internal static void AgregaTokenXML(ref string xml)
        {
            Token _token = Helper.GenerateTokenWssUser(Properties.Settings.Default.user, Properties.Settings.Default.pass);

            var security = @" <soapenv:Header>
                               <wsse:Security soapenv:mustUnderstand='1' xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd' xmlns:wsu='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd'>
                                  <wsse:UsernameToken wsu:Id='UsernameToken-10'>
                                    <wsse:Username>" + _token.user + @"</wsse:Username>
                                    <wsse:Password Type='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText'>" +_token.pass+@"</wsse:Password>
                                    <wsse:Nonce EncodingType='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary'>"+_token.encoding+@"</wsse:Nonce>
                                    <wsu:Created>" + _token.created + @"</wsu:Created>
                                  </wsse:UsernameToken>
                                </wsse:Security>";

            xml = xml.Replace("<soapenv:Header>", security);
        }

        internal static System.Data.DataTable CompletaDTGrid(List<Dto.bsv_recepcionsoa> list_recepcion_soa, string result)
        {
            var dt = new System.Data.DataTable();

            foreach (var item in list_recepcion_soa)
            {
                dt.Columns.Add(item.bsv_name);
            }

            Helper.ReadXMLResponse(result, ref dt);

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var col_label = list_recepcion_soa.FirstOrDefault(x => x.bsv_name != null && x.bsv_name == dt.Columns[i].ColumnName.ToString()).bsv_label;
                
                dt.Columns[i].ColumnName = col_label != string.Empty ? col_label : dt.Columns[i].ColumnName;
            }

            return dt;
        }
    }
}