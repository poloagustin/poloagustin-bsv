using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;



namespace ModuloSoa
{
    public partial class Home : System.Web.UI.Page
    {
        public Accendo.DynamicsIntegration.Crm2015.CrmHelper _helper = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
            }
            else
            {
                CargaListaConsultas(Request.QueryString["typename"]);                
            }
        }      

        private void Conection()
        {
            try
            {
                _helper = Helper.Connect();
            }

            catch (Exception)
            {                
                throw;
            }
        }


        private void ChequeaConexion()
        {
            if (_helper == null)
                Conection();
        }

        protected void CargaListaConsultas(string entidad)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                              <entity name='bsv_consulta_soa_por_entidad'>
                                <attribute name='bsv_consulta_soa_por_entidadid' />
                                <attribute name='bsv_name' />
                                <attribute name='createdon' />
                                <order attribute='bsv_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='bsv_name' operator='eq' value='"+entidad+@"' />
                                </filter>
                                <link-entity name='bsv_relacion_soa_entidad' from='bsv_entidad' to='bsv_consulta_soa_por_entidadid' alias='ac'>
                                  <link-entity name='bsv_configuracinserviciossoa' from='bsv_configuracinserviciossoaid' to='bsv_consultasoa' alias='ad'>
                                    <attribute name='bsv_name' />
                                  </link-entity>
                                </link-entity>
                              </entity>
                            </fetch>";

            EntityCollection coll = Helper.ReturnDataToCRM(fetch);

            if (!(coll != null && coll.Entities != null && coll.Entities.Count > 0))
                return;

            List<string> listConsultas = new List<string>();
            listConsultas.Add(string.Empty);
            
            foreach (var item in coll.Entities)
                listConsultas.Add(((AliasedValue)(item.Attributes["ad.bsv_name"])).Value.ToString());


            ddlConsultas.DataSource = listConsultas;
            ddlConsultas.DataBind();   
        }

        protected void ddlConsultas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlConsultas.SelectedItem.Text))
                return;

            string id = Request.QueryString["id"].Remove(0, 2);
            id = id.Remove(id.Length - 3, 3);

            var selected = "ConsultasGenericas.aspx" + "?id=" + id +"&consulta=" + ddlConsultas.SelectedItem.Text;

            try
            {
                Page.Response.Redirect(selected, false);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al intentar cargar la página");
            }
        }

    }
}