using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using DXVcsTools.UI.View;
using DXVcsTools.UI.ViewModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DXVcsTools {
    [Guid("ACBBE549-099F-4F74-9849-CEEBCE8D07E1")]
    public sealed class InternalBlameWindow : ToolWindowPane {
        bool isControlKeyDepressed = false;
        bool isShifKeyDepressed = false;
        bool isOtherKeyDepressed = false;
        bool isCommandCombinationDepressed = false;

        InternalBlameControl Control { get { return Content as InternalBlameControl; } }
        public InternalBlameWindow()
            : base(null) {
            Caption = "Blame window";
            Content = new InternalBlameControl();
        }
        public void Initialize(InternalBlameViewModel model) {
            Control.DataContext = model;
            Show();
        }
        public void Show() {
            var frame = (IVsWindowFrame)Frame;
            frame.Show();
        }
        protected override bool PreProcessMessage(ref Message msg) {
            // trap keyboard messages if window has focus
            if (msg.Msg == 256) {
                if (msg.WParam == (IntPtr)17) {
                    isControlKeyDepressed = true;
                    isOtherKeyDepressed = false;
                }
                else if (msg.WParam == (IntPtr)16) {
                    isShifKeyDepressed = true;
                    isOtherKeyDepressed = false;
                }
                else {
                    if (isOtherKeyDepressed) {
                        isControlKeyDepressed = false;
                        isShifKeyDepressed = false;
                    }
                    isOtherKeyDepressed = true;
                    if (isControlKeyDepressed) {
                        if (isShifKeyDepressed) {
                            switch (msg.WParam.ToInt64()) {
                                case 65: // Ctrl+Shit+A command
                                case 67: // Ctrl+Shit+C command
                                case 78: // Ctrl+Shit+N command
                                case 79: // Ctrl+Shit+O command
                                case 83: // Ctrl+Shit+S command
                                case 85: // Ctrl+Shit+U command
                                case 88: // Ctrl+Shit+X command
                                    isCommandCombinationDepressed = true;
                                    break;
                                default:
                                    isCommandCombinationDepressed = false;
                                    break;
                            }
                        }
                        else {
                            switch (msg.WParam.ToInt64()) {
                                case 70: // Ctrl+E command
                                    isCommandCombinationDepressed = true;
                                    break;
                                default:
                                    isCommandCombinationDepressed = false;
                                    break;
                            }
                        }
                    }
                    else {
                        isCommandCombinationDepressed = false;
                    }
                }

                if (isCommandCombinationDepressed == true) {
                    // send translated message via component dispatcher
                    MSG dispatchMsg = new MSG();
                    dispatchMsg.hwnd = msg.HWnd;
                    dispatchMsg.lParam = msg.LParam;
                    dispatchMsg.wParam = msg.WParam;
                    dispatchMsg.message = msg.Msg;
                    ComponentDispatcher.RaiseThreadMessage(ref dispatchMsg);
                    msg.Result = (IntPtr)1;
                    return true;
                }
            }
            return base.PreProcessMessage(ref msg);
        }
    }
}
