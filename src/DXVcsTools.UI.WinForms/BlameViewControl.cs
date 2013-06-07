using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using DXVcsTools.Data;

namespace DXVcsTools.UI.WinForms
{
    public partial class BlameViewControl : UserControl
    {
        Font font = new Font("Courier New", 10);
        IList<IBlameLine> lines = new List<IBlameLine>();

        public ContextMenuStrip InfoContextMenuStrip
        {
            get { return infoBox.ContextMenuStrip; }
            set { infoBox.ContextMenuStrip = value; }
        }
        public int CurrentLineIndex
        {
            get
            {
                return infoBox.GetCurrentLineIndex();
            }
        }
        public IList<IBlameLine> Lines
        {
            get { return lines; }
        }
        string[] SourceText
        {
            get
            {
                List<string> text = new List<string>();
                foreach (IBlameLine line in lines)
                    text.Add(line.SourceLine);
                return text.ToArray();
            }
        }

        public BlameViewControl()
        {
            InitializeComponent();
            infoBox.Font = font;
            sourceBox.Font = font;
            sourceBox.ExtendBackColor = true;
        }

        public void Fill(IList<IBlameLine> lines)
        {
            this.lines = lines;
            RenderText();
        }

        void RenderText()
        {
            int lineNumber;

            string rtf = @"{\rtf1";
            rtf += @"{\colortbl;\red240\green255\blue240;\red240\green240\blue255;}";
            rtf += @"\pard\cf0\ql ";

            lineNumber = 1;
            foreach (string line in SourceText)
            {
                rtf += "\\highlight" + (1 + lineNumber % 2).ToString() + " ";
                rtf += line + "\\par ";
                lineNumber++;
            }
            rtf += "}";
            this.sourceBox.Rtf = rtf;

            rtf = @"{\rtf1\pard\tx900\tx2300\ql ";
            lineNumber = 1;
            foreach (IBlameLine line in lines)
            {
                rtf += string.Format("{0,6}\\tab {1}\\tab {2,6}\\par ", line.Revision, line.User.Substring(0, Math.Min(11, line.User.Length)), lineNumber);
                lineNumber++;
            }
            rtf += "}";
            this.infoBox.Rtf = rtf;
        }

        void sourceBox_SizeChanged(object sender, EventArgs e)
        {
            sourceBox.Invalidate();
        }
        void sourceBox_VScroll(object sender, EventArgs e)
        {
            infoBox.ScrollPos = new Point(0, sourceBox.ScrollPos.Y);
        }
        private void infoBox_Enter(object sender, EventArgs e)
        {
            sourceBox.Focus();
        }

        private void infoBox_MouseMove(object sender, MouseEventArgs e)
        {
            int lineIndex = infoBox.GetLineAtPoint(e.Location);
            if (lineIndex >= 0)
            {
                string text = Lines[lineIndex].CommitDate.ToString("") + Environment.NewLine + Lines[lineIndex].Comment;
                toolTip1.SetToolTip(infoBox, text);
            }
            else 
            {
                toolTip1.Hide(infoBox);
            }
        }
    }
    class ExRichTextBox : RichTextBox
    {
        const int WM_USER = 1024;
        const int EM_GETSCROLLPOS = WM_USER + 221;
        const int EM_SETSCROLLPOS = WM_USER + 222;
        const int EM_GETEDITSTYLE = WM_USER + 203;
        const int EM_SETEDITSTYLE = WM_USER + 204;
        const int EM_GETLINECOUNT = 0x00BA;

        const int SES_EXTENDBACKCOLOR = 4;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, ref Point lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        bool extendBackColor;
        Point lastRightClicked;

        public bool ExtendBackColor
        {
            get
            {

                return IsHandleCreated ?
                    (SendMessage(this.Handle, EM_GETEDITSTYLE, 0, 0) & SES_EXTENDBACKCOLOR) != 0 :
                    extendBackColor;
            }
            set
            {
                if (IsHandleCreated)
                    SendMessage(this.Handle, EM_SETEDITSTYLE, value ? SES_EXTENDBACKCOLOR : 0, SES_EXTENDBACKCOLOR);
                else
                    extendBackColor = value;
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SendMessage(this.Handle, EM_SETEDITSTYLE, extendBackColor ? SES_EXTENDBACKCOLOR : 0, SES_EXTENDBACKCOLOR);
        }

        public Point ScrollPos
        {
            get
            {
                Point scrollPoint = Point.Empty;
                SendMessage(this.Handle, EM_GETSCROLLPOS, 0, ref scrollPoint);
                int height = GetHeight();
                if (height > 65535)
                    scrollPoint.Y = (int)((double)scrollPoint.Y * (height / 65535.0));
                return scrollPoint;
            }
            set
            {
                Point original = value;
                SendMessage(this.Handle, EM_SETSCROLLPOS, 0, ref original);
            }
        }

        int GetHeight()
        {
            int lines = SendMessage(Handle, EM_GETLINECOUNT, 0, IntPtr.Zero).ToInt32();
            return Font.Height * lines;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                lastRightClicked = e.Location;
                //MessageBox.Show(string.Format("{0} {1}", lastRightClicked.X, lastRightClicked.Y));
            }
            base.OnMouseDown(e);
        }

        public int GetCurrentLineIndex()
        {
            return GetLineAtPoint(lastRightClicked);
        }

        public int GetLineAtPoint(Point point) 
        {
            return GetHeight() >= point.Y ? GetLineFromCharIndex(GetCharIndexFromPosition(point)) : -1;
        }

    }
}
