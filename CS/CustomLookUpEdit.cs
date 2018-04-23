using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraEditors.ListControls;
using System.Windows.Forms;
using System.Collections;

namespace TestLookUpIncrementalSearch {
    [UserRepositoryItem("RegisterCustomEdit")]
    public class RepositoryItemCustomLookUpEdit : RepositoryItemLookUpEdit {

        //The static constructor that calls the registration method 
        static RepositoryItemCustomLookUpEdit() { RegisterCustomEdit(); }

        //Initialize new properties 
        public RepositoryItemCustomLookUpEdit() {

        }

        //The unique name for the custom editor 
        public const string CustomEditName = "CustomLookUpEdit";

        //Return the unique name 
        public override string EditorTypeName { get { return CustomEditName; } }

        //Register the editor 
        public static void RegisterCustomEdit() {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(CustomEditName,
              typeof(CustomLookUpEdit), typeof(RepositoryItemCustomLookUpEdit),
              typeof(LookUpEditViewInfo), new ButtonEditPainter(), true, null));
        }


        //Override the Assign method 
        public override void Assign(RepositoryItem item) {
            BeginUpdate();
            try {
                base.Assign(item);
                RepositoryItemCustomLookUpEdit source = item as RepositoryItemCustomLookUpEdit;
                if(source == null)
                    return;
            } finally {
                EndUpdate();
            }
        }


    }

    [ToolboxItem(true)]
    public class CustomLookUpEdit : LookUpEdit {

        //The static constructor that calls the registration method 
        static CustomLookUpEdit() { RepositoryItemCustomLookUpEdit.RegisterCustomEdit(); }

        //Initialize the new instance 
        public CustomLookUpEdit() {
            //... 
            this.ViewInfo.UseHighlightSearchAppearance = true;
        }

        //Return the unique name 
        public override string EditorTypeName {
            get {
                return
                    RepositoryItemCustomLookUpEdit.CustomEditName;
            }
        }

        protected override PopupBaseForm CreatePopupForm() {
            return new CustomPopupLookUpEditForm(this);
        }

        //Override the Properties property 
        //Simply type-cast the object to the custom repository item type 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new RepositoryItemCustomLookUpEdit Properties {
            get { return base.Properties as RepositoryItemCustomLookUpEdit; }
        }

        string lastChar;
        protected override void OnPreviewKeyDown(System.Windows.Forms.PreviewKeyDownEventArgs e) {
            CustomPopupLookUpEditForm form = this.PopupForm as CustomPopupLookUpEditForm;
            base.OnPreviewKeyDown(e);
            if(char.IsLetterOrDigit((char)e.KeyCode)) {
                lastChar = ((char)e.KeyCode).ToString();
            } else if(e.KeyData == Keys.Back) {
                lastChar = "backspace";
            } else {
                lastChar = "";
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            CustomPopupLookUpEditForm form = this.PopupForm as CustomPopupLookUpEditForm;
            if(lastChar == "backspace") {
                e.Handled = true;
                form.IncrementalSearchText = form.IncrementalSearchText.Remove(form.IncrementalSearchText.Length - 1);
            }
            base.OnKeyPress(e);
            if(lastChar != "backspace") {
                form.IncrementalSearchText += lastChar;
            }
        }

        public override void ClosePopup() {
            base.ClosePopup();
        }

    }

    public class CustomPopupLookUpEditForm : PopupLookUpEditForm {
        private string _IncrementalSearchText = String.Empty;
        public CustomPopupLookUpEditForm(LookUpEdit ownerEdit)
            : base(ownerEdit) {

        }
        public string IncrementalSearchText {
            get { return _IncrementalSearchText; }
            set {
                _IncrementalSearchText = value;
            }
        }
        public string GetSearchText() {
            return this.SearchText;
        }

        protected override bool DoIncrementalSearch(char keyChar, bool ctrl, bool shift) {
            bool result = base.DoIncrementalSearch(keyChar, ctrl, shift);
            selectedRowIndex = this.SelectedIndex;
            return result;
        }
        int selectedRowIndex = -1;

        protected override object QueryResultValue() {
            string currentValue = String.Empty;
            try {
                IList list = ListBindingHelper.GetList(this.OwnerEdit.Properties.DataSource) as IList;
                PropertyDescriptorCollection pdc = ListBindingHelper.GetListItemProperties(this.OwnerEdit.Properties.DataSource);
                PropertyDescriptor pd = pdc[this.OwnerEdit.Properties.Columns[this.OwnerEdit.Properties.AutoSearchColumnIndex].FieldName];
                currentValue = pd.GetValue(list[selectedRowIndex]).ToString();
            } catch { }
            if(!this.OwnerEdit.Properties.CaseSensitiveSearch) {
                currentValue = currentValue.ToLower();
                this.IncrementalSearchText = this.IncrementalSearchText.ToLower();
            }
            if(this.IncrementalSearchText == "" || currentValue.StartsWith(this.IncrementalSearchText)) {
                this.IncrementalSearchText = "";
                return base.QueryResultValue();
            } else {
                this.IncrementalSearchText = "";
                return base.QueryOldEditValue();
            }
        }


    }
}
