using System;
using C5;
using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace Library {
	public class DataConnectionModel {
		private ArrayList<String> parameters = new ArrayList<String> ();
		private Dictionary<String, SqlDbType> parametersType = new Dictionary<String, SqlDbType> ();
		private Dictionary<String, String> querys = new Dictionary<String, String> ();
		private String connectionParameter;
		private String connectionString;

		public DataConnectionModel () {
			loadData ();
		}

		private void loadData () {
			XmlDocument doc = new XmlDocument ();
			doc.Load ("../../src/config.xml");
			querys.Add ("InsertChild", doc.GetElementsByTagName ("InsertChild") [0].InnerText);
			querys.Add ("SelectChild", doc.GetElementsByTagName ("SelectChild") [0].InnerText);
			querys.Add ("DeleteChild", doc.GetElementsByTagName ("DeleteChild") [0].InnerText);
			querys.Add ("UpdateChild", doc.GetElementsByTagName ("UpdateChild") [0].InnerText);
			querys.Add ("SelectParent", doc.GetElementsByTagName ("SelectParent") [0].InnerText);
			connectionString = doc.GetElementsByTagName ("ConnectionString") [0].InnerText;
			connectionParameter = doc.GetElementsByTagName ("ConnectionParameter") [0].InnerText;
			foreach (XmlNode node in doc.GetElementsByTagName("parameter")) {
				String name = "";
				SqlDbType type = SqlDbType.BigInt;
				foreach (XmlNode childNode in node.ChildNodes) {
					if (childNode.Name == "name") {
						name = childNode.InnerText;
					} else if (childNode.Name == "type") {
							String ty = childNode.InnerText;
							if (ty.ToLower () == "bigint") {
								type = SqlDbType.BigInt;
							} else if (ty.ToLower () == "nvarchar") {
									type = SqlDbType.NVarChar;
								} else if (ty.ToLower () == "int") {
										type = SqlDbType.Int;
									} else if (ty.ToLower () == "varchar") {
											type = SqlDbType.VarChar;
										} else if (ty.ToLower () == "date") {
							type = SqlDbType.DateTime;
											}
						}
				}
				parameters.Add (name);
				parametersType.Add (name, type);
			}
			Console.WriteLine (querys);
			Console.WriteLine (connectionParameter);
			Console.WriteLine (parametersType.Values);
			Console.WriteLine (querys);
		}

		public ArrayList<String> Parameters {
			get {
				return parameters;
			}
		}

		public String ConnectionString {
			get {
				return connectionString;
			}
		}

		public String ConnectionParameter {
			get {
				return connectionParameter;
			}
		}

		public Dictionary<String, String> Querys {
			get {
				return querys;
			}
		}

		public Dictionary<String, SqlDbType> ParametersType {
			get {
				return parametersType;
			}
		}
	}
}

