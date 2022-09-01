using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ModelInfo {
    public int ID;
    public int ModelIfoId;
    public string Name;
	public int Basetype;
	public int Modeltype;
	public string Path;
    public string Time;

}


public class DataManager 
{
    public static DataManager m_instance;

	private const string datebase = "host=127.0.0.1;port=3306;user=root;password=root12345,database=tool;";
	string Tablebase = "mygamedb";//数据库表
	private MySqlConnection connection = null;
	public static DataManager Instance
    {
        get 
        { 
            if (m_instance == null)
                m_instance = new DataManager();
                return m_instance;
        }

    }

	//连接mysql数据库
	public  bool Connect()
	{
		//创建MySqlConnection对象
		connection = new MySqlConnection();
		connection.ConnectionString = datebase;
		//连接
		try
		{
			connection.Open();
			Debug.Log("[数据库]connect succ ");
			return true;
		}
		catch (Exception e)
		{
			Debug.Log("[数据库]connect fail, " + e.Message);
			return false;
		}
	}

	//查找是否存在数据库的表
	public bool IsHaveTable()
    {
		int t = -1;
		

		if (Connect())
        {
		  connection.Open();

		  string s = string.Format("select *from information_schema.TABLES where TABLE_NAME = '{0}'", Tablebase);
			
		  using (var mySqlCommand = new MySqlCommand(s, connection))
            {
				using (MySqlDataReader dataReader= mySqlCommand.ExecuteReader())
                {
					while (dataReader.Read())
                    {
						t = dataReader.GetInt32(0);
                    }
                }
				mySqlCommand.Dispose();
		  }
			connection.Close();
			connection.Dispose();
		}			
		return t==1;
    }

	//创建数据库下的表
	public void CreatTable()
    {
		if (Connect())
		{
			connection.Open();

			string s = string.Format("Create Table if not exists {0} ('ID' INTEGER, 'ModelInfoId' INTEGER,'Name','Basetype'INTEGER,'Modeltype'INTEGER,'Path','Time',PRIMARY KEY('ID' AUTOINCREMENT))", Tablebase);
				

			using (var mySqlCommand = new MySqlCommand(s, connection))
			{
				mySqlCommand.ExecuteNonQuery();

			}
			connection.Close();
			connection.Dispose();
		}
		
	}

	public List<ModelInfo> SelectTable()
    {
		
			if (!IsHaveTable())
			CreatTable();

		    List<ModelInfo> _lstModelInfo = new List<ModelInfo>();

			string s = string.Format("Select * from {0}", Tablebase);

			using (var mySqlCommand = new MySqlCommand(s, connection))
			{
				using (MySqlDataReader dataReader = mySqlCommand.ExecuteReader())
				{
					while (dataReader.Read())
					{
						ModelInfo modelInfo = new ModelInfo();
						modelInfo.ID = dataReader.GetInt32(0);
						modelInfo.ModelIfoId= dataReader.GetInt32(1);
						modelInfo.Name = dataReader.GetString(2);
						modelInfo.Basetype = dataReader.GetInt32(3);
						modelInfo.Modeltype = dataReader.GetInt32(4);
						modelInfo.Path = dataReader.GetString(5);
						modelInfo.Time = dataReader.GetString(6);
						_lstModelInfo.Add(modelInfo);
					}
				}

			}
			connection.Close();
			connection.Dispose();
		    return _lstModelInfo;
		
	}


	public ModelInfo SelectTableByID(int id)
    {
		
			if (!IsHaveTable())
			CreatTable();

		    ModelInfo modelInfo = null;

			string s = string.Format("Select * from {0} where ID={1}", Tablebase, id);

			using (var mySqlCommand = new MySqlCommand(s, connection))
			{
				using (MySqlDataReader dataReader = mySqlCommand.ExecuteReader())
				{
					while (dataReader.Read())
					{
					    modelInfo = new ModelInfo();
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
			connection.Close();
			connection.Dispose();
		    return modelInfo;
		
	}

	public List<ModelInfo> SelectTableByModelId(int modelID)
	{
		if (!IsHaveTable())
			CreatTable();
		List<ModelInfo> _lstModelInfo = new List<ModelInfo>();

		string s = string.Format("Select * from {0} where {1}", Tablebase, "ModelIfoId" + modelID);

		using (var mySqlCommand = new MySqlCommand(s, connection))
		{
			using (MySqlDataReader dataReader = mySqlCommand.ExecuteReader())
			{
				while (dataReader.Read())
				{
					ModelInfo modelInfo = new ModelInfo();
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
		connection.Close();
		connection.Dispose();


		return _lstModelInfo;
	}

	public int InsertTable(ModelInfo modelInfo)
	{
		if (!IsHaveTable())
			CreatTable();

		int temp = 0;

		string s = string.Format("insert into {0} values(@ID,@ModelIfoId,@Name,@Basetype,@Modeltype,@Path,@Time)", Tablebase);

		using (var mySqlCommand = new MySqlCommand(s, connection))
		{


			mySqlCommand.Parameters.AddWithValue("ID", null);
			mySqlCommand.Parameters.AddWithValue("ModelIfoId", modelInfo.ModelIfoId);
			mySqlCommand.Parameters.AddWithValue("Name", modelInfo.Name);
			mySqlCommand.Parameters.AddWithValue("Basetype", modelInfo.Basetype);
			mySqlCommand.Parameters.AddWithValue("Modeltype", modelInfo.Modeltype);
			mySqlCommand.Parameters.AddWithValue("Path", modelInfo.Path);
			mySqlCommand.Parameters.AddWithValue("Time", modelInfo.Time);
			temp = mySqlCommand.ExecuteNonQuery();
		}
		connection.Close();
		connection.Dispose();
		if (temp > 0)
		{
			Debug.Log("插入成功");
		}
		return temp;
	}

	public int UpdateTable(ModelInfo modelInfo)
	{
		if (!IsHaveTable())
			CreatTable();

		int temp = 0;

		string s = string.Format("update  {0} set ModelIfoId=@ModelIfoId,Name=@Name,Basetype=@Basetype,Modeltype=@Modeltype,Path=@Path,Time=@Time)",
			"ID=" + modelInfo.ID);

		using (var mySqlCommand = new MySqlCommand(s, connection))
		{

			mySqlCommand.Parameters.AddWithValue("ModelIfoId", modelInfo.ModelIfoId);
			mySqlCommand.Parameters.AddWithValue("Name", modelInfo.Name);
			mySqlCommand.Parameters.AddWithValue("Basetype", modelInfo.Basetype);
			mySqlCommand.Parameters.AddWithValue("Modeltype", modelInfo.Modeltype);
			mySqlCommand.Parameters.AddWithValue("Path", modelInfo.Path);
			mySqlCommand.Parameters.AddWithValue("Time", modelInfo.Time);
			temp = mySqlCommand.ExecuteNonQuery();

		}
		connection.Close();
		connection.Dispose();
		if (temp >0)
		{
			Debug.Log("更新成功");
		}
		return temp;
	}

	public int deleteTable(int ID)
	{
		if (!IsHaveTable())
			CreatTable();

		int temp = -1;


		string s = string.Format("delete from  {0} where{1}", Tablebase, "ID=" + ID);
		using (var mySqlCommand = new MySqlCommand(s, connection))
		{

			temp = mySqlCommand.ExecuteNonQuery();

		}

        if (temp >-1)
        {
			Debug.Log("删除成功");
		}
		return temp;

	}

}
