////===============================================================================
//// DB2Helper based on Microsoft Data Access Application Block (DAAB) for .NET
//// http://msdn.microsoft.com/library/en-us/dnbda/html/daab-rm.asp
////
//// DB2Helper.cs
////
//// This file contains the implementations of the DB2Helper and DB2HelperParameterCache
//// classes.
////
//// The DAAB for MS .NET Provider for DB2 has been tested in the context of this Nile implementation,
//// but has not undergone the generic functional testing that the SQL version has gone through.
//// You can use it in other .NET applications using DB2 databases.  For complete docs explaining how to use
//// and how it's built go to the originl appblock link. 
//// For this sample, the code resides in the Nile namespaces not the Microsoft.ApplicationBlocks namespace
////==============================================================================
//using System;
//using System.Data;
//using System.Xml;
//using IBM.Data.DB2;
//using System.Collections;
//using System.Configuration;

namespace DbHelper
{
    /// <summary>
    /// The DB2Helper class is intended to encapsulate high performance, scalable best practices for 
    /// common uses of DB2Client.
    /// </summary>
    public partial class DB2Helper:OracleHelper
    {
    //    //#region ConnectionStrings
    //    //// Read the connection strings from the configuration file
    //    //public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.ConnectionStrings["OraConnString1"].ConnectionString;
    //    //public static readonly string ConnectionStringInventoryDistributedTransaction = ConfigurationManager.ConnectionStrings["OraConnString2"].ConnectionString;
    //    //public static readonly string ConnectionStringOrderDistributedTransaction = ConfigurationManager.ConnectionStrings["OraConnString3"].ConnectionString;
    //    //public static readonly string ConnectionStringProfile = ConfigurationManager.ConnectionStrings["OraProfileConnString"].ConnectionString;
    //    //public static readonly string ConnectionStringMembership = ConfigurationManager.ConnectionStrings["OraMembershipConnString"].ConnectionString;
    //    //#endregion

    //    #region private utility methods & constructors

    //    //Since this class provides only static methods, make the default constructor private to prevent 
    //    //instances from being created with "new DB2Helper()".
    //    public DB2Helper() { }

    //    /// <summary>
    //    /// This method is used to attach array's of DB2Parameters to an DB2Command.
    //    /// 
    //    /// This method will assign a value of DbNull to any parameter with a direction of
    //    /// InputOutput and a value of null.  
    //    /// 
    //    /// This behavior will prevent default values from being used, but
    //    /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
    //    /// where the user provided no input value.
    //    /// </summary>
    //    /// <param name="command">The command to which the parameters will be added</param>
    //    /// <param name="commandParameters">an array of DB2Parameters tho be added to command</param>
    //    private static void AttachParameters(DB2Command command, DB2Parameter[] commandParameters)
    //    {
    //        foreach (DB2Parameter p in commandParameters)
    //        {
    //            //check for derived output value with no value assigned
    //            if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
    //            {
    //                p.Value = DBNull.Value;
    //            }

    //            command.Parameters.Add(p);
    //        }
    //    }

    //    /// <summary>
    //    /// This method assigns an array of values to an array of DB2Parameters.
    //    /// </summary>
    //    /// <param name="commandParameters">array of DB2Parameters to be assigned values</param>
    //    /// <param name="parameterValues">array of objects holding the values to be assigned</param>
    //    private static void AssignParameterValues(DB2Parameter[] commandParameters, object[] parameterValues)
    //    {
    //        if ((commandParameters == null) || (parameterValues == null))
    //        {
    //            //do nothing if we get no data
    //            return;
    //        }

    //        // we must have the same number of values as we pave parameters to put them in
    //        if (commandParameters.Length != parameterValues.Length)
    //        {
    //            throw new ArgumentException("Parameter count does not match Parameter Value count.");
    //        }

    //        //iterate through the DB2Parameters, assigning the values from the corresponding position in the 
    //        //value array
    //        for (int i = 0, j = commandParameters.Length; i < j; i++)
    //        {
    //            commandParameters[i].Value = parameterValues[i];
    //        }
    //    }

    //    /// <summary>
    //    /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
    //    /// to the provided command.
    //    /// </summary>
    //    /// <param name="command">the DB2Command to be prepared</param>
    //    /// <param name="connection">a valid DB2Connection, on which to execute this command</param>
    //    /// <param name="transaction">a valid DB2Transaction, or 'null'</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
    //    /// <param name="commandParameters">an array of DB2Parameters to be associated with the command or 'null' if no parameters are required</param>
    //    private static void PrepareCommand(DB2Command command, DB2Connection connection, DB2Transaction transaction, CommandType commandType, string commandText, DB2Parameter[] commandParameters)
    //    {
    //        //if the provided connection is not open, we will open it
    //        if (connection.State != ConnectionState.Open)
    //        {
    //            connection.Open();
    //        }

    //        command.CommandTimeout = 360;

    //        //associate the connection with the command
    //        command.Connection = connection;

    //        //set the command text (stored procedure name or DB2 statement)
    //        command.CommandText = commandText;

    //        //if we were provided a transaction, assign it.
    //        if (transaction != null)
    //        {
    //            command.Transaction = transaction;
    //        }

    //        //set the command type
    //        command.CommandType = commandType;

    //        //attach the command parameters if they are provided
    //        if (commandParameters != null)
    //        {
    //            AttachParameters(command, commandParameters);
    //        }

    //        return;
    //    }


    //    #endregion private utility methods & constructors

    //    #region ExecuteNonQuery

    //    /// <summary>
    //    /// Execute an DB2Command (that returns no resultset and takes no parameters) against the database specified in 
    //    /// the connection string. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
    //    /// <returns>an int representing the number of rows affected by the command</returns>
    //    public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteNonQuery(connectionString, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns no resultset) against the database specified in the connection string 
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>an int representing the number of rows affected by the command</returns>
    //    public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create & open an DB2Connection, and dispose of it after we are done.
    //        using (DB2Connection cn = new DB2Connection(connectionString))
    //        {
    //            cn.Open();

    //            //call the overload that takes a connection in place of the connection string
    //            return ExecuteNonQuery(cn, commandType, commandText, commandParameters);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns no resultset) against the database specified in 
    //    /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>an int representing the number of rows affected by the command</returns>
    //    public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(connectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns no resultset and takes no parameters) against the provided DB2Connection. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>
    //    /// <returns>an int representing the number of rows affected by the command</returns>
    //    public static int ExecuteNonQuery(DB2Connection connection, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteNonQuery(connection, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns no resultset) against the specified DB2Connection 
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>an int representing the number of rows affected by the command</returns>
    //    public static int ExecuteNonQuery(DB2Connection connection, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create a command and prepare it for execution
    //        DB2Command cmd = new DB2Command();
    //        PrepareCommand(cmd, connection, (DB2Transaction)null, commandType, commandText, commandParameters);
    //        cmd.CommandTimeout = connection.ConnectionTimeout;
    //        //finally, execute the command.
    //        try
    //        {
    //            return cmd.ExecuteNonQuery();
    //        }
    //        catch (Exception err)
    //        {
    //            throw new Exception(commandText + "\r\n" + err.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns no resultset) against the specified DB2Connection 
    //    /// using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>an int representing the number of rows affected by the command</returns>
    //    public static int ExecuteNonQuery(DB2Connection connection, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns no resultset and takes no parameters) against the provided DB2Transaction. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
    //    /// <returns>an int representing the number of rows affected by the command</returns>
    //    public static int ExecuteNonQuery(DB2Transaction transaction, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteNonQuery(transaction, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns no resultset) against the specified DB2Transaction
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>an int representing the number of rows affected by the command</returns>
    //    public static int ExecuteNonQuery(DB2Transaction transaction, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create a command and prepare it for execution
    //        DB2Command cmd = new DB2Command();
    //        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
    //        cmd.CommandTimeout = transaction.Connection.ConnectionTimeout;
    //        //finally, execute the command.
    //        try
    //        {
    //            return cmd.ExecuteNonQuery();
    //        }
    //        catch (Exception err)
    //        {
    //            throw new Exception(commandText + "\r\n" + err.Message);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns no resultset) against the specified 
    //    /// DB2Transaction using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>an int representing the number of rows affected by the command</returns>
    //    public static int ExecuteNonQuery(DB2Transaction transaction, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populet the cache)
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    #endregion ExecuteNonQuery

    //    #region ExecuteDataSet

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset and takes no parameters) against the database specified in 
    //    /// the connection string. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
    //    /// <returns>a dataset containing the resultset generated by the command</returns>
    //    public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteDataset(connectionString, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset) against the database specified in the connection string 
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>a dataset containing the resultset generated by the command</returns>
    //    public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create & open an DB2Connection, and dispose of it after we are done.
    //        using (DB2Connection cn = new DB2Connection(connectionString))
    //        {
    //            try
    //            {
    //                cn.Open();

    //                //call the overload that takes a connection in place of the connection string
    //                return ExecuteDataset(cn, commandType, commandText, commandParameters);
    //            }
    //            catch(Exception err) {
    //                throw err;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns a resultset) against the database specified in 
    //    /// the conneciton string using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>a dataset containing the resultset generated by the command</returns>
    //    public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populet the cache)
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(connectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset and takes no parameters) against the provided DB2Connection. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>
    //    /// <returns>a dataset containing the resultset generated by the command</returns>
    //    public static DataSet ExecuteDataset(DB2Connection connection, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteDataset(connection, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset) against the specified DB2Connection 
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>a dataset containing the resultset generated by the command</returns>
    //    public static DataSet ExecuteDataset(DB2Connection connection, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create a command and prepare it for execution
    //        DB2Command cmd = new DB2Command();
    //        PrepareCommand(cmd, connection, (DB2Transaction)null, commandType, commandText, commandParameters);
    //        cmd.CommandTimeout = connection.ConnectionTimeout;

    //        //create the DataAdapter & DataSet
    //        DB2DataAdapter da = new DB2DataAdapter(cmd);
    //        DataSet ds = new DataSet();

    //        //fill the DataSet using default values for DataTable names, etc.
    //        try
    //        {
    //            da.Fill(ds);
    //        }
    //        catch(Exception e)
    //        {
    //            throw new Exception(e.Message + "\n\r"+commandText);
    //        }
    //        //return the dataset
    //        return ds;
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns a resultset) against the specified DB2Connection 
    //    /// using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>a dataset containing the resultset generated by the command</returns>
    //    public static DataSet ExecuteDataset(DB2Connection connection, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset and takes no parameters) against the provided DB2Transaction. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
    //    /// <returns>a dataset containing the resultset generated by the command</returns>
    //    public static DataSet ExecuteDataset(DB2Transaction transaction, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteDataset(transaction, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset) against the specified DB2Transaction
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>a dataset containing the resultset generated by the command</returns>
    //    public static DataSet ExecuteDataset(DB2Transaction transaction, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create a command and prepare it for execution
    //        DB2Command cmd = new DB2Command();
    //        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);
    //        cmd.CommandTimeout = transaction.Connection.ConnectionTimeout;

    //        //create the DataAdapter & DataSet
    //        DB2DataAdapter da = new DB2DataAdapter(cmd);
    //        DataSet ds = new DataSet();

    //        //fill the DataSet using default values for DataTable names, etc.
    //        da.Fill(ds);

    //        //return the dataset
    //        return ds;
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns a resultset) against the specified 
    //    /// DB2Transaction using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>a dataset containing the resultset generated by the command</returns>
    //    public static DataSet ExecuteDataset(DB2Transaction transaction, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    #endregion ExecuteDataSet

    //    #region ExecuteReader

    //    /// <summary>
    //    /// this enum is used to indicate weather the connection was provided by the caller, or created by DB2Helper, so that
    //    /// we can set the appropriate CommandBehavior when calling ExecuteReader()
    //    /// </summary>
    //    private enum DB2ConnectionOwnership
    //    {
    //        /// <summary>Connection is owned and managed by DB2Helper</summary>
    //        Internal,
    //        /// <summary>Connection is owned and managed by the caller</summary>
    //        External
    //    }


    //    /// <summary>
    //    /// Create and prepare an DB2Command, and call ExecuteReader with the appropriate CommandBehavior.
    //    /// </summary>
    //    /// <remarks>
    //    /// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
    //    /// 
    //    /// If the caller provided the connection, we want to leave it to them to manage.
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection, on which to execute this command</param>
    //    /// <param name="transaction">a valid DB2Transaction, or 'null'</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
    //    /// <param name="commandParameters">an array of DB2Parameters to be associated with the command or 'null' if no parameters are required</param>
    //    /// <param name="connectionOwnership">indicates whether the connection parameter was provided by the caller, or created by DB2Helper</param>
    //    /// <returns>DB2DataReader containing the results of the command</returns>
    //    private static DB2DataReader ExecuteReader(DB2Connection connection, DB2Transaction transaction, CommandType commandType, string commandText, DB2Parameter[] commandParameters, DB2ConnectionOwnership connectionOwnership)
    //    {
    //        //create a command and prepare it for execution
    //        DB2Command cmd = new DB2Command();
    //        PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);

    //        //create a reader
    //        DB2DataReader dr;

    //        // call ExecuteReader with the appropriate CommandBehavior
    //        if (connectionOwnership == DB2ConnectionOwnership.External)
    //        {
    //            dr = cmd.ExecuteReader();
    //        }
    //        else
    //        {
    //            dr = cmd.ExecuteReader((CommandBehavior)((int)CommandBehavior.CloseConnection));
    //        }

    //        return (DB2DataReader)dr;
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset and takes no parameters) against the database specified in 
    //    /// the connection string. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  DB2DataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
    //    /// <returns>an DB2DataReader containing the resultset generated by the command</returns>
    //    public static DB2DataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteReader(connectionString, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset) against the database specified in the connection string 
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  DB2DataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>an DB2DataReader containing the resultset generated by the command</returns>
    //    public static DB2DataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create & open an DB2bConnection
    //        DB2Connection cn = new DB2Connection(connectionString);
    //        cn.Open();

    //        try
    //        {
    //            //call the private overload that takes an internally owned connection in place of the connection string
    //            return ExecuteReader(cn, null, commandType, commandText, commandParameters, DB2ConnectionOwnership.Internal);
    //        }
    //        catch
    //        {
    //            //if we fail to return the DB2DataReader, we need to close the connection ourselves
    //            cn.Close();
    //            throw;
    //        }
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns a resultset) against the database specified in 
    //    /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  DB2DataReader dr = ExecuteReader(connString, "GetOrders", 24, 36);
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>an DB2DataReader containing the resultset generated by the command</returns>
    //    public static DB2DataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(connectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset and takes no parameters) against the provided DB2Connection. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  DB2DataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>
    //    /// <returns>an DB2DataReader containing the resultset generated by the command</returns>
    //    public static DB2DataReader ExecuteReader(DB2Connection connection, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteReader(connection, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset) against the specified DB2Connection 
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  DB2DataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>an DB2DataReader containing the resultset generated by the command</returns>
    //    public static DB2DataReader ExecuteReader(DB2Connection connection, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //pass through the call to the private overload using a null transaction value and an externally owned connection
    //        return ExecuteReader(connection, (DB2Transaction)null, commandType, commandText, commandParameters, DB2ConnectionOwnership.External);
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns a resultset) against the specified DB2Connection 
    //    /// using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  DB2DataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>an DB2DataReader containing the resultset generated by the command</returns>
    //    public static DB2DataReader ExecuteReader(DB2Connection connection, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

    //            AssignParameterValues(commandParameters, parameterValues);

    //            return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteReader(connection, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset and takes no parameters) against the provided DB2Transaction. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  DB2DataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param>  
    //    /// <returns>an DB2DataReader containing the resultset generated by the command</returns>
    //    public static DB2DataReader ExecuteReader(DB2Transaction transaction, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteReader(transaction, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a resultset) against the specified DB2Transaction
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///   DB2DataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command</param> 
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>an DB2DataReader containing the resultset generated by the command</returns>
    //    public static DB2DataReader ExecuteReader(DB2Transaction transaction, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //pass through to private overload, indicating that the connection is owned by the caller
    //        return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, DB2ConnectionOwnership.External);
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns a resultset) against the specified
    //    /// DB2Transaction using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  DB2DataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>an DB2DataReader containing the resultset generated by the command</returns>
    //    public static DB2DataReader ExecuteReader(DB2Transaction transaction, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

    //            AssignParameterValues(commandParameters, parameterValues);

    //            return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    #endregion ExecuteReader

    //    #region ExecuteScalar

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a 1x1 resultset and takes no parameters) against the database specified in 
    //    /// the connection string. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or T-DB2 command</param>
    //    /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
    //    public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteScalar(connectionString, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a 1x1 resultset) against the database specified in the connection string 
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or T-DB2 command</param>
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
    //    public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create & open an DB2Connection, and dispose of it after we are done.
    //        using (DB2Connection cn = new DB2Connection(connectionString))
    //        {
    //            cn.Open();

    //            //call the overload that takes a connection in place of the connection string
    //            return ExecuteScalar(cn, commandType, commandText, commandParameters);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns a 1x1 resultset) against the database specified in 
    //    /// the conneciton string using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
    //    public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populet the cache)
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(connectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a 1x1 resultset and takes no parameters) against the provided DB2Connection. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or T-DB2 command</param>
    //    /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
    //    public static object ExecuteScalar(DB2Connection connection, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteScalar(connection, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a 1x1 resultset) against the specified DB2Connection 
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or T-OleDb command</param>
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
    //    public static object ExecuteScalar(DB2Connection connection, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create a command and prepare it for execution
    //        DB2Command cmd = new DB2Command();
    //        PrepareCommand(cmd, connection, (DB2Transaction)null, commandType, commandText, commandParameters);

    //        //execute the command & return the results
    //        return cmd.ExecuteScalar();
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns a 1x1 resultset) against the specified DB2Connection 
    //    /// using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
    //    public static object ExecuteScalar(DB2Connection connection, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populet the cache)
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a 1x1 resultset and takes no parameters) against the provided DB2Transaction. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or T-OleDb command</param>
    //    /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
    //    public static object ExecuteScalar(DB2Transaction transaction, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteScalar(transaction, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute an DB2Command (that returns a 1x1 resultset) against the specified DB2Transaction
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or T-OleDb command</param>
    //    /// <param name="commandParameters">an array of DB2Parameters used to execute the command</param>
    //    /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
    //    public static object ExecuteScalar(DB2Transaction transaction, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create a command and prepare it for execution
    //        DB2Command cmd = new DB2Command();
    //        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

    //        //execute the command & return the results
    //        return cmd.ExecuteScalar();

    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via an DB2Command (that returns a 1x1 resultset) against the specified
    //    /// DB2Transaction using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
    //    public static object ExecuteScalar(DB2Transaction transaction, string spName, params object[] parameterValues)
    //    {
    //        //if we got parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populet the cache)
    //            DB2Parameter[] commandParameters = DB2HelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    #endregion ExecuteScalar

    //    #region ExecuteXmlReader
    //    /*
    //    /// <summary>
    //    /// Execute a DB2Command (that returns a resultset and takes no parameters) against the provided DB2Connection. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders");
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command using "FOR XML AUTO"</param>
    //    /// <returns>an XmlReader containing the resultset generated by the command</returns>
    //    public static XmlReader ExecuteXmlReader(DB2Connection connection, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteXmlReader(connection, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute a DB2Command (that returns a resultset) against the specified DB2Connection 
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command using "FOR XML AUTO"</param>
    //    /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
    //    /// <returns>an XmlReader containing the resultset generated by the command</returns>
    //    public static XmlReader ExecuteXmlReader(DB2Connection connection, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create a command and prepare it for execution
    //        DB2Command cmd = new DB2Command();
    //        PrepareCommand(cmd, connection, (DB2Transaction)null, commandType, commandText, commandParameters);

    //        //create the DataAdapter & DataSet
    //        XmlReader retval = cmd.ExecuteXmlReader();

    //        // detach the DB2Parameters from the command object, so they can be used again.
    //        cmd.Parameters.Clear();
    //        return retval;

    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via a DB2Command (that returns a resultset) against the specified DB2Connection 
    //    /// using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36);
    //    /// </remarks>
    //    /// <param name="connection">a valid DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure using "FOR XML AUTO"</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>an XmlReader containing the resultset generated by the command</returns>
    //    public static XmlReader ExecuteXmlReader(DB2Connection connection, string spName, params object[] parameterValues)
    //    {
    //        //if we receive parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
    //            DB2Parameter[] commandParameters = DB2DataAccessParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    /// <summary>
    //    /// Execute a DB2Command (that returns a resultset and takes no parameters) against the provided DB2Transaction. 
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders");
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command using "FOR XML AUTO"</param>
    //    /// <returns>an XmlReader containing the resultset generated by the command</returns>
    //    public static XmlReader ExecuteXmlReader(DB2Transaction transaction, CommandType commandType, string commandText)
    //    {
    //        //pass through the call providing null for the set of DB2Parameters
    //        return ExecuteXmlReader(transaction, commandType, commandText, (DB2Parameter[])null);
    //    }

    //    /// <summary>
    //    /// Execute a DB2Command (that returns a resultset) against the specified DB2Transaction
    //    /// using the provided parameters.
    //    /// </summary>
    //    /// <remarks>
    //    /// e.g.:  
    //    ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new DB2Parameter("@prodid", 24));
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
    //    /// <param name="commandText">the stored procedure name or PL/SQL command using "FOR XML AUTO"</param>
    //    /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
    //    /// <returns>an XmlReader containing the resultset generated by the command</returns>
    //    public static XmlReader ExecuteXmlReader(DB2Transaction transaction, CommandType commandType, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        //create a command and prepare it for execution
    //        DB2Command cmd = new DB2Command();
    //        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

    //        //create the DataAdapter & DataSet
    //        XmlReader retval = cmd.ExecuteXmlReader();

    //        // detach the DB2Parameters from the command object, so they can be used again.
    //        cmd.Parameters.Clear();
    //        return retval;
    //    }

    //    /// <summary>
    //    /// Execute a stored procedure via a DB2Command (that returns a resultset) against the specified 
    //    /// DB2Transaction using the provided parameter values.  This method will query the database to discover the parameters for the 
    //    /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
    //    /// </summary>
    //    /// <remarks>
    //    /// This method provides no access to output parameters or the stored procedure's return value parameter.
    //    /// 
    //    /// e.g.:  
    //    ///  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36);
    //    /// </remarks>
    //    /// <param name="transaction">a valid DB2Transaction</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
    //    /// <returns>a dataset containing the resultset generated by the command</returns>
    //    public static XmlReader ExecuteXmlReader(DB2Transaction transaction, string spName, params object[] parameterValues)
    //    {
    //        //if we receive parameter values, we need to figure out where they go
    //        if ((parameterValues != null) && (parameterValues.Length > 0))
    //        {
    //            //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
    //            DB2Parameter[] commandParameters = DB2DataAccessParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

    //            //assign the provided values to these parameters based on parameter order
    //            AssignParameterValues(commandParameters, parameterValues);

    //            //call the overload that takes an array of DB2Parameters
    //            return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
    //        }
    //        //otherwise we can just call the SP without params
    //        else
    //        {
    //            return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
    //        }
    //    }

    //    */
    //    #endregion ExecuteXmlReader
    //}

    ///// <summary>
    ///// DB2HelperParameterCache provides functions to leverage a static cache of procedure parameters, and the
    ///// ability to discover parameters for stored procedures at run-time.
    ///// </summary>
    //public sealed class DB2HelperParameterCache
    //{
    //    #region private methods, variables, and constructors

    //    //Since this class provides only static methods, make the default constructor private to prevent 
    //    //instances from being created with "new DB2HelperParameterCache()".
    //    private DB2HelperParameterCache() { }

    //    private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

    //    /// <summary>
    //    /// resolve at run-time the appropriate set of DB2Parameters for a stored procedure
    //    /// </summary>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="includeReturnValueParameter">whether or not to include ther return value parameter</param>
    //    /// <returns></returns>
    //    private static DB2Parameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
    //    {
    //        using (DB2Connection cn = new DB2Connection(connectionString))
    //        using (DB2Command cmd = new DB2Command(spName, cn))
    //        {
    //            cn.Open();
    //            cmd.CommandType = CommandType.StoredProcedure;

    //            DB2CommandBuilder.DeriveParameters(cmd);

    //            if (!includeReturnValueParameter)
    //            {
    //                if (ParameterDirection.ReturnValue == cmd.Parameters[0].Direction)
    //                    cmd.Parameters.RemoveAt(0);
    //            }

    //            DB2Parameter[] discoveredParameters = new DB2Parameter[cmd.Parameters.Count];

    //            cmd.Parameters.CopyTo(discoveredParameters, 0);

    //            return discoveredParameters;
    //        }
    //    }

    //    //deep copy of cached DB2Parameter array
    //    private static DB2Parameter[] CloneParameters(DB2Parameter[] originalParameters)
    //    {
    //        DB2Parameter[] clonedParameters = new DB2Parameter[originalParameters.Length];

    //        for (int i = 0, j = originalParameters.Length; i < j; i++)
    //        {
    //            clonedParameters[i] = (DB2Parameter)((ICloneable)originalParameters[i]).Clone();
    //        }

    //        return clonedParameters;
    //    }

    //    #endregion private methods, variables, and constructors

    //    #region caching functions

    //    /// <summary>
    //    /// add parameter array to the cache
    //    /// </summary>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="commandText">the stored procedure name or T-OleDb command</param>
    //    /// <param name="commandParameters">an array of DB2Parameters to be cached</param>
    //    public static void CacheParameterSet(string connectionString, string commandText, params DB2Parameter[] commandParameters)
    //    {
    //        string hashKey = connectionString + ":" + commandText;

    //        paramCache[hashKey] = commandParameters;
    //    }

    //    /// <summary>
    //    /// retrieve a parameter array from the cache
    //    /// </summary>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="commandText">the stored procedure name or T-OleDb command</param>
    //    /// <returns>an array of DB2Parameters</returns>
    //    public static DB2Parameter[] GetCachedParameterSet(string connectionString, string commandText)
    //    {
    //        string hashKey = connectionString + ":" + commandText;

    //        DB2Parameter[] cachedParameters = (DB2Parameter[])paramCache[hashKey];

    //        if (cachedParameters == null)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            return CloneParameters(cachedParameters);
    //        }
    //    }

    //    #endregion caching functions

    //    #region Parameter Discovery Functions

    //    /// <summary>
    //    /// Retrieves the set of DB2Parameters appropriate for the stored procedure
    //    /// </summary>
    //    /// <remarks>
    //    /// This method will query the database for this information, and then store it in a cache for future requests.
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <returns>an array of DB2Parameters</returns>
    //    public static DB2Parameter[] GetSpParameterSet(string connectionString, string spName)
    //    {
    //        return GetSpParameterSet(connectionString, spName, false);
    //    }

    //    /// <summary>
    //    /// Retrieves the set of DB2Parameters appropriate for the stored procedure
    //    /// </summary>
    //    /// <remarks>
    //    /// This method will query the database for this information, and then store it in a cache for future requests.
    //    /// </remarks>
    //    /// <param name="connectionString">a valid connection string for an DB2Connection</param>
    //    /// <param name="spName">the name of the stored procedure</param>
    //    /// <param name="includeReturnValueParameter">a bool value indicating whether the return value parameter should be included in the results</param>
    //    /// <returns>an array of DB2Parameters</returns>
    //    public static DB2Parameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
    //    {
    //        string hashKey = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

    //        DB2Parameter[] cachedParameters;

    //        cachedParameters = (DB2Parameter[])paramCache[hashKey];

    //        if (cachedParameters == null)
    //        {
    //            cachedParameters = (DB2Parameter[])(paramCache[hashKey] = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter));
    //        }

    //        return CloneParameters(cachedParameters);
    //    }

    //    #endregion Parameter Discovery Functions

    }
}

