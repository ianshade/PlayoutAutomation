﻿using System.Windows.Input;

namespace TAS.Client.Common
{
    public abstract class OkCancelViewModelBase : DialogViewModelBase
    {              
        public bool OkCancelButtonsActivateViaKeyboard { get; set; } = true;
        
        /// <summary>
        /// OkCancelViewModelBase provide basic support for Ok/Cancel concept dialog
        /// </summary>        
        /// <param name="okButtonContent">Ok button content</param>
        /// <param name="cancelButtonContent">Cancel button content</param>
        public OkCancelViewModelBase(string okButtonContent = "Ok", string cancelButtonContent = "Cancel")
        {                        
            LoadCommands();
           
            OkButtonContent = okButtonContent;            
            CancelButtonContent = cancelButtonContent;         
        }

        private void LoadCommands()
        {
            CommandOk = new UiCommand((obj) =>
            {
                if (Ok(obj))
                    base.DialogConfirm();
            }, CanOk);

            CommandCancel = new UiCommand((obj) => 
            {
                Cancel(obj);
                base.DialogClose();  
            }, CanCancel);            
        }

        public virtual bool Ok(object obj) { return true; }
        public virtual bool CanOk(object obj) { return IsModified; }
        public virtual bool CanCancel(object obj) { return true; }
        public virtual void Cancel(object obj) { } 

        public ICommand CommandCancel { get; protected set; }
        public ICommand CommandOk { get; protected set; }

        public string OkButtonContent { get; }
        public string CancelButtonContent { get; }
    }
}
