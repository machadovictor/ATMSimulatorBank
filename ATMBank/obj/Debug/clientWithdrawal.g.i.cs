﻿#pragma checksum "..\..\clientWithdrawal.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "453C1E4606E77A2CD5A8A8A8ECA3103DCBD5C4BE2C0DA58F330D633E516BD421"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

using ATMBank;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace ATMBank {
    
    
    /// <summary>
    /// clientWithdrawal
    /// </summary>
    public partial class clientWithdrawal : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\clientWithdrawal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textBox;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\clientWithdrawal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox wdAccountsList;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\clientWithdrawal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox wdAmount;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\clientWithdrawal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox wdAccountBalance;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\clientWithdrawal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnWdSave;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\clientWithdrawal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnWdCancel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ATMBank;component/clientwithdrawal.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\clientWithdrawal.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.textBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.wdAccountsList = ((System.Windows.Controls.ComboBox)(target));
            
            #line 16 "..\..\clientWithdrawal.xaml"
            this.wdAccountsList.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.wdAccountsList_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.wdAmount = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.wdAccountBalance = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.btnWdSave = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\clientWithdrawal.xaml"
            this.btnWdSave.Click += new System.Windows.RoutedEventHandler(this.btnWdSave_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnWdCancel = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\clientWithdrawal.xaml"
            this.btnWdCancel.Click += new System.Windows.RoutedEventHandler(this.btnWdCancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

