using Microsoft.Xrm.Sdk;
using Supervielle.Domain.Crm;
using Supervielle.Domain.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.BusinessLogic.Mappers
{
    public static class PersonaJuridicaMapper
    {
        public static Account GetAccount(PersonaJuridica personaJuridica, EntityReference pais, EntityReference sucursal, EntityReference canalVenta, EntityReference segmento, EntityReference domicilioPais, EntityReference localidad, EntityReference provincia, EntityReference banca, EntityReference actividad)
        {
            var account = new Account();

            MapAccount(personaJuridica, account, pais, sucursal, canalVenta, segmento, domicilioPais, localidad, provincia, banca, actividad);

            return account;
        }

        public static void MapAccount(PersonaJuridica personaJuridica, Account account, EntityReference pais, EntityReference sucursal, EntityReference canalVenta, EntityReference segmento, EntityReference domicilioPais, EntityReference localidad, EntityReference provincia, EntityReference banca, EntityReference actividad)
        {
            account.bsv_pais = pais;
            account.bsv_tipo_de_documento = new OptionSetValue(personaJuridica.TipoDocumentoId);
            account.bsv_no_documento_cuit_cuil = personaJuridica.NumeroDocumento;
            account.bsv_no_documento_cuit_cuil = personaJuridica.CuitCuil;
            account.Name = personaJuridica.RazonSocial;
            account.EMailAddress1 = personaJuridica.CorreoElectronico;
            // TODO: Determine field for Contacto
            //account.bsv= personaJuridica.Contacto;
            account.bsv_sucursal = sucursal;
            // TODO: Determine field for EjecutivoCuenta
            //account.bsv_ejecutivo_de_ventas= personaJuridica.EjecutivoCuenta;
            account.bsv_canal_venta = canalVenta;
            account.bsv_segmento = segmento;
            account.bsv_fecha_de_alta_cliente = personaJuridica.FechaAlta;
            account.bsv_tipo = new OptionSetValue(Convert.ToInt32(personaJuridica.TipoDomicilioId));
            account.bsv_direccion_pais = domicilioPais;
            account.Address1_Line1 = personaJuridica.DomicilioCalle;
            account.bsv_numero = personaJuridica.DomicilioNumero.ToString();
            account.bsv_dpto = personaJuridica.DomicilioDepartamento;
            account.Address1_PostalCode = personaJuridica.DomicilioCodigoPostal;
            account.bsv_localidad = localidad;
            account.bsv_provincia = provincia;
            account.bsv_tipo = new OptionSetValue(personaJuridica.TipoTelefonoId ? 0 : 1);
            account.Address1_Telephone1 = personaJuridica.NumeroTelefonoPrincipal.ToString();
            account.bsv_banca = banca;
            // TODO: Determine field for OficialResponsable
            //account.bsv_= personaJuridica.OficialResponsableId;
            account.bsv_cliente = personaJuridica.TipoClienteId != 0;
            account.bsv_actividad_rubro = actividad;
            // TODO: Determine field for Iva
            //account.bsv_iva= personaJuridica.IvaId;
            // TODO: Determine field for Titular
            //account.= personaJuridica.Titular;
        }
    }
}
