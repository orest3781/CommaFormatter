using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CommaFormatter
    {
    /// <summary>
    /// A WinForms form that formats multiline text, copies it to the clipboard,
    /// includes a Clear button, and minimizes to the system tray.
    /// This version uses a corporate color palette for a more cohesive style.
    /// </summary>
    public partial class CommaFormatterForm : Form
        {
        // Example "corporate" color palette
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
        /// Sets up the form's UI controls and applies a corporate color palette.
        /// </summary>
        private void InitializeForm()
            {
            // Form styling
            this.Text = "Comma Formatter";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(420, 320);

            // A modern, fixed, non-resizable form
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Corporate background color and font
            this.BackColor = BrandLight;
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // (Optional) Set a custom icon from the same folder
            try
                {
                string iconPath = Path.Combine(AppContext.BaseDirectory, "favicon.ico");
                if (File.Exists(iconPath))
                    {
                    this.Icon = new Icon(iconPath);
                    }
                }
            catch
                {
                // Ignore if icon loading fails
                }

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

            // "Format & Copy" button (use double ampersand to show the '&')
            formatAndCopyButton = new Button
                {
                Text = "Format && Copy",
                Size = new Size(100, 35),
                Location = new Point(80, 200),
                FlatStyle = FlatStyle.Flat,
                BackColor = BrandPrimary,
                ForeColor = Color.White
                };
            // Remove the default border, adjust hover/pressed colors
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
                BackColor = BrandPrimary,
                ForeColor = Color.White
                };
            clearButton.FlatAppearance.BorderSize = 0;
            clearButton.FlatAppearance.MouseOverBackColor = BrandSecondary;
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
                Visible = false // Hidden until we minimize
                };

            // Use the same icon as the form, or default if none
            trayIcon.Icon = this.Icon ?? SystemIcons.Application;

            // Double-clicking the tray icon restores the window
            trayIcon.DoubleClick += (s, e) => RestoreFromTray();
            }

        /// <summary>
        /// Intercept form closing to minimize to tray instead of truly closing.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
            {
            // If the user hits 'X', hide instead of close
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
        /// Exit the application fully (used by tray "Exit" menu).
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

            // 4. Show confirmation
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
        }
    }
