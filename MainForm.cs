using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectStarter
{
    public partial class MainForm : Form
    {
        // UI sizing constants
        private const int DefaultFormWidth = 640;
        private const int DefaultFormHeight = 640;
        private const int DefaultSplitterDistance = 520;
        private const int DefaultPanel1MinSize = 150;
        private const int DefaultPanel2MinSize = 150;
        private const int DefaultLeftPanelWidth = 480;
        private const int DefaultTitleHeight = 50;
        private const int DefaultButtonHeight = 60;
        private const int DefaultControlPadding = 10;
        private const int DefaultControlSpacing = 5;
        private const int DefaultTextBoxWidth = 330;
        private const int DefaultButtonWidth = 120;
        private const int DefaultSmallButtonWidth = 80;
        private const int DefaultButtonSpacing = 10;
        private const int DefaultLabelOffset = 120;

        private readonly List<string> templates = new List<string>
        {
            "React",
            "React + Vite",
            "Vite (Vanilla)",
            "Svelte",
            "SvelteKit",
            "Vue",
            "Next.js",
            "Three.js (using Vite)",
            "Express",
            "Angular",
            "Tailwind Standalone"
        };

        private readonly List<string> enhancements = new List<string>
        {
            "Tailwind CSS",
            "ESLint + Prettier",
            "Sass",
            "Playwright (E2E testing)",
            "Vitest (Unit testing)",
            "TypeScript"
        };

        private RichTextBox outputTextBox;
        private TextBox projectNameTextBox;
        private ListBox templateListBox;
        private CheckedListBox enhancementsListBox;
        private Label nodeStatusLabel;
        private Button createButton;
        private Button browseButton;
        private Label projectLocationLabel;
        private string outputDirectory;
        private string startCommand;
        private SplitContainer mainSplitContainer;

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
            CheckPrerequisitesAsync();
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.Name = "MainForm";
            this.Text = "Project Starter";
            this.Size = new Size(DefaultFormWidth, DefaultFormHeight);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 9F);
            this.AutoScroll = true;

            this.ResumeLayout(false);
        }

        private void InitializeUI()
        {
            // ==========================
            // 1) MAIN SPLIT CONTAINER
            // ==========================
            mainSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = DefaultSplitterDistance,
                Panel1MinSize = DefaultPanel1MinSize,
                Panel2MinSize = DefaultPanel2MinSize,
                FixedPanel = FixedPanel.None
            };
            this.Controls.Add(mainSplitContainer);



            // ==========================
            // 2) MAIN CONTENT HOLDER
            // ==========================
            TableLayoutPanel mainContentHolder = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            // First row auto-sizes to fit contentPanel; second row takes the remaining space.
            mainContentHolder.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainContentHolder.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainSplitContainer.Panel1.Controls.Add(mainContentHolder);

            // -------------------------------------------
            // 2a) CONTENT PANEL (Project Name and Location)
            // -------------------------------------------
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(DefaultControlPadding)
            };
            mainContentHolder.Controls.Add(contentPanel, 0, 0);

            // -- Project Location Section (use FlowLayoutPanel for horizontal layout)
            FlowLayoutPanel locationPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 5, 0, 5)
            };
            contentPanel.Controls.Add(locationPanel);

            Label locationLabel = new Label
            {
                Text = "Project Location:",
                AutoSize = true
            };
            locationPanel.Controls.Add(locationLabel);

            projectLocationLabel = new Label
            {
                Text = "",
                AutoSize = true,
                ForeColor = Color.Blue,
                Margin = new Padding(0, 5, 0, 5)
            };
            locationPanel.Controls.Add(projectLocationLabel);

            browseButton = new Button
            {
                Text = "Browse..."
            };
            browseButton.Click += BrowseButton_Click;
            locationPanel.Controls.Add(browseButton);

            // -- Project Name Section (also using FlowLayoutPanel)
            FlowLayoutPanel projectNamePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 5, 0, 5)
            };
            contentPanel.Controls.Add(projectNamePanel);

            Label projectNameLabel = new Label
            {
                Text = "Project Name:",
                AutoSize = true
            };
            projectNamePanel.Controls.Add(projectNameLabel);

            projectNameTextBox = new TextBox
            {
                Width = DefaultTextBoxWidth,
                Margin = new Padding(10, 0, 0, 0)
            };
            projectNamePanel.Controls.Add(projectNameTextBox);

            // -------------------------------------------
            // 2b) LIST HOLDER (Templates and Enhancements)
            // -------------------------------------------
            TableLayoutPanel listHolder = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            listHolder.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // Left column gets 50%
            listHolder.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // Right column gets 50%
            mainContentHolder.Controls.Add(listHolder, 0, 1);


            // Left Panel: Template List (fixed width)
            Panel leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                //Width = DefaultLeftPanelWidth,
                Padding = new Padding(DefaultControlPadding)
            };
            listHolder.Controls.Add(leftPanel, 0, 0);

            // Template selection section
            TableLayoutPanel templateTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            templateTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F)); // Fixed height for label
            templateTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Remaining space for list
            leftPanel.Controls.Add(templateTable);

            Label templateLabel = new Label
            {
                Text = "Select Template:",
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            templateTable.Controls.Add(templateLabel, 0, 0);

            templateListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                IntegralHeight = false,
                BorderStyle = BorderStyle.FixedSingle
            };
            templateTable.Controls.Add(templateListBox, 0, 1);

            foreach (string template in templates)
            {
                templateListBox.Items.Add(template);
            }
            templateListBox.SelectedIndex = 0;

            // Right Panel: Enhancements List (fills remaining space)
            Panel rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Padding = new Padding(DefaultControlPadding)
            };
            listHolder.Controls.Add(rightPanel, 1, 0);

            // Enhancements section
            TableLayoutPanel enhancementsTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            enhancementsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F)); // Fixed height for label
            enhancementsTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Remaining space for list
            rightPanel.Controls.Add(enhancementsTable);

            Label enhancementsLabel = new Label
            {
                Text = "Select Enhancements (Optional):",
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            enhancementsTable.Controls.Add(enhancementsLabel, 0, 0);

            enhancementsListBox = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                IntegralHeight = false,
                BorderStyle = BorderStyle.FixedSingle,
                CheckOnClick = true
            };
            enhancementsTable.Controls.Add(enhancementsListBox, 0, 1);

            foreach (string enhancement in enhancements)
            {
                enhancementsListBox.Items.Add(enhancement);
            }

            // ==========================
            // 3) OUTPUT & BOTTOM BUTTON PANEL
            // ==========================
            // Output TextBox in Panel2 of SplitContainer
            outputTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 9F)
            };
            mainSplitContainer.Panel2.Controls.Add(outputTextBox);

            // Button panel at bottom of Panel1
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = DefaultButtonHeight
            };
            mainSplitContainer.Panel1.Controls.Add(buttonPanel);

            createButton = new Button
            {
                Text = "Create Project",
                Size = new Size(DefaultButtonWidth, 30),
                Enabled = false,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            createButton.Location = new Point((buttonPanel.Width - createButton.Width) / 2, 10);
            createButton.Anchor = AnchorStyles.Top;
            createButton.Click += CreateButton_Click;
            buttonPanel.Controls.Add(createButton);

            // Set default output directory
            outputDirectory = "";
        }

        private async void CheckPrerequisitesAsync()
        {
            try
            {
                // Improved Node.js and npm detection - use which/where command first
                bool nodeInstalled = await IsCommandAvailableAsync("node");
                bool npmInstalled = await IsCommandAvailableAsync("npm");

                if (nodeInstalled && npmInstalled)
                {
                    // Only show success in the console output, not in the label
                    if (nodeStatusLabel != null)
                    {
                        nodeStatusLabel.Visible = false;
                    }
                    createButton.Enabled = true;

                    // Get versions for display
                    string nodeVersion = await GetCommandOutputAsync("node", "-v");
                    string npmVersion = await GetCommandOutputAsync("npm", "-v");

                    AppendOutput($"Node.js {nodeVersion.Trim()} detected", Color.Green);
                    AppendOutput($"npm {npmVersion.Trim()} detected", Color.Green);
                }
                else
                {
                    // Only show error message if prerequisites are missing
                    nodeStatusLabel.Text = "❌ Node.js and/or npm are not installed!";
                    nodeStatusLabel.ForeColor = Color.Red;
                    nodeStatusLabel.Visible = true;

                    AppendOutput("ERROR: Node.js and/or npm are not installed!", Color.Red);

                    DialogResult result = MessageBox.Show(
                        "Node.js and/or npm are not installed on your system or not in your PATH. Would you like to open the Node.js download page?",
                        "Prerequisites Missing",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "https://nodejs.org/en/download/",
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                nodeStatusLabel.Text = $"Error checking prerequisites: {ex.Message}";
                nodeStatusLabel.ForeColor = Color.Red;
                nodeStatusLabel.Visible = true;
                AppendOutput($"Error: {ex.Message}", Color.Red);
            }
        }

        private async Task<bool> IsCommandAvailableAsync(string command)
        {
            try
            {
                // First try "where" (Windows) or "which" (Unix) command to check if executable is in PATH
                string checkCommand = Environment.OSVersion.Platform == PlatformID.Win32NT ? "where" : "which";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = checkCommand,
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0)
                    {
                        return true;
                    }
                }

                // Fallback: try running the command directly with -v flag
                return await CheckCommandAsync(command, "-v");
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GetCommandOutputAsync(string command, string args)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    string output = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();
                    return output;
                }
            }
            catch
            {
                return "Unknown";
            }
        }

        private async Task<bool> CheckCommandAsync(string command, string args)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    await process.WaitForExitAsync();
                    return process.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            // Require project name before browsing
            string projectName = projectNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(projectName))
            {
                MessageBox.Show("Please enter a project name first.", "Project Name Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                projectNameTextBox.Focus();
                return;
            }

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select Project Location";
                folderDialog.UseDescriptionForTitle = true;

                // Set initial directory if not empty
                if (!string.IsNullOrWhiteSpace(outputDirectory))
                {
                    folderDialog.SelectedPath = outputDirectory;
                }

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    // Store the base directory
                    outputDirectory = folderDialog.SelectedPath;

                    // Show the full path including project name
                    string fullProjectPath = Path.Combine(outputDirectory, projectName);
                    projectLocationLabel.Text = ShortenPath(fullProjectPath);
                }
            }
        }

        private async void CreateButton_Click(object sender, EventArgs e)
        {
            string projectName = projectNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(projectName))
            {
                MessageBox.Show("Please enter a project name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                MessageBox.Show("Please select a project location.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Sanitize project name (replace spaces with hyphens and convert to lowercase)
            string sanitizedProjectName = projectName.Replace(" ", "-").ToLower();

            // Get selected template
            string selectedTemplate = templateListBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedTemplate))
            {
                MessageBox.Show("Please select a template.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get selected enhancements
            List<string> selectedEnhancements = new List<string>();
            for (int i = 0; i < enhancementsListBox.Items.Count; i++)
            {
                if (enhancementsListBox.GetItemChecked(i))
                {
                    selectedEnhancements.Add(enhancementsListBox.Items[i].ToString());
                }
            }

            // Show configuration summary and confirm
            string summary = $"Project Name: {sanitizedProjectName}\n" +
                $"Template: {selectedTemplate}\n" +
                $"Enhancements: {(selectedEnhancements.Count > 0 ? string.Join(", ", selectedEnhancements) : "None")}\n" +
                $"Location: {outputDirectory}\n\n" +
                "Do you want to proceed with this configuration?";

            DialogResult result = MessageBox.Show(
                summary,
                "Confirm Setup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return;
            }

            // Clear output and start setup
            ClearOutput();
            mainSplitContainer.SplitterDistance = 300; // Resize to show more of output panel
            AppendOutput("Starting project setup...\n", Color.White);

            try
            {
                // Set default start command
                startCommand = "npm run dev";

                // Execute commands based on the selected template
                await SetupProjectAsync(sanitizedProjectName, selectedTemplate, selectedEnhancements);

                // Add button to launch development server
                AddDevelopmentServerButton(sanitizedProjectName);

                AppendOutput("\n🚀 Project setup complete!", Color.Lime);
                AppendOutput("\nTo start development:", Color.White);
                AppendOutput($"  cd {sanitizedProjectName}", Color.LightGray);
                AppendOutput($"  {startCommand}", Color.LightGray);
                AppendOutput("\nHappy coding! 💻", Color.Cyan);
            }
            catch (Exception ex)
            {
                AppendOutput($"\nError setting up project: {ex.Message}", Color.Red);
            }
        }

        private void AddDevelopmentServerButton(string projectName)
        {
            // Create panel for buttons
            Panel buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom
            };

            Button startServerButton = new Button
            {
                Text = "Start Development Server",
                Size = new Size(180, 30),
                Location = new Point(DefaultControlPadding, DefaultControlPadding)
            };
            startServerButton.Click += async (s, e) => await StartDevelopmentServerAsync(projectName);

            Button openFolderButton = new Button
            {
                Text = "Open Project Folder",
                Size = new Size(150, 30),
                Location = new Point(200, DefaultControlPadding)
            };
            openFolderButton.Click += (s, e) => OpenProjectFolder(projectName);

            Button backButton = new Button
            {
                Text = "New Project",
                Size = new Size(100, 30),
                Location = new Point(360, DefaultControlPadding)
            };
            backButton.Click += (s, e) => {
                mainSplitContainer.SplitterDistance = DefaultSplitterDistance; // Reset split distance
                projectNameTextBox.Text = "";
                // Remove any existing button panels
                foreach (Control c in mainSplitContainer.Panel2.Controls)
                {
                    if (c is Panel panel && panel != buttonPanel)
                    {
                        mainSplitContainer.Panel2.Controls.Remove(panel);
                    }
                }
            };

            buttonPanel.Controls.Add(startServerButton);
            buttonPanel.Controls.Add(openFolderButton);
            buttonPanel.Controls.Add(backButton);

            // Remove any existing button panels
            foreach (Control c in mainSplitContainer.Panel2.Controls)
            {
                if (c is Panel panel && panel != buttonPanel)
                {
                    mainSplitContainer.Panel2.Controls.Remove(panel);
                }
            }

            mainSplitContainer.Panel2.Controls.Add(buttonPanel);
        }

        private void OpenProjectFolder(string projectName)
        {
            string projectPath = Path.Combine(outputDirectory, projectName);
            if (Directory.Exists(projectPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = projectPath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show($"Project folder not found: {projectPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task StartDevelopmentServerAsync(string projectName)
        {
            string projectPath = Path.Combine(outputDirectory, projectName);
            if (!Directory.Exists(projectPath))
            {
                MessageBox.Show($"Project folder not found: {projectPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppendOutput("\nStarting development server...", Color.Yellow);
            AppendOutput("You can stop the server by closing the command window.", Color.Yellow);

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/k cd /d \"{projectPath}\" && {startCommand}",
                    UseShellExecute = true,
                    WorkingDirectory = projectPath
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                AppendOutput($"\nError starting development server: {ex.Message}", Color.Red);
            }
        }

        private async Task SetupProjectAsync(string projectName, string template, List<string> enhancements)
        {
            // Create base command based on template
            string baseCommand = "";
            List<string> postInstallCommands = new List<string>();

            AppendOutput($"Setting up {template} project: {projectName}\n", Color.White);

            switch (template)
            {
                case "React":
                    baseCommand = $"npx --yes create-react-app {projectName}";
                    startCommand = "npm start";
                    break;
                case "React + Vite":
                    baseCommand = $"npm create vite@latest {projectName} -y -- --template react";
                    postInstallCommands.Add($"cd {projectName} && npm install");
                    break;
                case "Vite (Vanilla)":
                    baseCommand = $"npm create vite@latest {projectName} -y -- --template vanilla";
                    postInstallCommands.Add($"cd {projectName} && npm install");
                    break;
                case "Svelte":
                    baseCommand = $"npx --yes degit sveltejs/template {projectName}";
                    postInstallCommands.Add($"cd {projectName} && npm install");
                    break;
                case "SvelteKit":
                    baseCommand = $"npm create svelte@latest {projectName} -y";
                    postInstallCommands.Add($"cd {projectName} && npm install");
                    break;
                case "Vue":
                    baseCommand = $"npm create vue@latest {projectName} -y";
                    postInstallCommands.Add($"cd {projectName} && npm install");
                    break;
                case "Next.js":
                    baseCommand = $"npx --yes create-next-app@latest {projectName}";
                    break;
                case "Three.js (using Vite)":
                    baseCommand = $"npm create vite@latest {projectName} -y -- --template vanilla";
                    postInstallCommands.Add($"cd {projectName} && npm install");
                    postInstallCommands.Add($"cd {projectName} && npm install three");
                    break;
                case "Express":
                    baseCommand = $"npx --yes express-generator {projectName}";
                    postInstallCommands.Add($"cd {projectName} && npm install");
                    startCommand = "npm start";
                    break;
                case "Angular":
                    baseCommand = $"npx --yes @angular/cli new {projectName} --defaults";
                    startCommand = "ng serve";
                    break;
                case "Tailwind Standalone":
                    // Create basic HTML project with Tailwind CSS
                    CreateTailwindStandaloneProject(projectName);
                    startCommand = "start \"\" http://localhost:8000 && python -m http.server";
                    return;
            }

            // Execute base command
            if (!string.IsNullOrEmpty(baseCommand))
            {
                await ExecuteCommandAsync(baseCommand, outputDirectory);
            }

            // Execute post-install commands
            foreach (string command in postInstallCommands)
            {
                await ExecuteCommandAsync(command, outputDirectory);
            }

            // Apply enhancements
            if (enhancements.Count > 0)
            {
                await ApplyEnhancementsAsync(projectName, enhancements);
            }
        }

        private void CreateTailwindStandaloneProject(string projectName)
        {
            string projectPath = Path.Combine(outputDirectory, projectName);

            // Create project directory
            AppendOutput($"Creating directory: {projectPath}", Color.White);
            Directory.CreateDirectory(projectPath);
            Directory.CreateDirectory(Path.Combine(projectPath, "js"));

            // Create index.html with Tailwind CDN
            string htmlContent = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{projectName}</title>
    <script src=""https://cdn.tailwindcss.com""></script>
    <script src=""js/main.js"" defer></script>
</head>
<body class=""bg-gray-100 min-h-screen"">
    <div class=""container mx-auto px-4 py-8"">
        <h1 class=""text-3xl font-bold text-center mb-8"">Welcome to {projectName}</h1>
        
        <div class=""bg-white p-6 rounded-lg shadow-md"">
            <p class=""text-gray-700"">This is a starter template with Tailwind CSS.</p>
        </div>
    </div>
</body>
</html>";

            File.WriteAllText(Path.Combine(projectPath, "index.html"), htmlContent);
            AppendOutput("Created index.html with Tailwind CSS", Color.Green);

            // Create a simple JavaScript file
            string jsContent = $@"// Your JavaScript code here
console.log('{projectName} is ready!');";

            File.WriteAllText(Path.Combine(projectPath, "js", "main.js"), jsContent);
            AppendOutput("Created main.js", Color.Green);

            // Success message
            AppendOutput("\nTailwind Standalone project created successfully!", Color.Green);
        }

        private async Task ApplyEnhancementsAsync(string projectName, List<string> enhancements)
        {
            string projectPath = Path.Combine(outputDirectory, projectName);

            AppendOutput("\nApplying enhancements...", Color.Yellow);

            foreach (string enhancement in enhancements)
            {
                AppendOutput($"\nAdding {enhancement}...", Color.Cyan);

                switch (enhancement)
                {
                    case "Tailwind CSS":
                        await ExecuteCommandAsync($"cd {projectName} && npm install -D tailwindcss postcss autoprefixer --yes", outputDirectory);
                        await ExecuteCommandAsync($"cd {projectName} && npx --yes tailwindcss init -p", outputDirectory);

                        // Create tailwind.config.js
                        string tailwindConfig = @"/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    ""./src/**/*.{js,jsx,ts,tsx,svelte,vue,html}"",
    ""./index.html"",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}";
                        File.WriteAllText(Path.Combine(projectPath, "tailwind.config.js"), tailwindConfig);

                        // Add Tailwind directives to CSS files
                        string tailwindDirectives = @"@tailwind base;
@tailwind components;
@tailwind utilities;";

                        if (Directory.Exists(Path.Combine(projectPath, "src")))
                        {
                            string cssPath = "";
                            if (File.Exists(Path.Combine(projectPath, "src", "index.css")))
                            {
                                cssPath = Path.Combine(projectPath, "src", "index.css");
                            }
                            else if (File.Exists(Path.Combine(projectPath, "src", "style.css")))
                            {
                                cssPath = Path.Combine(projectPath, "src", "style.css");
                            }
                            else
                            {
                                cssPath = Path.Combine(projectPath, "src", "index.css");
                            }

                            File.WriteAllText(cssPath, tailwindDirectives);
                            AppendOutput("Added Tailwind CSS directives to styles", Color.Green);
                        }
                        break;

                    case "ESLint + Prettier":
                        await ExecuteCommandAsync($"cd {projectName} && npm install -D eslint prettier eslint-config-prettier eslint-plugin-prettier --yes", outputDirectory);

                        // Create ESLint config
                        string eslintConfig = @"module.exports = {
  ""env"": {
    ""browser"": true,
    ""es2021"": true,
    ""node"": true
  },
  ""extends"": [
    ""eslint:recommended"",
    ""plugin:prettier/recommended""
  ],
  ""parserOptions"": {
    ""ecmaVersion"": ""latest"",
    ""sourceType"": ""module""
  },
  ""rules"": {
    ""prettier/prettier"": ""error""
  }
}";
                        File.WriteAllText(Path.Combine(projectPath, ".eslintrc.js"), eslintConfig);

                        // Create Prettier config
                        string prettierConfig = @"{
  ""semi"": true,
  ""singleQuote"": true,
  ""tabWidth"": 2,
  ""trailingComma"": ""es5""
}";
                        File.WriteAllText(Path.Combine(projectPath, ".prettierrc"), prettierConfig);
                        AppendOutput("Added ESLint and Prettier configuration", Color.Green);
                        break;

                    case "Sass":
                        await ExecuteCommandAsync($"cd {projectName} && npm install -D sass --yes", outputDirectory);
                        AppendOutput("Added Sass support", Color.Green);
                        break;

                    case "Playwright (E2E testing)":
                        await ExecuteCommandAsync($"cd {projectName} && npm init playwright@latest -y", outputDirectory);
                        AppendOutput("Added Playwright for E2E testing", Color.Green);
                        break;

                    case "Vitest (Unit testing)":
                        await ExecuteCommandAsync($"cd {projectName} && npm install -D vitest --yes", outputDirectory);

                        // Create Vitest config
                        string vitestConfig = @"import { defineConfig } from 'vitest/config'

export default defineConfig({
  test: {
    globals: true,
    environment: 'jsdom',
  },
})";
                        File.WriteAllText(Path.Combine(projectPath, "vitest.config.js"), vitestConfig);
                        AppendOutput("Added Vitest for unit testing", Color.Green);
                        break;

                    case "TypeScript":
                        await ExecuteCommandAsync($"cd {projectName} && npm install -D typescript @types/node --yes", outputDirectory);

                        // Create tsconfig.json
                        string tsConfig = @"{
  ""compilerOptions"": {
    ""target"": ""es2022"",
    ""module"": ""esnext"",
    ""moduleResolution"": ""node"",
    ""esModuleInterop"": true,
    ""forceConsistentCasingInFileNames"": true,
    ""strict"": true,
    ""skipLibCheck"": true
  }
}";
                        File.WriteAllText(Path.Combine(projectPath, "tsconfig.json"), tsConfig);
                        AppendOutput("Added TypeScript support", Color.Green);
                        break;
                }
            }
        }

        private async Task ExecuteCommandAsync(string command, string workingDirectory)
        {
            AppendOutput($"\nExecuting: {command}", Color.Yellow);

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    StringBuilder outputBuilder = new StringBuilder();
                    StringBuilder errorBuilder = new StringBuilder();

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            AppendOutputThreadSafe(e.Data, Color.White);
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            AppendOutputThreadSafe(e.Data, Color.Red);
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    var timeoutTask = Task.Delay(TimeSpan.FromMinutes(5)); // 5-minute timeout
                    var processTask = process.WaitForExitAsync();
                    if (await Task.WhenAny(processTask, timeoutTask) == timeoutTask)
                    {
                        // Process timed out
                        try
                        {
                            process.Kill(true); // Kill the process and its children
                        }
                        catch { /* ignore errors on process kill */ }
                        AppendOutput("Process timed out after 5 minutes. Operation aborted.", Color.Red);
                        throw new TimeoutException("Command execution timed out.");
                    }

                    if (process.ExitCode != 0)
                    {
                        AppendOutput($"Command completed with exit code: {process.ExitCode}", Color.Yellow);
                    }
                    else
                    {
                        AppendOutput("Command completed successfully", Color.Green);
                    }
                }
                   
                // If I want to launch a separate terminal that the user can type into...
                //ProcessStartInfo startInfo = new ProcessStartInfo
                //{
                //    FileName = "cmd.exe",
                //    Arguments = $"/k cd /d \"{workingDirectory}\" && {command}",
                //    UseShellExecute = true, // This allows the window to be visible
                //    CreateNoWindow = false, // We want to see the window
                //    WorkingDirectory = workingDirectory
                //};

                //// Launch process and let user interact with it directly
                //Process process = Process.Start(startInfo);
                //await process.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                AppendOutput($"Error executing command: {ex.Message}", Color.Red);
                throw;
            }
        }

        private void AppendOutput(string text, Color color)
        {
            if (outputTextBox.InvokeRequired)
            {
                outputTextBox.Invoke(new Action(() => AppendOutputThreadSafe(text, color)));
            }
            else
            {
                AppendOutputThreadSafe(text, color);
            }
        }

        private void AppendOutputThreadSafe(string text, Color color)
        {
            // Check if invoke is needed (we're on a different thread)
            if (outputTextBox.InvokeRequired)
            {
                outputTextBox.Invoke(new Action(() => {
                    // Now we're on the UI thread
                    outputTextBox.SelectionStart = outputTextBox.TextLength;
                    outputTextBox.SelectionLength = 0;
                    outputTextBox.SelectionColor = color;
                    outputTextBox.AppendText(text + Environment.NewLine);
                    outputTextBox.SelectionStart = outputTextBox.TextLength;
                    outputTextBox.ScrollToCaret();
                }));
            }
            else
            {
                // Already on the UI thread
                outputTextBox.SelectionStart = outputTextBox.TextLength;
                outputTextBox.SelectionLength = 0;
                outputTextBox.SelectionColor = color;
                outputTextBox.AppendText(text + Environment.NewLine);
                outputTextBox.SelectionStart = outputTextBox.TextLength;
                outputTextBox.ScrollToCaret();
            }
        }

        private string ShortenPath(string path, int maxFolders = 4)
        {
            string[] folders = path.Split(Path.DirectorySeparatorChar);
            if (folders.Length <= maxFolders)
                return path;

            return "..." + Path.DirectorySeparatorChar +
                   string.Join(Path.DirectorySeparatorChar.ToString(),
                   folders.Skip(folders.Length - maxFolders));
        }

        private void ClearOutput()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => ClearOutput()));
                return;
            }
            outputTextBox.Clear();
        }

    }
}