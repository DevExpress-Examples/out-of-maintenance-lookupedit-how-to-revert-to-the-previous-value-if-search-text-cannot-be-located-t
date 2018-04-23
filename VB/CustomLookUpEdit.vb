Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Drawing
Imports DevExpress.XtraEditors.Popup
Imports DevExpress.XtraEditors.Registrator
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraEditors.ViewInfo
Imports DevExpress.XtraEditors.Controls
Imports System.Reflection
Imports DevExpress.Utils
Imports DevExpress.XtraEditors.ListControls
Imports System.Windows.Forms
Imports System.Collections

Namespace TestLookUpIncrementalSearch
	<UserRepositoryItem("RegisterCustomEdit")>
	Public Class RepositoryItemCustomLookUpEdit
		Inherits RepositoryItemLookUpEdit

		'The static constructor that calls the registration method 
		Shared Sub New()
			RegisterCustomEdit()
		End Sub

		'Initialize new properties 
		Public Sub New()

		End Sub

		'The unique name for the custom editor 
		Public Const CustomEditName As String = "CustomLookUpEdit"

		'Return the unique name 
		Public Overrides ReadOnly Property EditorTypeName() As String
			Get
				Return CustomEditName
			End Get
		End Property

		'Register the editor 
        Public Shared Sub RegisterCustomEdit()
            Dim img As Image = Nothing
            EditorRegistrationInfo.Default.Editors.Add(New EditorClassInfo(CustomEditName, GetType(CustomLookUpEdit), GetType(RepositoryItemCustomLookUpEdit), GetType(LookUpEditViewInfo), New ButtonEditPainter(), True, img))
        End Sub


		'Override the Assign method 
		Public Overrides Sub Assign(ByVal item As RepositoryItem)
			BeginUpdate()
			Try
				MyBase.Assign(item)
				Dim source As RepositoryItemCustomLookUpEdit = TryCast(item, RepositoryItemCustomLookUpEdit)
				If source Is Nothing Then
					Return
				End If
			Finally
				EndUpdate()
			End Try
		End Sub


	End Class

	<ToolboxItem(True)>
	Public Class CustomLookUpEdit
		Inherits LookUpEdit

		'The static constructor that calls the registration method 
		Shared Sub New()
			RepositoryItemCustomLookUpEdit.RegisterCustomEdit()
		End Sub

		'Initialize the new instance 
		Public Sub New()
			'... 
			Me.ViewInfo.UseHighlightSearchAppearance = True
		End Sub

		'Return the unique name 
		Public Overrides ReadOnly Property EditorTypeName() As String
			Get
				Return RepositoryItemCustomLookUpEdit.CustomEditName
			End Get
		End Property

		Protected Overrides Function CreatePopupForm() As PopupBaseForm
			Return New CustomPopupLookUpEditForm(Me)
		End Function

		'Override the Properties property 
		'Simply type-cast the object to the custom repository item type 
		<DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
		Public Shadows ReadOnly Property Properties() As RepositoryItemCustomLookUpEdit
			Get
				Return TryCast(MyBase.Properties, RepositoryItemCustomLookUpEdit)
			End Get
		End Property

		Private lastChar As String
		Protected Overrides Sub OnPreviewKeyDown(ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs)
			Dim form As CustomPopupLookUpEditForm = TryCast(Me.PopupForm, CustomPopupLookUpEditForm)
			MyBase.OnPreviewKeyDown(e)
            If Char.IsLetterOrDigit(ChrW(e.KeyCode)) Then
                lastChar = ChrW(e.KeyCode).ToString()
            ElseIf e.KeyData = Keys.Back Then
                lastChar = "backspace"
            Else
                lastChar = ""
            End If
		End Sub

		Protected Overrides Sub OnKeyPress(ByVal e As KeyPressEventArgs)
			Dim form As CustomPopupLookUpEditForm = TryCast(Me.PopupForm, CustomPopupLookUpEditForm)
			If lastChar = "backspace" Then
				e.Handled = True
				form.IncrementalSearchText = form.IncrementalSearchText.Remove(form.IncrementalSearchText.Length - 1)
			End If
			MyBase.OnKeyPress(e)
			If lastChar <> "backspace" Then
				form.IncrementalSearchText &= lastChar
			End If
		End Sub

		Public Overrides Sub ClosePopup()
			MyBase.ClosePopup()
		End Sub

	End Class

	Public Class CustomPopupLookUpEditForm
		Inherits PopupLookUpEditForm

		Private _IncrementalSearchText As String = String.Empty
		Public Sub New(ByVal ownerEdit As LookUpEdit)
			MyBase.New(ownerEdit)

		End Sub
		Public Property IncrementalSearchText() As String
			Get
				Return _IncrementalSearchText
			End Get
			Set(ByVal value As String)
				_IncrementalSearchText = value
			End Set
		End Property
		Public Function GetSearchText() As String
			Return Me.SearchText
		End Function

		Protected Overrides Function DoIncrementalSearch(ByVal keyChar As Char, ByVal ctrl As Boolean, ByVal shift As Boolean) As Boolean
			Dim result As Boolean = MyBase.DoIncrementalSearch(keyChar, ctrl, shift)
			selectedRowIndex = Me.SelectedIndex
			Return result
		End Function
		Private selectedRowIndex As Integer = -1

		Protected Overrides Function QueryResultValue() As Object
            Dim _currentValue As String = String.Empty
            Try
                Dim list As IList = TryCast(ListBindingHelper.GetList(Me.OwnerEdit.Properties.DataSource), IList)
                Dim pdc As PropertyDescriptorCollection = ListBindingHelper.GetListItemProperties(Me.OwnerEdit.Properties.DataSource)
                Dim pd As PropertyDescriptor = pdc(Me.OwnerEdit.Properties.Columns(Me.OwnerEdit.Properties.AutoSearchColumnIndex).FieldName)
                _currentValue = pd.GetValue(list(selectedRowIndex)).ToString()
            Catch
            End Try
            If Not Me.OwnerEdit.Properties.CaseSensitiveSearch Then
                _currentValue = _currentValue.ToLower()
                Me.IncrementalSearchText = Me.IncrementalSearchText.ToLower()
            End If
            If Me.IncrementalSearchText = "" OrElse _currentValue.StartsWith(Me.IncrementalSearchText) Then
                Me.IncrementalSearchText = ""
                Return MyBase.QueryResultValue()
            Else
                Me.IncrementalSearchText = ""
                Return MyBase.QueryOldEditValue()
            End If
		End Function


	End Class
End Namespace
