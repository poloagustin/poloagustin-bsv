using Microsoft.Web.Services3.Security.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Text;
using Microsoft.Web.Services3.Xml;
using System.Xml;
using System.ServiceModel.Channels;
using ModuloSoa.Models;
using System.ServiceModel;
using DevExpress.Web.ASPxGridView;
using ModuloSoa.Dto;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Xrm.Sdk;


namespace ModuloSoa
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
            }
            else
            {
                string requestId = Request.QueryString["id"];
                string nombreConsulta = Request.QueryString["consulta"];

                ArmaConsultaGenerica(requestId, nombreConsulta);             
            }

        }

        private void ArmaConsultaGenerica(string requestId, string nombreConsulta)
        {
            lab_title_consulta.Text = nombreConsulta.ToUpper();

            var coll_bsv_configuracinserviciossoa = Helper.ReturnDataToCRM("bsv_configuracinserviciossoa", null, new List<string> { "bsv_name" }, new List<string> { "eq" }, new List<string> { nombreConsulta });

            if (coll_bsv_configuracinserviciossoa == null || coll_bsv_configuracinserviciossoa.Entities == null || coll_bsv_configuracinserviciossoa.Entities.Count == 0)
                return; //ToDo ! Agregar log?

            var coll_bsv_enviosoa = Helper.ReturnDataToCRM("bsv_enviosoa", null, new List<string> { "bsv_consultasoa" }, new List<string> { "eq" }, new List<string> { coll_bsv_configuracinserviciossoa[0].Id.ToString() });
            var coll_bsv_recepcionsoa = Helper.ReturnDataToCRM("bsv_recepcionsoa", null, new List<string> { "bsv_consultasoa" }, new List<string> { "eq" }, new List<string> { coll_bsv_configuracinserviciossoa[0].Id.ToString() });       
                
            bsv_configuracinserviciossoa configuracion_serviciossoa = new bsv_configuracinserviciossoa();
            List<bsv_enviosoa> list_envio_soa = new List<bsv_enviosoa>();
            List<bsv_recepcionsoa> list_recepcion_soa = new List<bsv_recepcionsoa>();
            
            Builder.MappConfiguracionServiciosSoa(coll_bsv_configuracinserviciossoa, ref configuracion_serviciossoa);
            Builder.MappEnvioSoa(coll_bsv_enviosoa, ref list_envio_soa);
            Builder.MappRecepcionSoa(coll_bsv_recepcionsoa, ref list_recepcion_soa);

            GetEntPadre(coll_bsv_configuracinserviciossoa[0].Id, requestId, ref list_envio_soa);

            if(list_envio_soa == null)
                return; //ToDo ! Agregar log?

            string xml = Builder.ConstruyeXMLaEnviar(configuracion_serviciossoa.bsv_xml, list_envio_soa).ToString();
            
            //Agrega token seguridad al XML
            Builder.AgregaTokenXML(ref xml);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            //Envia el XML a la Consulta SOA.
            var result = Helper.SendXMLtoCreate(xmlDoc, configuracion_serviciossoa.bsv_url, configuracion_serviciossoa.bsv_action);

            DataTable dt = Builder.CompletaDTGrid(list_recepcion_soa, result);

            ConstruyeGridUI(list_recepcion_soa);

            Grid.DataSource = dt;
            Grid.DataBind();   

        }

        private void GetEntPadre(Guid consultaId, string registroEntidadID, ref List<bsv_enviosoa> list)
        {
            string ent_name = string.Empty;
            EntityReference look = new EntityReference();

            try
            {
                var coll = Helper.ReturnDataToCRM("bsv_relacion_soa_entidad", new List<string> { "bsv_entidad" }, new List<string> { "bsv_consultasoa" }, new List<string> { "eq" }, new List<string> { consultaId.ToString() });

                if (coll != null && coll.Entities != null && coll.Entities.Count > 0 && coll[0].Attributes.Contains("bsv_entidad"))
                    look = ((EntityReference)(coll[0].Attributes["bsv_entidad"]));

                if (!(look != null && look.Name != null))
                {
                    list = null;
                    return;
                }

                ent_name = look.Name;

                var entidad_padre = Helper.ReturnDataToCRM(ent_name, null, new List<string> { ent_name + "id" }, new List<string> { "eq" }, new List<string> { registroEntidadID });

                if (entidad_padre == null || entidad_padre.Entities == null || entidad_padre.Entities.Count < 1)
                {
                    list = null;
                    return;
                }

                var ent = entidad_padre.Entities[0].Attributes;

                foreach (var item in list)
                {
                    if (ent.Contains(item.bsv_atributo))
                        item.valor = ConvertTypeToString(ent[item.bsv_atributo]);
                }

            }

            catch (Exception ex)
            {
                list = null;
                throw ex;
            }

            return;

        }

        private string ConvertTypeToString(object p)
        {

             switch (p.GetType().Name.ToLower())
            {
                 case "optionsetvalue":
                    {
                        return ((OptionSetValue)(p)).Value.ToString();
                    }
                 case "string":
                     {
                         return p.ToString();
                     }
                 case "int":
                 case "decimal":
                 case "float":
                     {
                         return p.ToString();
                     }
                 case "lookup":
                 case "entityreference":
                     {
                         var item = (EntityReference)(p);

                         if( item != null && item.Id != null && item.Name != null)
                         {
                             var coll = Helper.ReturnDataToCRM(item.LogicalName, new List<string> { "bsv_name" }, new List<string> { item.LogicalName + "id" }, new List<string> { "eq" }, new List<string> { item.Id.ToString() });

                             if(coll != null && coll.Entities != null && coll.Entities.Count>0)
                                return coll[0].Attributes["bsv_name"].ToString();
                         }

                         break;
                     }

                default:
                     break;
            }

             return string.Empty;
        }

        private void ConstruyeGridUI(List<bsv_recepcionsoa> _list)
        {            
            foreach (var item in _list)
            {
                if (Grid.Columns.IndexOf(Grid.Columns[item.bsv_label]) != -1)
                    return;

                GridViewDataTextColumn col = new GridViewDataTextColumn();

                col.FieldName = item.bsv_label;
                col.Name = item.bsv_label;
                col.Caption = item.bsv_label;
                col.Width = Unit.Parse(item.bsv_ancho.ToString());
                col.VisibleIndex = item.bsv_orden;
                col.Visible = true;
             
                Grid.Columns.Add(col);
                //  <settings showfilterrow="True" />
            }
        }    

    }
}