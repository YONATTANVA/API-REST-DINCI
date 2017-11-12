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
    public class IncidentController : ApiController
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
        public List<Incident> GetAllIncident()
        {
            List<Incident> lstRetorno = new List<Incident>();
            Incident objetolista = new Incident();
            if (SqlConexion(false))
            {
                AddStore("Usp_GetAllIncident");
                AddParametro("@Id_Incident", SqlDbType.VarChar, DBNull.Value);
                AddParametro("@Id_citizen", SqlDbType.VarChar, DBNull.Value);
                AddParametro("@Id_incident_type", SqlDbType.VarChar, DBNull.Value);

                try
                {
                    SqlDataReader reader = ComandoSql.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            objetolista = new Incident();
                            objetolista.IdIncident = Convert.ToInt32(reader["id_Incident"]);
                            objetolista.idIncidentType = Convert.ToString(reader["id_Incident_Type"]);
                            objetolista.dateIncident = Convert.ToString(reader["date_Incident"]);
                            objetolista.ubicationIncident = Convert.ToString(reader["ubication_Incident"]);
                            objetolista.commetsIncident = Convert.ToString(reader["commets_Incident"]);
                            objetolista.idStatus = Convert.ToInt32(reader["id_Status"]);
                            objetolista.idCitizen = Convert.ToInt32(reader["id_Citizen"]);
                            objetolista.idEmployee = Convert.ToInt32(reader["id_Employee"]);
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
        public List<Incident> GetAllIncidentDetails(int id)
        {
            List<Incident> lstRetorno = new List<Incident>();
            Incident objetolista = new Incident();
            if (SqlConexion(false))
            {
                AddStore("Usp_GetAllIncident");
                AddParametro("@Id_Incident", SqlDbType.VarChar, DBNull.Value);
                AddParametro("@Id_citizen", SqlDbType.VarChar, DBNull.Value);
                AddParametro("@Id_incident_type", SqlDbType.VarChar, DBNull.Value);

                try
                {
                    SqlDataReader reader = ComandoSql.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            objetolista = new Incident();
                            objetolista.IdIncident = Convert.ToInt32(reader["id_Incident"]);
                            objetolista.idIncidentType = Convert.ToString(reader["id_Incident_Type"]);
                            objetolista.dateIncident = Convert.ToString(reader["date_Incident"]);
                            objetolista.ubicationIncident = Convert.ToString(reader["ubication_Incident"]);
                            objetolista.commetsIncident = Convert.ToString(reader["commets_Incident"]);
                            objetolista.idStatus = Convert.ToInt32(reader["id_Status"]);
                            objetolista.idCitizen = Convert.ToInt32(reader["id_Citizen"]);
                            objetolista.idEmployee = Convert.ToInt32(reader["id_Employee"]);
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
        public Boolean AddIncident([FromBody]Incident tabla)//
        {
            bool retorno = true;
            if (SqlConexion(true))
            {
                try
                {
                    AddStore("Usp_AddIncident");
                    AddParametro("@id_incident_type", SqlDbType.VarChar, tabla.idIncidentType);
                    AddParametro("@ubication_incident", SqlDbType.VarChar, tabla.ubicationIncident);
                    AddParametro("@commets_incident", SqlDbType.VarChar, tabla.commetsIncident);
                    AddParametro("@id_citizen", SqlDbType.VarChar, tabla.idCitizen);
                    AddParametro("@id_employee", SqlDbType.VarChar, DBNull.Value);

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
        public Boolean UpdateIncident([FromBody]Incident tabla)//
        {
            bool retorno = true;
            if (SqlConexion(true))
            {
                try
                {
                    AddStore("Usp_UpdateIncident");
                    AddParametro("@Id_incident", SqlDbType.VarChar, tabla.IdIncident);
                    AddParametro("@ubication_incident", SqlDbType.VarChar, tabla.ubicationIncident);
                    AddParametro("@commets_incident", SqlDbType.VarChar, tabla.commetsIncident);
                    AddParametro("@id_status", SqlDbType.VarChar, tabla.idStatus);
                    

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
        public Boolean DeleteIncident([FromBody]Incident tabla)//
        {
            bool retorno = true;
            if (SqlConexion(true))
            {
                try
                {
                    AddStore("Usp_DeleteIncident");
                    AddParametro("@Id_incident", SqlDbType.VarChar, tabla.IdIncident);
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
