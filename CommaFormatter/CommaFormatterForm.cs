using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CommaFormatter
    {
    /// <summary>
    /// A simple WinForms form to convert multiline text into a comma-separated string.
    /// </summary>
    public partial class CommaFormatterForm : Form
        {
        private TextBox textBox;
        private Button formatAndCopyButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommaFormatterForm"/> class.
        /// </summary>
        public CommaFormatterForm()
            {
            // Initialize form and controls
            InitializeForm();
            }

        /// <summary>
        /// Sets up this form's properties and child controls.
        /// </summary>
        private void InitializeForm()
            {
            this.Text = "Comma Formatter";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 300);

            // ...

            try
                {
                // Build the path to favicon.ico from the current directory
                string iconFileName = "favicon.ico";
                string iconPath = Path.Combine(AppContext.BaseDirectory, iconFileName);
                // Alternative: string iconPath = Path.Combine(Application.StartupPath, iconFileName);

                if (File.Exists(iconPath))
                    {
                    this.Icon = new Icon(iconPath);
                    }
                else
                    {
                    // Handle the case where the icon file is missing
                    // e.g. log or silently ignore
                    }
                }
            catch
                {
                // If the icon fails to load, handle or ignore the exception
                }

            // Create a multiline TextBox
            textBox = new TextBox
                {
                Multiline = true,
                Size = new Size(350, 150),
                Location = new Point(20, 20)
                };
            this.Controls.Add(textBox);

            // Create a "Format & Copy" Button
            formatAndCopyButton = new Button
                {
                Text = "Format & Copy",
                Size = new Size(100, 30),
                Location = new Point(140, 190)
                };
            formatAndCopyButton.Click += FormatAndCopyButton_Click;
            this.Controls.Add(formatAndCopyButton);
            }

        /// <summary>
        /// Button click event that formats text and copies it to the clipboard.
        /// </summary>
        private void FormatAndCopyButton_Click(object sender, EventArgs e)
            {
            // 1. Get the raw text
            string inputText = textBox.Text;

            // 2. Split by newlines, trim lines, remove empty, then join with commas
            var lines = inputText
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l));

            string formattedText = string.Join(",", lines);

            // 3. Copy to clipboard
            Clipboard.SetText(formattedText);

            // 4. Show a confirmation dialog
            MessageBox.Show(
                "Formatted text copied to clipboard!",
                "Success",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            }
        }
    }
