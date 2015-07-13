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
    public static class PersonaFisicaMapper
    {
        public static Contact GetContact(PersonaFisica personaFisica, EntityReference domicilioPais, EntityReference actividadLaboral, EntityReference banca, EntityReference pais, EntityReference localidad, EntityReference sucursal, EntityReference segmento, EntityReference provincia, EntityReference profesion)
        {
            var contact = new Contact();

            MapContact(personaFisica, domicilioPais, actividadLaboral, banca, pais, localidad, sucursal, segmento, provincia, profesion, contact);

            return contact;
        }

        public static void MapContact(PersonaFisica personaFisica, EntityReference domicilioPais, EntityReference actividadLaboral, EntityReference banca, EntityReference pais, EntityReference localidad, EntityReference sucursal, EntityReference segmento, EntityReference provincia, EntityReference profesion, Contact contact)
        {
            contact.bsv_Pais = pais;
            contact.bsv_tipo_de_documento = new OptionSetValue(personaFisica.TipoDocumentoId);
            contact.bsv_no_de_documento = personaFisica.NumeroDocumento;
            contact.bsv_cuil = personaFisica.CuitCuil;
            var nameParts = personaFisica.Nombre.Split(',');
            contact.FirstName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
            contact.LastName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            contact.GenderCode = new OptionSetValue(personaFisica.Sexo == 'M' ? 1 : 2);
            contact.BirthDate = personaFisica.FechaNacimiento;
            contact.EMailAddress1 = personaFisica.CorreoElectronico;
            contact.bsv_sucursal = sucursal;
            // TODO: Master EjecutivoCuenta
            //contact.bsv_Ejecutivo_de_ventas = personaFisica.EjecutivoCuentaId.ToString();
            // TODO: Master CanalVenta
            //contact.bsv_canal_de_ventas = personaFisica.CanalVentaId.ToString();
            contact.bsv_segmento = segmento;
            contact.bsv_fecha_alta_cliente = personaFisica.FechaAlta;
            contact.bsv_tipo_de_documento = new OptionSetValue(personaFisica.TipoDocumentoId);
            contact.bsv_domicilio_pais = domicilioPais;
            contact.Address1_Line1 = personaFisica.DomicilioCalle;
            contact.bsv_numero = personaFisica.DomicilioNumero.ToString();
            contact.bsv_dpto = personaFisica.DomicilioDepartamento;
            contact.Address1_PostalCode = personaFisica.DomicilioCodigoPostal;
            contact.bsv_localidad = localidad;
            contact.bsv_provincia = provincia;
            contact.bsv_tipo = new OptionSetValue(personaFisica.TipoTelefonoId ? 0 : 1);
            contact.Address1_Telephone1 = personaFisica.NumeroTelefonoPrincipal.ToString();
            // TODO: Set NumeroTelefonoLaboral
            //contact.??? = personaFisica.NumeroTelefonoLaboral.ToString();
            contact.bsv_estado_civil = new OptionSetValue(personaFisica.EstadoCivilId);
            contact.bsv_banca = banca;
            // TODO: Master ResponsableOficial
            //contact.bsv_oficial_responsable = personaFisica.ResponsableOficialId.ToString();
            contact.bsv_cliente = personaFisica.TipoClienteId != 0;
            contact.bsv_actividad_laboral = actividadLaboral;
            // TODO: Set IvaId
            //contact.??? = personaFisica.IvaId;
            contact.bsv_profesion = profesion;
            // TODO: Set OcupacionId
            //contact.??? = personaFisica.OcupacionId;
            // TODO: Set Titular
            //contact.??? = personaFisica.Titular;
        }
    }
}
