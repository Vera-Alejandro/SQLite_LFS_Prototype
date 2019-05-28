using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;

namespace Interstates.Control.Database
{
    /// <summary>
    /// Summary description for ConnectionStringUIDialog.
    /// </summary>
    [ComVisible(false)]
    public class ConnectionStringUIDialog : System.ComponentModel.Component
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container _components = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="container"></param>
        public ConnectionStringUIDialog(System.ComponentModel.IContainer container)
        {
            // Required for Windows.Forms Class Composition Designer support
            container.Add(this);
            InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        public ConnectionStringUIDialog()
        {
            // Required for Windows.Forms Class Composition Designer support
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private string mstrConnectionString = string.Empty;

        /// <summary>
        /// The connection string to edit.
        /// </summary>
        [Browsable(true)]
        [Editor(typeof(ConnectionStringUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ConnectionString
        {
            get { return mstrConnectionString; }
            set
            {
                mstrConnectionString = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        static public DialogResult ShowDialog(ref string connectionString)
        {
            ConnectionType connectionType = new ConnectionType("SQL", connectionString);
            connectionType.ConnectionStringBuilder.ConnectionString = connectionString;
            ConnectionStringBuilderUIEditor frmStringBuilder = new ConnectionStringBuilderUIEditor();

            frmStringBuilder.ConnectionType = connectionType;

            DialogResult result = frmStringBuilder.ShowDialog();
            if (result == DialogResult.OK)
                connectionString = frmStringBuilder.ConnectionType.ConnectionStringBuilder.ConnectionString;

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        static public DialogResult ShowDialog(ref ConnectionType connectionType)
        {
            ConnectionStringBuilderUIEditor frmStringBuilder = new ConnectionStringBuilderUIEditor();

            frmStringBuilder.ConnectionType = connectionType;

            DialogResult result = frmStringBuilder.ShowDialog();
            if (result == DialogResult.OK)
                connectionType = frmStringBuilder.ConnectionType;

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public virtual DialogResult ShowDialog()
        {
            return ConnectionStringUIDialog.ShowDialog(ref mstrConnectionString);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _components = new System.ComponentModel.Container();
        }
        #endregion
    }

    /// <summary>
    ///
    /// </summary>
    [ComVisible(false)]
    public class ConnectionStringUIEditor : System.Drawing.Design.UITypeEditor
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override System.Object EditValue(
            System.ComponentModel.ITypeDescriptorContext context,
            System.IServiceProvider provider, System.Object value)
        {
            if (value is ConnectionType)
            {
                ConnectionType type = value as ConnectionType;

                if (ConnectionStringUIDialog.ShowDialog(ref type) == DialogResult.OK)
                {
                    return type;
                }
                else
                {
                    return value;
                }
            }
            else
            {
                string strConnectionString = (value == null) ? "" : value.ToString();

                if (ConnectionStringUIDialog.ShowDialog(ref strConnectionString) == DialogResult.OK)
                {
                    return strConnectionString;
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Design.UITypeEditorEditStyle
        GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return false;
        }
    }

    /// <summary>
    ///
    /// </summary>
    [ComVisible(false)]
    public class ConnectionTypeUIEditor : System.Drawing.Design.UITypeEditor
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override System.Object EditValue(
            System.ComponentModel.ITypeDescriptorContext context,
            System.IServiceProvider provider, System.Object value)
        {
            ConnectionType connectionType = value as ConnectionType;

            if (ConnectionStringUIDialog.ShowDialog(ref connectionType) == DialogResult.OK)
            {
                return connectionType;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Design.UITypeEditorEditStyle
        GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return false;
        }
    }

    public class ProviderConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, 
            //but allow free-form entry
            return true;
        }

        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(DatabaseConfiguration.ProviderTypes);
        }
    }
}
