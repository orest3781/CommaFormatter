using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CommaFormatter
    {
    /// <summary>
    /// A WinForms form that:
    /// 1. Formats multiline text into comma-separated text,
    /// 2. Has a Clear button,
    /// 3. Minimizes to system tray,
    /// 4. Uses a modern color palette.
    /// 
    /// Also shows a warning if the text box is empty before formatting.
    /// </summary>
    public partial class CommaFormatterForm : Form
        {
        // Example corporate color palette (tweak to your liking)
        private static readonly Color BrandPrimary = Color.FromArgb(0, 150, 136);   // Dark blue
        private static readonly Color BrandSecondary = Color.FromArgb(0, 170, 155); // Medium teal
        private static readonly Color BrandAccent = Color.FromArgb(0, 120, 110); // Gold
        private static readonly Color BrandLight = Color.FromArgb(245, 245, 245); // Light grayish-blue background

        private TextBox textBox;
        private Button formatAndCopyButton;
        private Button clearButton;

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommaFormatterForm"/> class.
        /// </summary>
        public CommaFormatterForm()
            {
            InitializeForm();
            InitializeTrayIcon();
            }

        /// <summary>
        /// Sets up the form's UI controls and applies the color palette.
        /// </summary>
        private void InitializeForm()
            {
            // Basic form styling
            this.Text = "Comma Formatter";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(420, 320);

            // Prevent resizing, modern flat border
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Light background and modern font
            this.BackColor = BrandLight;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // (Optional) If you have an icon in Project Properties => Application => Icon
            // That icon is already embedded into the .exe.
            // If you want the form to explicitly load the same icon, you can do:
            // this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // Create a multiline TextBox
            textBox = new TextBox
                {
                Multiline = true,
                Size = new Size(350, 150),
                Location = new Point(30, 30),
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle
                };
            this.Controls.Add(textBox);

            // "Format & Copy" button
            formatAndCopyButton = new Button
                {
                Text = "Format && Copy",  // Double '&' displays single '&'
                Size = new Size(100, 35),
                Location = new Point(80, 200),
                FlatStyle = FlatStyle.Flat,
                BackColor = BrandPrimary,
                ForeColor = Color.White
                };
            formatAndCopyButton.FlatAppearance.BorderSize = 0;
            formatAndCopyButton.FlatAppearance.MouseOverBackColor = BrandSecondary;
            formatAndCopyButton.FlatAppearance.MouseDownBackColor = BrandAccent;
            formatAndCopyButton.Click += FormatAndCopyButton_Click;
            this.Controls.Add(formatAndCopyButton);

            // "Clear" button
            clearButton = new Button
                {
                Text = "Clear",
                Size = new Size(100, 35),
                Location = new Point(220, 200),
                FlatStyle = FlatStyle.Flat,
                BackColor = BrandSecondary,
                ForeColor = Color.White
                };
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.FlatAppearance.MouseOverBackColor = BrandPrimary;
            clearButton.FlatAppearance.MouseDownBackColor = BrandAccent;
            clearButton.Click += ClearButton_Click;
            this.Controls.Add(clearButton);
            }

        /// <summary>
        /// Initializes the system tray icon and context menu for minimizing to tray.
        /// </summary>
        private void InitializeTrayIcon()
            {
            // Create a context menu for the tray icon
            trayMenu = new ContextMenuStrip();

            // "Restore" menu item
            var restoreItem = new ToolStripMenuItem("Restore");
            restoreItem.Click += (s, e) => RestoreFromTray();
            trayMenu.Items.Add(restoreItem);

            // "Exit" menu item
            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => ExitApplication();
            trayMenu.Items.Add(exitItem);

            // Create the tray icon
            trayIcon = new NotifyIcon
                {
                Text = "Comma Formatter (Running)",
                ContextMenuStrip = trayMenu,
                Visible = false // Hidden until we minimize the form
                };

            // If you haven't set an icon in Project Properties => Application,
            // trayIcon.Icon = SystemIcons.Application;
            // Otherwise, you can reuse the Form's icon, if set:
            // trayIcon.Icon = this.Icon ?? SystemIcons.Application;
            }

        /// <summary>
        /// Intercept form closing to minimize to tray instead of truly closing.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
            {
            // If user tries to close the form (X button), hide it to tray
            if (e.CloseReason == CloseReason.UserClosing)
                {
                e.Cancel = true;
                HideToTray();
                }
            else
                {
                base.OnFormClosing(e);
                }
            }

        /// <summary>
        /// Hide the form and show the system tray icon.
        /// </summary>
        private void HideToTray()
            {
            this.Hide();
            trayIcon.Visible = true;
            }

        /// <summary>
        /// Restore the form from the system tray.
        /// </summary>
        private void RestoreFromTray()
            {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            trayIcon.Visible = false;
            }

        /// <summary>
        /// Exit the application fully (via tray "Exit" menu).
        /// </summary>
        private void ExitApplication()
            {
            trayIcon.Visible = false;
            trayIcon.Dispose();
            Application.Exit();
            }

        /// <summary>
        /// Format and copy the textbox contents to clipboard.
        /// </summary>
        private void FormatAndCopyButton_Click(object sender, EventArgs e)
            {
            // Early check: if textbox is empty or whitespace, show a warning
            if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                MessageBox.Show(
                    "Please enter some text before formatting.",
                    "No Input",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
                }

            // 1. Get raw text
            string inputText = textBox.Text;

            // 2. Split, trim, remove empty lines, join with commas
            var lines = inputText
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l));

            string formattedText = string.Join(",", lines);

            // 3. Copy to clipboard
            Clipboard.SetText(formattedText);

            // 4. Show a confirmation
            MessageBox.Show(
                "Formatted text copied to clipboard!",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            }

        /// <summary>
        /// Clear the textbox contents.
        /// </summary>
        private void ClearButton_Click(object sender, EventArgs e)
            {
            textBox.Clear();
            }

        private void InitializeComponent()
            {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommaFormatterForm));
            SuspendLayout();
            // 
            // CommaFormatterForm
            // 
            ClientSize = new Size(284, 261);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "CommaFormatterForm";
            ResumeLayout(false);
            }
        }
    }
