using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct DataNameManager {

	public const string PeopleDB = "PeopleDB";
	public const string ModelDB = "ModelDB";
}


public class SQLModelInfo
{
    public int ID;
    public int ModelIfoId;
    public string Name;
    public int Basetype;
    public int Modeltype;
    public string Path;
    public string Time;

}

public class SQLiteDataManager 
{
	public static SQLiteDataManager m_instance;

	
	public static SQLiteDataManager Instance
	{
		get
		{
			if (m_instance == null)
				m_instance = new SQLiteDataManager();
			return m_instance;
		}

	}

	public string m_strDBPath = "";
	public string DBName { get; set; } = "GDB";//表名,这没用到，方法中自带参数name，代表数据库的名字


	public SQLiteDataManager()
    {
		m_strDBPath = Environment.CurrentDirectory + "/RawData/DB.db";
        if (!File.Exists(m_strDBPath))
        {
			SqliteConnection.CreateFile(m_strDBPath);
        }

	}


	//查找是否存在数据库的表
	public bool IsHaveTable(string name)
	{
		int t = -1;
		using (SqliteConnection dbConn = new SqliteConnection("Data Source=" + m_strDBPath))
		{
			dbConn.Open();
			string s = string.Format("SELECT COUNT(*) FROM sqlite_master WHERE TYPE='table' AND NAME = '{0}'", name);
			using (var com = new SqliteCommand(s, dbConn as SqliteConnection))
			{
				using (SqliteDataReader dataReader = com.ExecuteReader())
				{
					while (dataReader.Read())
					{
						t = dataReader.GetInt32(0);
					}
				}
			}
			dbConn.Close();
			dbConn.Dispose();
		}
	   return t == 1;
	}





	//创建数据库下的表
	public void CreatTable(string name)
	{
		using (SqliteConnection dbConn = new SqliteConnection("Data Source=" + m_strDBPath))
		{
			dbConn.Open();
			string s = string.Format("Create Table if not exists {0} ('ID' INTEGER, 'ModelInfoId' INTEGER,'Name','Basetype'INTEGER,'Modeltype'INTEGER,'Path','Time' ,PRIMARY KEY('ID' AUTOINCREMENT))", name);


			using (var com = new SqliteCommand(s, dbConn as SqliteConnection))
			{
				com.ExecuteNonQuery();

			}
			dbConn.Close();
			dbConn.Dispose();
		}

	}

	//读取所有的数据
	public List<SQLModelInfo> SelectTable(string name)
	{

		if (!IsHaveTable(name))
			CreatTable(name);

		List<SQLModelInfo> _lstModelInfo = new List<SQLModelInfo>();
		using (SqliteConnection dbConn = new SqliteConnection("Data Source=" + m_strDBPath))
		{
			dbConn.Open();
			string s = string.Format("Select * from {0}", name);

			using (var com = new SqliteCommand(s, dbConn as SqliteConnection))
			{
				using (SqliteDataReader dataReader = com.ExecuteReader())
				{

					while (dataReader.Read())
					{
						SQLModelInfo modelInfo = new SQLModelInfo();
						modelInfo.ID = dataReader.GetInt32(0);
						modelInfo.ModelIfoId = dataReader.GetInt32(1);
						modelInfo.Name = dataReader.GetString(2);
						modelInfo.Basetype = dataReader.GetInt32(3);
						modelInfo.Modeltype = dataReader.GetInt32(4);
						modelInfo.Path = dataReader.GetString(5);
						modelInfo.Time = dataReader.GetString(6);
						_lstModelInfo.Add(modelInfo);
					}
				}			
			}
			dbConn.Close();
			dbConn.Dispose();
		}		
		return _lstModelInfo;
	}


	public SQLModelInfo SelectTableByID(int id,string name)
	{

		if (!IsHaveTable(name))
			CreatTable(name);

		SQLModelInfo modelInfo = null;

		using (SqliteConnection dbConn = new SqliteConnection("Data Source=" + m_strDBPath))
		{
			dbConn.Open();
			string s = string.Format("Select * from {0} where ID={1}", name, id);

			using (var com = new SqliteCommand(s, dbConn as SqliteConnection))
			{
				using (SqliteDataReader dataReader = com.ExecuteReader())
				{

					while (dataReader.Read())
					{
						
						modelInfo.ID = dataReader.GetInt32(0);
						modelInfo.ModelIfoId = dataReader.GetInt32(1);
						modelInfo.Name = dataReader.GetString(2);
						modelInfo.Basetype = dataReader.GetInt32(3);
						modelInfo.Modeltype = dataReader.GetInt32(4);
						modelInfo.Path = dataReader.GetString(5);
						modelInfo.Time = dataReader.GetString(6);
						
					}
				}
			}
			dbConn.Close();
			dbConn.Dispose();
		}
		
		return modelInfo;

	}

	public List<SQLModelInfo> SelectTableByModelId(int modelID,string name)
	{
		if (!IsHaveTable(name))
			CreatTable(name);
		List<SQLModelInfo> _lstModelInfo = new List<SQLModelInfo>();
		using (SqliteConnection dbConn = new SqliteConnection("Data Source=" + m_strDBPath))
		{
			dbConn.Open();
			string s = string.Format("Select * from {0} where {1}", name, "ModelIfoId=" + modelID);

			using (var com = new SqliteCommand(s, dbConn as SqliteConnection))
			{
				using (SqliteDataReader dataReader = com.ExecuteReader())
				{

					while (dataReader.Read())
					{
						SQLModelInfo modelInfo = new SQLModelInfo();
						modelInfo.ID = dataReader.GetInt32(0);
						modelInfo.ModelIfoId = dataReader.GetInt32(1);
						modelInfo.Name = dataReader.GetString(2);
						modelInfo.Basetype = dataReader.GetInt32(3);
						modelInfo.Modeltype = dataReader.GetInt32(4);
						modelInfo.Path = dataReader.GetString(5);
						modelInfo.Time = dataReader.GetString(6);
						_lstModelInfo.Add(modelInfo);
					}
				}
			}
			dbConn.Close();
			dbConn.Dispose();
		}
		return _lstModelInfo;
	}

	public int InsertTable(SQLModelInfo modelInfo,string name)
	{
		if (!IsHaveTable(name))
			CreatTable(name);

		int temp = 0;
		using (SqliteConnection dbConn = new SqliteConnection("Data Source=" + m_strDBPath))
		{
			dbConn.Open();
			string s = string.Format("insert into {0} values(@ID,@ModelIfoId,@Name,@Basetype,@Modeltype,@Path,@Time)", name);

			using (var com = new SqliteCommand(s, dbConn as SqliteConnection))
			{

				com.Parameters.Add("ID", System.Data.DbType.Int32).Value = null;
				com.Parameters.Add("ModelIfoId", System.Data.DbType.Int32).Value = modelInfo.ModelIfoId;
				com.Parameters.Add("Name", System.Data.DbType.String).Value = modelInfo.Name;
				com.Parameters.Add("Basetype", System.Data.DbType.Int32).Value = modelInfo.Basetype;
				com.Parameters.Add("Modeltype", System.Data.DbType.Int32).Value = modelInfo.Modeltype;
				com.Parameters.Add("Path", System.Data.DbType.String).Value = modelInfo.Path;
				com.Parameters.Add("Time", System.Data.DbType.String).Value = modelInfo.Time;
				
				temp = com.ExecuteNonQuery();
			}

			dbConn.Close();
			dbConn.Dispose();
		}
		if (temp > 0)
		{
			Debug.Log("插入成功");
		}
		return temp;
	}

	public int UpdateTable(SQLModelInfo modelInfo, string name)
	{
		if (!IsHaveTable(name))
			CreatTable(name);

		int temp = 0;
		using (SqliteConnection dbConn = new SqliteConnection("Data Source=" + m_strDBPath))
		{
			dbConn.Open();
			string s = string.Format("update  {0} set ModelIfoId=@ModelIfoId,Name=@Name,Basetype=@Basetype,Modeltype=@Modeltype,Path=@Path,Time=@Time)",
			"ID=" + modelInfo.ID);

			using (SqliteCommand com = new SqliteCommand(s, dbConn as SqliteConnection))
			{


				com.Parameters.Add("ID", System.Data.DbType.Int32).Value = null;
				com.Parameters.Add("ModelIfoId", System.Data.DbType.Int32).Value = modelInfo.ModelIfoId;
				com.Parameters.Add("Name", System.Data.DbType.String).Value = modelInfo.Name;
				com.Parameters.Add("Basetype", System.Data.DbType.Int32).Value = modelInfo.Basetype;
				com.Parameters.Add("Modeltype", System.Data.DbType.Int32).Value = modelInfo.Modeltype;
				com.Parameters.Add("Path", System.Data.DbType.String).Value = modelInfo.Path;
				com.Parameters.Add("Time", System.Data.DbType.String).Value = modelInfo.Time;
				temp = com.ExecuteNonQuery();
			}

			dbConn.Close();
			dbConn.Dispose();
		}
		if (temp > 0)
		{
			Debug.Log("更新成功");
		}
		return temp;
	}

	public int deleteTable(int ID,string name)
	{
		if (!IsHaveTable(name))
			CreatTable(name);

		int temp = -1;

		using (SqliteConnection dbConn = new SqliteConnection("Data Source=" + m_strDBPath))
		{
			dbConn.Open();
			string s = string.Format("delete from  {0} where {1}", name, "ID=" + ID);
			using (SqliteCommand com = new SqliteCommand(s, dbConn as SqliteConnection))
			{

				temp = com.ExecuteNonQuery();
			}

		}
		if (temp > -1)
		{
			Debug.Log("删除成功");
		}
		return temp;

	}

}

