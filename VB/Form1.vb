Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.LookAndFeel
Imports DevExpress.Skins
Imports DevExpress.XtraEditors

Namespace TestLookUpIncrementalSearch
	Partial Public Class Form1
		Inherits DevExpress.XtraEditors.XtraForm

		Public Sub New()
			InitializeComponent()

			Dim ritem As New RepositoryItemCustomLookUpEdit()
			ritem.DataSource = GetData(10)
			ritem.ValueMember = "Name"
			ritem.DisplayMember = "Name"
			ritem.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.OnlyInPopup
			ritem.AutoSearchColumnIndex = 3
			gridControl1.RepositoryItems.Add(ritem)

			gridControl1.DataSource = GetMainDataSource(10)
			gridView1.Columns("Name").ColumnEdit = ritem

		End Sub


		Private Function GetData(ByVal count As Integer) As DataTable
			Dim dt As New DataTable()
			dt.Columns.Add("ID", GetType(Integer))
			dt.Columns.Add("Name", GetType(String))
			dt.Columns.Add("Quantity", GetType(Integer))
			dt.Columns.Add("Info", GetType(String))
			Dim r As New Random()
			For i As Integer = 0 To (count \ 4) - 1
				dt.Rows.Add(i, "Name" & i, r.Next(100), "abc" & i)
			Next i
			Dim rowCount As Integer = dt.Rows.Count
			For i As Integer = rowCount To (rowCount + count \ 4) - 1
				dt.Rows.Add(i, "Name" & i, r.Next(100), "bcd" & i)
			Next i
			rowCount = dt.Rows.Count
			For i As Integer = rowCount To (rowCount + count \ 4) - 1
				dt.Rows.Add(i, "Name" & i, r.Next(100), "def" & i)
			Next i
			rowCount = dt.Rows.Count
			For i As Integer = rowCount To count - 1
				dt.Rows.Add(i, "Name" & i, r.Next(100), "efg" & i)
			Next i
			Return dt
		End Function

		Private Function GetMainDataSource(ByVal count As Integer) As List(Of MyObject)
			Dim list As New List(Of MyObject)()
			For i As Integer = 0 To count - 1
				Dim m As New MyObject() With {.ID = i, .Name = "Name" & i}
				   list.Add(m)
			Next i
			Return list
		End Function

	End Class

	Public Class MyObject
		Public Property ID() As Integer
		Public Property Name() As String
	End Class
End Namespace
