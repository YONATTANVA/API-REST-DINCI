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
    public class CitizenController : ApiController
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
        public List<Citizen> GetAllCitizen()
        {
            List<Citizen> lstRetorno = new List<Citizen>();
            Citizen objetolista = new Citizen();
            if (SqlConexion(false))
            {
                AddStore("Usp_GetAllCitizen");
                AddParametro("@Id_citizen", SqlDbType.VarChar, DBNull.Value);

                try
                {
                    SqlDataReader reader = ComandoSql.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            objetolista = new Citizen();
                            objetolista.idCitizen = Convert.ToInt32(reader["id_Citizen"]);
                            objetolista.nameCitizen = Convert.ToString(reader["name_Citizen"]);
                            objetolista.passwordCitizen = Convert.ToString(reader["password_Citizen"]);
                            objetolista.phonenumberCitizen = Convert.ToString(reader["phonenumber_Citizen"]);
                            objetolista.statusCitizen = Convert.ToString(reader["status_Citizen"]);
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
       public List<Citizen> GetAllCitizenDetails(int id)
       {
           List<Citizen> lstRetorno = new List<Citizen>();
           Citizen objetolista = new Citizen();
           if (SqlConexion(false))
           {
               AddStore("Usp_GetAllCitizen");
               AddParametro("@Id_citizen", SqlDbType.VarChar, id);

               try
               {
                   SqlDataReader reader = ComandoSql.ExecuteReader();
                   if (reader.HasRows)
                   {
                       while (reader.Read())
                       {
                           objetolista = new Citizen();
                           objetolista.idCitizen = Convert.ToInt32(reader["id_Citizen"]);
                           objetolista.nameCitizen = Convert.ToString(reader["name_Citizen"]);
                           objetolista.passwordCitizen = Convert.ToString(reader["password_Citizen"]);
                           objetolista.phonenumberCitizen = Convert.ToString(reader["phonenumber_Citizen"]);
                           objetolista.statusCitizen = Convert.ToString(reader["status_Citizen"]);
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
      public Boolean AddCitizen ([FromBody]Citizen tabla)//
      {
          bool retorno = true;
          if (SqlConexion(true))
          {
              try
              {                  
                      AddStore("Usp_AddCitizen");
                      AddParametro("@name_citizen", SqlDbType.VarChar, tabla.nameCitizen);
                      AddParametro("@password_citizen", SqlDbType.VarChar, tabla.passwordCitizen);
                      AddParametro("@phonenumber_citizen", SqlDbType.VarChar, tabla.phonenumberCitizen);                      
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
       public Boolean UpdateCitizen([FromBody]Citizen tabla)//
       {
           bool retorno = true;
           if (SqlConexion(true))
           {
               try
               {
                   AddStore("Usp_UpdateCitizen");
                   AddParametro("@id_citizen", SqlDbType.VarChar, tabla.idCitizen);
                   AddParametro("@name_citizen", SqlDbType.VarChar, tabla.nameCitizen);
                   AddParametro("@password_citizen", SqlDbType.VarChar, tabla.passwordCitizen);
                   AddParametro("@phonenumber_citizen", SqlDbType.VarChar, tabla.phonenumberCitizen);
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
        public Boolean DeleteCitizen([FromBody]Citizen tabla)//
        {
            bool retorno = true;
            if (SqlConexion(true))
            {
                try
                {
                    AddStore("Usp_DeleteCitizen");
                    AddParametro("@id_citizen", SqlDbType.VarChar, tabla.idCitizen);                    
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
