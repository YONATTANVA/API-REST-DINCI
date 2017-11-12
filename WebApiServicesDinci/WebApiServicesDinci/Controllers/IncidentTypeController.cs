using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Configuration;
using WebApiServicesDinci.Models;
using System.Data;


namespace WebApiServicesDinci.Controllers
{
    public class IncidentTypeController : ApiController
    {

        #region "Variables"
        SqlConnection Conexion = new SqlConnection();
        SqlCommand ComandoSql = new SqlCommand();
        SqlTransaction Transaccion = null;

        public string SqlError = string.Empty;//Almacena la cadena del mensaje de error

        string str_conexion = string.Empty;//ConfigurationManager.ConnectionStrings["CON"].ConnectionString;//
        #endregion

        #region "Metodos"

        public void AddParametro(string Nombre, SqlDbType Tipo, object Valor)
        {
            ComandoSql.Parameters.Add(Nombre, Tipo);
            ComandoSql.Parameters[Nombre].Value = Valor;
        }
        public void AddStore(string NombreStore)
        {
            ComandoSql.CommandType = CommandType.StoredProcedure;
            ComandoSql.CommandText = NombreStore;
        }

        //-------------------------------------------------------------------------------------------------------
        // Establece Conexión
        //-------------------------------------------------------------------------------------------------------
        public bool SqlConexion(bool Inicia_Transac)
        {
            bool zapertura = false;
            SqlError = string.Empty;
            str_conexion = ConfigurationManager.ConnectionStrings["CON"].ConnectionString;
            try
            {
                Conexion = new SqlConnection(str_conexion);
                Conexion.Open();
                ComandoSql.Connection = Conexion;
                ComandoSql.CommandTimeout = 0;
                if (Inicia_Transac == true)
                {
                    Transaccion = Conexion.BeginTransaction();
                    ComandoSql.Transaction = Transaccion;
                }
                zapertura = true;
            }
            catch (Exception ex)
            {
                SqlError = ex.Message;
                zapertura = false;
            }
            return zapertura;
        }
        //-------------------------------------------------------------------------------------------------------
        // Cierra Conexión
        //-------------------------------------------------------------------------------------------------------
        public bool SqlDesconexion()
        {
            bool zclose = false;
            try
            {
                if (!(Transaccion == null))
                {
                    if (string.IsNullOrEmpty(SqlError))
                        Transaccion.Commit();
                    else
                        Transaccion.Rollback();
                } // if
                Conexion.Close();
                zclose = true;
            }
            catch (Exception ex)
            {
                SqlError = ex.Message;
                try
                {
                    if (!(Transaccion == null))
                        Transaccion.Rollback();
                }
                catch (Exception ex1)
                {
                    SqlError = ex1.Message;
                }
            }
            Transaccion = null;
            if ((ComandoSql != null))
            {
                ComandoSql.Parameters.Clear();
                ComandoSql.Dispose();
            }
            return zclose;
        }

        #endregion

        [HttpGet]
        public List<IncidentType> GetAllIncidentType()
        {
            List<IncidentType> lstRetorno = new List<IncidentType>();
            IncidentType objetolista = new IncidentType();
            if (SqlConexion(false))
            {
                AddStore("Usp_GetAllIncidentType");
                AddParametro("@Id_Incident_Type", SqlDbType.VarChar, DBNull.Value);

                try
                {
                    SqlDataReader reader = ComandoSql.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            objetolista = new IncidentType();
                            objetolista.idIncidentType = Convert.ToInt32(reader["id_Incident_Type"]);
                            objetolista.descriptionIncidentType = Convert.ToString(reader["description_Incident_Type"]);
                            objetolista.statusIncidentType = Convert.ToInt32(reader["status_Incident_Type"]);
                            lstRetorno.Add(objetolista);
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    SqlError = ex.Message;
                }
                SqlDesconexion();
            }
            return lstRetorno;
        }

        [HttpGet]
        public List<IncidentType> GetAllIncidentTypeDetails(int id)
        {
            List<IncidentType> lstRetorno = new List<IncidentType>();
            IncidentType objetolista = new IncidentType();
            if (SqlConexion(false))
            {
                AddStore("Usp_GetAllIncidentType");
                AddParametro("@Id_Incident_Type", SqlDbType.VarChar, id);
                
                try
                {
                    SqlDataReader reader = ComandoSql.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            objetolista = new IncidentType();
                             objetolista.idIncidentType = Convert.ToInt32(reader["id_Incident_Type"]);
                            objetolista.descriptionIncidentType = Convert.ToString(reader["description_Incident_Type"]);
                            objetolista.statusIncidentType = Convert.ToInt32(reader["status_Incident_Type"]);
                            lstRetorno.Add(objetolista);
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    SqlError = ex.Message;
                }
                SqlDesconexion();
            }

            //if (lstRetorno.Count==0)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            //}

            return lstRetorno;
        }


        [HttpPost]
        public Boolean AddIncidentType([FromBody]IncidentType tabla)//
        {
            bool retorno = true;
            if (SqlConexion(true))
            {
                try
                {
                    AddStore("Usp_AddIncidentType");
                    AddParametro("@description_Incident_Type", SqlDbType.VarChar, tabla.descriptionIncidentType);
                                       
                    ComandoSql.ExecuteNonQuery();
                    ComandoSql.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    SqlError = ex.Message;
                    retorno = false;
                }
                SqlDesconexion();
            }
            return (retorno == true);
        }

        [HttpPut]
        //public string UpdateEmpDetails(string Name, String Id)
        //{
        //    return "Employee details Updated with Name " + Name + " and Id " + Id;
        //}
        public Boolean UpdateIncidentType([FromBody]IncidentType tabla)//
        {
            bool retorno = true;
            if (SqlConexion(true))
            {
                try
                {
                    AddStore("Usp_UpdateIncidentType");
                    AddParametro("@id_Incident_Type", SqlDbType.VarChar, tabla.idIncidentType);
                    AddParametro("@description_Incident_Type", SqlDbType.VarChar, tabla.descriptionIncidentType);
                    
                    ComandoSql.ExecuteNonQuery();
                    ComandoSql.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    SqlError = ex.Message;
                    retorno = false;
                }
                SqlDesconexion();
            }
            return (retorno == true);
        }



        [HttpDelete]
        //public string DeleteEmpDetails(string id)
        //{
        //    return "Employee details deleted having Id " + id;

        //}
        public Boolean DeleteIncidentType([FromBody]IncidentType tabla)//
        {
            bool retorno = true;
            if (SqlConexion(true))
            {
                try
                {
                    AddStore("Usp_DeleteIncidentType");
                    AddParametro("@id_Incident_Type", SqlDbType.VarChar, tabla.idIncidentType);
                    ComandoSql.ExecuteNonQuery();
                    ComandoSql.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    SqlError = ex.Message;
                    retorno = false;
                }
                SqlDesconexion();
            }
            return (retorno == true);
        } 



    }
}
