using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using C5;

namespace Library {
	public class MainTables : Form {
		private DataSet dataSet = new DataSet ();
		private SqlDataAdapter parendSDA = new SqlDataAdapter ();
		private SqlDataAdapter childSDA = new SqlDataAdapter ();
		private SqlConnection cnn;

		private Dictionary<String, TextBox> textBoxes = new Dictionary<String, TextBox> ();
		private DataConnectionModel myData = new DataConnectionModel ();

		public MainTables () {
			this.Load += new EventHandler (MainTables_Load);
		}

		private Panel buttonPanel = new Panel ();
		private DataGridView parentDataGridView = new DataGridView ();
		private DataGridView childDataGridView = new DataGridView ();

		private void MainTables_Load (System.Object sender, System.EventArgs e) {
			SetupLayout ();
			SetupDataGridView ();
			PopulateDataGridView ();
		}

		private void addNewRowButton_Click (object sender, EventArgs e) {
			try {
				childSDA.InsertCommand = new SqlCommand (myData.Querys ["InsertChild"], cnn);
				foreach (String parameter in myData.Parameters) {
					childSDA.InsertCommand.Parameters.Add (new SqlParameter {
							ParameterName = "@" + parameter,
							Value = textBoxes [parameter].Text,
							SqlDbType = myData.ParametersType [parameter],
							Size = 75
						});
					if (parameter != myData.ConnectionParameter)
						textBoxes [parameter].Text = "";
				}
				childSDA.InsertCommand.ExecuteNonQuery ();
				this.reloadChildren ();
			} catch (Exception err) {
				MessageBox.Show (err.Message, "Error!", MessageBoxButtons.OK);
			}
		}

		private void reloadChildren () {
			try {
				childDataGridView.DataSource = null;
				if (dataSet.Tables ["Child"] != null)
					dataSet.Tables ["Child"].Clear ();
				childSDA.SelectCommand = new SqlCommand (myData.Querys ["SelectChild"], cnn);
				childSDA.SelectCommand.Parameters.Add (new SqlParameter {
						ParameterName = myData.ConnectionParameter,
						Value = parentDataGridView.CurrentRow.Cells [myData.ConnectionParameter].Value.ToString (),
						SqlDbType = SqlDbType.BigInt,
						Size = 50
					});
				childSDA.Fill (dataSet, "Child");
				childDataGridView.DataSource = dataSet.Tables ["Child"].DefaultView;
				if (this.childDataGridView.CurrentRow != null) {
					foreach (DataGridViewColumn column in this.childDataGridView.Columns) {
						if (this.textBoxes [column.Name] != null) {
							this.textBoxes [column.Name].Text = this.childDataGridView.CurrentRow.Cells [column.Name].Value.ToString ();
						} 
					}
				} else {
					foreach (String parameter in myData.Parameters) {
						if (parameter != myData.ConnectionParameter)
							textBoxes [parameter].Text = "";
					}
				}
			} catch (Exception e) {
				MessageBox.Show (e.Message, "Error!", MessageBoxButtons.OK);
			}
		}

		private void deleteRowButton_Click (object sender, EventArgs e) {
			try {
				childSDA.DeleteCommand = new SqlCommand (myData.Querys ["DeleteChild"], cnn);
				foreach (String parameter in myData.Parameters) {
					childSDA.DeleteCommand.Parameters.Add (new SqlParameter {
							ParameterName = "@" + parameter,
							Value = textBoxes [parameter].Text,
							SqlDbType = myData.ParametersType [parameter],
							Size = 75
						});
					if (parameter != myData.ConnectionParameter)
						textBoxes [parameter].Text = "";
				}
				childSDA.DeleteCommand.ExecuteNonQuery ();
			} catch (Exception err) {
				MessageBox.Show (err.Message, "Error!", MessageBoxButtons.OK);
			}
			this.reloadChildren ();

		}

		private void updateRowButton_Click (object sender, EventArgs e) {
			try {
				childSDA.UpdateCommand = new SqlCommand (myData.Querys ["UpdateChild"], cnn);
				foreach (String parameter in myData.Parameters) {
					childSDA.UpdateCommand.Parameters.Add (new SqlParameter {
							ParameterName = "@" + parameter,
							Value = textBoxes [parameter].Text,
							SqlDbType = myData.ParametersType [parameter],
							Size = 75
						});
					if (parameter != myData.ConnectionParameter)
						textBoxes [parameter].Text = "";
				}
				childSDA.UpdateCommand.ExecuteNonQuery ();
			} catch (Exception err) {
				MessageBox.Show (err.Message, "Error!", MessageBoxButtons.OK);
			}
			this.reloadChildren ();
		}

		private void SetupLayout () {
			this.Size = new Size (1500, 500);
			Button addNewRowButton = new Button ();
			Button deleteRowButton = new Button ();
			Button updateRowButton = new Button ();
	
			int y = 90;
			foreach (String name in myData.Parameters) {
				TextBox textBox = new TextBox ();
				Label label = new Label ();
				textBox.Location = new Point (60, y);
				label.Location = new Point (0, y);
				y += 30;
				label.Text = name;
				textBox.Tag = name;
				buttonPanel.Controls.Add (textBox);
				buttonPanel.Controls.Add (label);
				textBoxes.Add (name, textBox);
			}

			addNewRowButton.Text = "Add Row";
			addNewRowButton.Location = new Point (10, 0);
			addNewRowButton.Click += new EventHandler (addNewRowButton_Click);

			deleteRowButton.Text = "Delete Row";
			deleteRowButton.Location = new Point (10, 30);
			deleteRowButton.Click += new EventHandler (deleteRowButton_Click);

			updateRowButton.Text = "Update Row";
			updateRowButton.Location = new Point (10, 60);
			updateRowButton.Click += new EventHandler (updateRowButton_Click);

			buttonPanel.Controls.Add (addNewRowButton);
			buttonPanel.Controls.Add (deleteRowButton);
			buttonPanel.Controls.Add (updateRowButton);

			buttonPanel.Height = 50;
			buttonPanel.Dock = DockStyle.Right;
		
			this.Controls.Add (this.buttonPanel);
		}

		private void parentDataGridView_Click (object sender, EventArgs e) {
			this.textBoxes [myData.ConnectionParameter].Text = parentDataGridView.CurrentRow.Cells [myData.ConnectionParameter].Value.ToString ();
			this.reloadChildren ();
		}

		private void childDataGridView_Click (object sender, EventArgs e) {
			foreach (DataGridViewColumn column in this.childDataGridView.Columns) {
				if (this.textBoxes [column.Name] != null && this.childDataGridView.CurrentRow.Cells [column.Name] != null) {
					this.textBoxes [column.Name].Text = this.childDataGridView.CurrentRow.Cells [column.Name].Value.ToString ();
				}
			}
		}

		private void SetupDataGridView () {
			this.Controls.Add (parentDataGridView);
			this.Controls.Add (childDataGridView);

			parentDataGridView.Name = "parentDataGridView";
			parentDataGridView.Size = new Size (600, 250);
			parentDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			parentDataGridView.MultiSelect = false;
			parentDataGridView.Dock = DockStyle.Left;
			parentDataGridView.AutoGenerateColumns = true;

			parentDataGridView.Click += new EventHandler (parentDataGridView_Click);

			childDataGridView.Name = "childDataGridView";
			childDataGridView.Size = new Size (600, 250);
			childDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			childDataGridView.MultiSelect = false;
			childDataGridView.Dock = DockStyle.Right;
			childDataGridView.AutoGenerateColumns = true;

			childDataGridView.Click += new EventHandler (childDataGridView_Click);
		}

		private void PopulateDataGridView () {
			cnn = new SqlConnection (myData.ConnectionString);
			cnn.Open ();

			parendSDA.SelectCommand = new SqlCommand (myData.Querys ["SelectParent"], cnn);
			parendSDA.Fill (dataSet, "Parent");
			parentDataGridView.DataSource = dataSet.Tables ["Parent"].DefaultView;
			if (parentDataGridView.CurrentRow != null) {
				this.textBoxes [myData.ConnectionParameter].Text = parentDataGridView.CurrentRow.Cells [myData.ConnectionParameter].Value.ToString ();
			}
			this.reloadChildren ();
		}
	}
}

