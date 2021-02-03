﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

class SqlServer
{
    /// <summary>
    /// Returns ADO connection string
    /// </summary>
    private static string ConnectionString
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString;
        }
    }

    /// <summary>
    /// Returns string from first column of first row of records returned
    /// </summary>
    /// <param name="strSQL">SQL statement</param>
    /// <returns>string: first column of first row generated by SQL statement</returns>
    public static string Scalar(string strSQL)
    {
        Params arrParams = new Params();
        return Scalar(strSQL, arrParams);
    }

    /// <summary>
    /// Returns string from first column of first row of records returned
    /// </summary>
    /// <param name="strSQL">SQL statement</param>
    /// <param name="arrParams">Parameter collection</param>
    /// <returns>string: first column of first row generated by SQL statement</returns>
    public static string Scalar(string strSQL, Params arrParams)
    {
        SqlParameter[] sqlParams = arrParams.ToArray();
        SqlConnection myConnection = new SqlConnection(SqlServer.ConnectionString);
        SqlCommand myCmd = new SqlCommand(strSQL, myConnection);
        myCmd.CommandType = CommandType.Text;
        if (sqlParams.Length > 0)
        {
            for (int i = 0; i < sqlParams.Length; i++)
            {
                myCmd.Parameters.Add(sqlParams[i]);
            }
        }
        myConnection.Open();
        string strScalarValue = myCmd.ExecuteScalar().ToString();
        myCmd.ExecuteNonQuery();
        myConnection.Close();
        return strScalarValue;
    }

    /// <summary>
    /// Returns in-memory representation of database query result.
    /// </summary>
    /// <param name="strSQL">SQL statement</param>
    /// <returns>DataTable: In-memory representation of all rows and columns returned by query.</returns>
    public static DataTable Recordset(string strSQL)
    {
        Params arrParams = new Params();
        return Recordset(strSQL, arrParams);
    }

    /// <summary>
    /// Returns in-memory representation of database query result.
    /// </summary>
    /// <param name="strSQL">SQL statement</param>
    /// <param name="arrParams">Parameter collection</param>
    /// <returns>DataTable: In-memory representation of all rows and columns returned by query.</returns>
    public static DataTable Recordset(string strSQL, Params arrParams)
    {

        SqlParameter[] sqlParams = arrParams.ToArray();
        SqlConnection myConnection = new SqlConnection(SqlServer.ConnectionString);
        SqlDataAdapter myAdapter = new SqlDataAdapter();
        myAdapter.SelectCommand = new SqlCommand(strSQL, myConnection);
        myAdapter.SelectCommand.CommandType = CommandType.Text;
        if (sqlParams.Length > 0)
        {
            for (int i = 0; i < sqlParams.Length; i++)
            {
                myAdapter.SelectCommand.Parameters.Add(sqlParams[i]);
            }
        }
        DataTable dt = new DataTable();
        myAdapter.Fill(dt);
        return dt;
    }

    /// <summary>
    /// Executes SQL query. Used for INSERT, UPDATE and DELETE commands.
    /// </summary>
    /// <param name="strSQL">SQL statement</param>
    /// <returns>int: ID number of row affected by query.</returns>
    public static void Execute(string strSQL)
    {
        Params arrParams = new Params();
        int x = Execute(strSQL, arrParams, true);
    }

    /// <summary>
    /// Executes SQL query. Used for INSERT, UPDATE and DELETE commands.
    /// </summary>
    /// <param name="strSQL">SQL statement</param>
    /// <param name="arrParams">Parameter collection</param>
    /// <returns>int: ID number of row affected by query.</returns>
    public static void Execute(string strSQL, Params arrParams)
    {
        int x = Execute(strSQL, arrParams, true);
    }

    /// <summary>
    /// Executes SQL query. Used for INSERT, UPDATE and DELETE commands.
    /// </summary>
    /// <param name="strSQL">SQL statement</param>
    /// <param name="arrParams">Parameter collection</param>
    /// /// <param name="arrParams">Parameter collection</param>
    /// <returns>int: ID number of row affected by query.</returns>
    public static int Execute(string strSQL, Params arrParams, bool ReturnID)
    {
        strSQL = strSQL + "; SELECT SCOPE_IDENTITY() AS NewID;";
        SqlConnection myConnection = new SqlConnection(SqlServer.ConnectionString);
        SqlCommand myCmd = new SqlCommand(strSQL, myConnection);
        myCmd.CommandType = CommandType.Text;
        if (arrParams.Count > 0)
        {
            for (int i = 0; i < arrParams.Count; i++)
            {
                myCmd.Parameters.Add(arrParams[i]);
            }
        }
        myConnection.Open();
        object message = myCmd.ExecuteScalar();
        myConnection.Close();

        int id = 0;
        Int32.TryParse(message.ToString(), out id);
        return id;
    }
}

/// <summary>
/// Collection of SQL parameters.
/// </summary>
class Params
{
    private ArrayList _params;

    public Params()
    {
        _params = new ArrayList();
    }

    /// <summary>
    /// Adds SQL parameter to collection.
    /// </summary>
    /// <param name="sName">The name of the parameter e.g. @employee_id</param>
    /// <param name="oValue">The value of the parameter</param>
    public void Add(string sName, object oValue)
    {
        Add(sName, oValue, false);
    }

    /// <summary>
    /// Adds SQL parameter to collection.
    /// </summary>
    /// <param name="sName">The name of the parameter e.g. @employee_id</param>
    /// <param name="oValue">The value of the parameter</param>
    /// <param name="IsBinary">Set to true if the SQL parameter value is a binary stream, to be inserted into a BLOB / IMAGE column</param>
    public void Add(string sName, object oValue, bool IsBinary)
    {
        if (IsBinary)
        {
            byte[] image = (byte[])oValue;
            _params.Add(new SqlParameter(sName, SqlDbType.Image, image.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, image));
        }
        else
        {
            _params.Add(new SqlParameter(sName, oValue.ToString()));
        }
    }

    /// <summary>
    /// Returns parameter list as native SqlParameter array. Not to be used outside internal database logic.
    /// </summary>
    /// <returns>SqlParameter[]: native SqlParameter array</returns>
    public SqlParameter[] ToArray()
    {
        return (SqlParameter[])_params.ToArray(typeof(SqlParameter));
    }

    public object this[int index]
    {
        get
        {
            if (index < 0 || index >= _params.Count)
            {
                // handle bad index
            }
            object[] aObjs = _params.ToArray();
            return aObjs[index];
        }
    }

    /// <summary>
    /// Returns the number of parameters within the collection.
    /// </summary>
    /// <returns>int: number of parameters within collection</returns>
    public int Count
    {
        get
        {
            return _params.Count;
        }
    }
}
