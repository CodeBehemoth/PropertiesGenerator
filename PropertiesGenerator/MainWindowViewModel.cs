using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PropertiesGenerator
{
    /// <summary>
    /// Data class to describe a ViewModel-Property
    /// </summary>
    public class PropertyDesription : ViewModelBase
    {
        public string PropertyType
        {
            get
            {
                return myPropertyType;
            }
            set
            {
                myPropertyType = value;
                RaisePropertyChanged( nameof( PropertyType ) );
            }
        }
        string myPropertyType;

        public string Name
        {
            get
            {
                return myName;
            }
            set
            {
                myName = value;
                RaisePropertyChanged();
            }
        }
        string myName;

        public string Comment
        {
            get
            {
                return myComment;
            }
            set
            {
                myComment = value;
                RaisePropertyChanged();
            }
        }
        string myComment;

        public PropertyDesription( string thePropertyType, string theName, string theComment )
        {
            PropertyType = thePropertyType;
            Name = theName;
            Comment = theComment;
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        private static string _ = new String( ' ', 4 );
        private InfoView myInfoView;

        public ICommand CommandAddRow { get; private set; }
        public ICommand CommandGenerateCode { get; private set; }
        public ICommand CommandCopyToClipboard { get; private set; }

        public ICommand CommandShowInfo { get; private set; }
        public ICommand CommandCloseInfo { get; private set; }
        public ICommand CommandCopyInfoCodeToClipboard { get; private set; }



        public ObservableCollection<string> SupportedTypes
        {
            get { return mySupportedTypes; }
            set
            {
                mySupportedTypes = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<string> mySupportedTypes;

        public ObservableCollection<PropertyDesription> Properties
        {
            get
            {
                return myProps;
            }
            set
            {
                myProps = value;
                RaisePropertyChanged( nameof( Properties ) );
            }
        }
        private ObservableCollection<PropertyDesription> myProps = new ObservableCollection<PropertyDesription>();



        public string SelectedTemplate
        {
            get { return Templates[SelectedTemplateKey]; }
        }



        public string SelectedTemplateKey
        {
            get
            {
                return mySelectedTemplateKey;
            }
            set
            {
                mySelectedTemplateKey = value;
                RaisePropertyChanged();
                RaisePropertyChanged( nameof( SelectedTemplate ) );
                RaisePropertyChanged( nameof( IsInfoButtonVisible ) );
            }
        }
        private string mySelectedTemplateKey;



        public Dictionary<string, string> Templates
        {
            get { return myTemplates; }
            set { myTemplates = value; RaisePropertyChanged(); }
        }
        private Dictionary<string, string> myTemplates;

        /// <summary>
        ///Prefix for private field
        /// </summary>
        public string PrivatePrefix
        {
            get
            {
                return myPrivatePrefix;
            }
            set
            {
                myPrivatePrefix = value;
                updateTemplates();
                RaisePropertyChanged();
            }
        }
        private string myPrivatePrefix;

        /// <summary>
        /// is C#6.0 supported? 
        /// ( for using of nameof() )
        /// </summary>
        public bool IsCS6
        {
            get { return isCS6; }
            set { if ( IsCS6 != value ) { isCS6 = value; RaisePropertyChanged(); } }
        }
        private bool isCS6;

        /// <summary>
        /// Generated source code
        /// </summary>
        public string ResultSourceCode
        {
            get { return myCode; }
            set { myCode = value; RaisePropertyChanged(); }
        }
        private string myCode;

        /// <summary>
        /// Text in Info View
        /// </summary>
        public string InfoText
        {
            get { return myInfoText; }
            set { myInfoText = value; RaisePropertyChanged(); }
        }
        private string myInfoText;

        /// <summary>
        /// Source code in Info View
        /// </summary>
        public string InfoCode
        {
            get { return myInfoCode; }
            set { myInfoCode = value; RaisePropertyChanged(); }
        }
        private string myInfoCode;

        public bool IsInfoButtonVisible
        {
            get { return SelectedTemplateKey == "CallerMemberName"; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel()
        {
            Templates = new Dictionary<string, string>();

            SupportedTypes = new ObservableCollection<string>()
            {
                "string",
                "bool",
                "int", "long",
                "double", "float", "decimal",
                "object",
                "char",
                "uint", "ulong", "short", "ushort", "byte", "sbyte",
            };

            PrivatePrefix = "m_";  // inkl. updateTemplate();

            //create some rows
            for ( int i = 0; i < 10; i++ )
            {
                AddRow();
            }

            CommandAddRow = new RelayCommand( p1 => AddRow() );

            CommandGenerateCode = new RelayCommand( p1 =>
             {
                 ResultSourceCode = "";
                 foreach ( var aProp in Properties )
                 {
                     if ( !String.IsNullOrEmpty( aProp.Name ) )
                     {
                         ResultSourceCode += SelectedTemplate.Replace( "#Name#", aProp.Name ).
                                          Replace( "#Comment#", aProp.Comment ).
                                          Replace( "#Type#", aProp.PropertyType );
                         ResultSourceCode += Environment.NewLine;
                     }
                 }
             } );

            CommandCopyToClipboard = new RelayCommand( p1 => Clipboard.SetText( ResultSourceCode ) );

            ResultSourceCode = Environment.NewLine + "   Fill out the table and then press 'Generate Code'";

            myInfoView = new InfoView();
            myInfoView.DataContext = this;
            InfoText = "To use this syntax, your class should be inherited from ViewModelBase ( see code below ).";
            InfoCode = 
                myViewModelBaseCode + Environment.NewLine +
                myClassBeginCode + Environment.NewLine +
                _ + "//your code here " + Environment.NewLine + Environment.NewLine +
                myClassEndCode;
            CommandShowInfo = new RelayCommand( p => myInfoView.Show() );
            CommandCopyInfoCodeToClipboard = new RelayCommand( p1 => Clipboard.SetText( InfoCode ) );
            CommandCloseInfo = new RelayCommand( p => myInfoView.Hide() );
        }

        private string myViewModelBaseCode =
                "public abstract class ViewModelBase : INotifyPropertyChanged" + Environment.NewLine +
                "{" + Environment.NewLine +
                _ + "public event PropertyChangedEventHandler PropertyChanged;" + Environment.NewLine +
                Environment.NewLine +
                _ + "public void RaisePropertyChanged( [CallerMemberName] string thePropertyName = \"\" )" + Environment.NewLine +
                _ + "{" + Environment.NewLine +
                _ + _ + "PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( thePropertyName ) );" + Environment.NewLine +
                _ + "}" + Environment.NewLine +
                "}" + Environment.NewLine;

        private readonly string myClassBeginCode =
                "public class CLASS_NAME : ViewModelBase" + Environment.NewLine +
                "{" + Environment.NewLine;

        private readonly string myClassEndCode =
                "}" + Environment.NewLine;

        private void updateTemplates()
        {
            Templates["default"] = getBaseTemplate( 0, "nameof(#Name#)" );

            Templates["compact"] =
                _ + "public #Type# #Name# { get { return " + PrivatePrefix + "#Name#; } set { " + PrivatePrefix + "#Name# = value; RaisePropertyChanged(); } } private #Type# " + PrivatePrefix + "#Name#;";

            Templates["CallerMemberName"] =
                myViewModelBaseCode + Environment.NewLine +
                myClassBeginCode +
                getBaseTemplate( 1, "" ) +
                myClassEndCode;


            if ( String.IsNullOrEmpty( SelectedTemplateKey ) )
            {
                SelectedTemplateKey = "default";
            }

            RaisePropertyChanged( nameof( SelectedTemplate ) );
        }


        private string getBaseTemplate( int additionalIndents, string raisePropertyChangedParam )
        {
            string _ = new String( ' ', ( 1 + additionalIndents ) * 4 );

            return _ + "/// <summary>" + Environment.NewLine +
                   _ + "/// #Comment#" + Environment.NewLine +
                   _ + "/// </summary>" + Environment.NewLine +
                   _ + "public #Type# #Name#" + Environment.NewLine +
                   _ + "{" + Environment.NewLine +
                   _ + _ + "get" + Environment.NewLine +
                   _ + _ + "{" + Environment.NewLine +
                   _ + _ + _ + "return " + PrivatePrefix + "#Name#;" + Environment.NewLine +
                   _ + _ + "}" + Environment.NewLine +
                   _ + _ + "set" + Environment.NewLine +
                   _ + _ + "{" + Environment.NewLine +
                   _ + _ + _ + "if ( #Name# != value )" + Environment.NewLine +
                   _ + _ + _ + "{" + Environment.NewLine +
                   _ + _ + _ + _ + PrivatePrefix + "#Name# = value;" + Environment.NewLine +
                   _ + _ + _ + _ + "RaisePropertyChanged(nameof(#Name#));" + Environment.NewLine +
                   _ + _ + _ + "}" + Environment.NewLine +
                   _ + _ + "}" + Environment.NewLine +
                   _ + "}" + Environment.NewLine +
                   _ + "private #Type# " + PrivatePrefix + "#Name#;" + Environment.NewLine;
        }

        internal void AddRow()
        {
            Properties.Add( new PropertyDesription( "string", "", "" ) );
        }

    }
}
