using Supervielle.DataAccess.Sql.Helpers;
using Supervielle.DataAccess.Sql.Interfaces;
using Supervielle.Domain.Enums;
using Supervielle.Domain.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * EJEMPLO CAJADEAHORRO
    //new Field("cta_id_empresa", 0, OriginType.Int),
    //new Field("cta_numero", 0, OriginType.Int),
    //new Field("ope_id_modulo", 0, OriginType.Int),
    //new Field("ope_id_moneda", 0, OriginType.Int),
    //new Field("ope_id_papel", 0, OriginType.Int),
    //new Field("ope_id_sucursal", 0, OriginType.Int),
    //new Field("ope_operacion", 0, OriginType.Int),
    //new Field("ope_sub_operacion", 0, OriginType.Int),
    //new Field("ope_id_tipo_operacion", 0, OriginType.Int),
    //new Field("per_id_pais", 0, OriginType.Int),
    //new Field("per_id_tipo_doc", 0, OriginType.Int),
    //new Field("per_nro_documento", 0, OriginType.String),
    //new Field("id_canal_venta", 0, OriginType.Int),
    //new Field("id_oficial", 0, OriginType.Int),
    //new Field("tipo_cuenta", 0, OriginType.String),
    //new Field("id_estado", 0, OriginType.Int),
    //new Field("embargos", 0, OriginType.String),
    //new Field("CBU", 0, OriginType.String),
    //new Field("cod_producto", 0, OriginType.Int),
    //new Field("id_paquete", 0, OriginType.Int),
    //new Field("marca_resumen_on_line", 0, OriginType.String),
    //new Field("tarjeta_debito_relacionada", 0, OriginType.String),
    //new Field("flg_plan_sueldo", 0, OriginType.Int),
    //new Field("bonificacion", 0, OriginType.String),
    //new Field("cod_promocion", 0, OriginType.Int),
    //new Field("convenio", 0, OriginType.Int),
    //new Field("tenencia_debito_cta", 0, OriginType.String),
    //new Field("cant_debito_cta", 0, OriginType.Int),
    //new Field("mora", 0, OriginType.String),
    //new Field("fecha_saldo_deudor", 0, OriginType.Int),
    //new Field("tasa_interes_asociada", 0, OriginType.Decimal),
    //new Field("comisionada", 0, OriginType.Int),
    //new Field("sobregiro", 0, OriginType.Int)
 * */
namespace Supervielle.DataAccess.Sql
{
    public class PersonaFisicaDao : GenericDao<PersonaFisica>, IPersonaFisicaDao
    {
        protected override string tableName
        {
            get { return "sta_crm_dcl_persona_fisica"; }
        }
    }
}
