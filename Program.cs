using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

// DOB NOW Trident Host — a free standalone IE-engine browser.
// Hosts the Windows MSHTML/Trident control (the same engine IE Tab uses),
// forces IE11 document mode, and loads the DOB NOW page so the Silverlight
// objection-attachment upload works. No annual fee.

namespace DobTrident
{
    static class Program
    {
        public const string HOME = "https://a810-lmpaca.nyc.gov/citizenaccessbuildings/welcome.aspx";

        [STAThread]
        static void Main(string[] args)
        {
            SelfRegisterIE11();
            EnableLoginPersistence();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string start = (args != null && args.Length > 0) ? args[0] : HOME;
            Application.Run(new MainForm(start));
        }

        // Tell the embedded WebBrowser control to render as IE11 (edge mode),
        // keyed to THIS exe's file name. HKCU, so no admin needed.
        static void SelfRegisterIE11()
        {
            try
            {
                string exe = Path.GetFileName(Application.ExecutablePath);
                Set(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", exe, 11001);
                // Don't force GPU rendering — legacy Silverlight renders more reliably
                // with the default path inside the Trident control.
                Set(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_NINPUT_LEGACYMODE", exe, 0);
            }
            catch { /* non-fatal: app still runs, just maybe in older doc mode */ }
        }

        static void Set(string subkey, string name, int val)
        {
            using (RegistryKey k = Registry.CurrentUser.CreateSubKey(subkey))
            {
                if (k != null) k.SetValue(name, val, RegistryValueKind.DWord);
            }
        }

        static void SetStr(string subkey, string name, string val)
        {
            using (RegistryKey k = Registry.CurrentUser.CreateSubKey(subkey))
            {
                if (k != null) k.SetValue(name, val, RegistryValueKind.String);
            }
        }

        // Turn on IE form + password AutoComplete so the Trident control offers to
        // save your DOB login and fills it back in. (Persistent cookies already
        // survive between launches via the shared Windows/WinINET cookie store, so
        // checking the site's "Remember me" box keeps you signed in.)
        static void EnableLoginPersistence()
        {
            try
            {
                const string main = @"Software\Microsoft\Internet Explorer\Main";
                SetStr(main, "Use FormSuggest", "yes");
                SetStr(main, "FormSuggest Passwords", "yes");
                SetStr(main, "FormSuggest PW Ask", "no");
            }
            catch { }
        }
    }

    public class MainForm : Form
    {
        readonly WebBrowser web = new WebBrowser();
        readonly TextBox addr = new TextBox();

        public MainForm(string startUrl)
        {
            Text = "DOB NOW — Trident/Silverlight Host (free IE Tab replacement)";
            Width = 1280;
            Height = 900;
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            try { Icon = SystemIcons.Application; } catch { }

            // ---- top toolbar ----
            ToolStrip bar = new ToolStrip();
            bar.GripStyle = ToolStripGripStyle.Hidden;
            bar.Items.Add(MakeButton("◀ Back", (s, e) => { if (web.CanGoBack) web.GoBack(); }));
            bar.Items.Add(MakeButton("Forward ▶", (s, e) => { if (web.CanGoForward) web.GoForward(); }));
            bar.Items.Add(MakeButton("⟳ Reload", (s, e) => web.Refresh()));
            bar.Items.Add(MakeButton("⌂ DOB Home", (s, e) => web.Navigate(Program.HOME)));
            bar.Items.Add(new ToolStripSeparator());

            ToolStripControlHost addrHost = new ToolStripControlHost(addr);
            addrHost.AutoSize = false;
            addrHost.Width = 600;
            addr.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    Go(addr.Text);
                }
            };
            bar.Items.Add(addrHost);
            bar.Items.Add(MakeButton("Go", (s, e) => Go(addr.Text)));
            bar.Items.Add(new ToolStripSeparator());

            // Silverlight readiness indicator — checks the 32-bit ActiveX is present.
            ToolStripLabel sl = new ToolStripLabel(SilverlightStatus());
            bar.Items.Add(sl);

            // ---- browser ----
            web.Dock = DockStyle.Fill;
            web.ScriptErrorsSuppressed = true;           // hide the IE script-error popups
            web.IsWebBrowserContextMenuEnabled = true;
            web.AllowWebBrowserDrop = false;
            web.Navigated += (s, e) => { addr.Text = e.Url != null ? e.Url.ToString() : ""; };

            Controls.Add(web);
            Controls.Add(bar);

            Load += (s, e) => Go(startUrl);
        }

        void Go(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;
            url = url.Trim();
            if (!url.Contains("://")) url = "https://" + url;
            try { web.Navigate(url); } catch { }
        }

        // Reads the 32-bit Silverlight ActiveX registration (this is an x86 process,
        // so the registry view already points at WOW6432Node) and confirms the dll exists.
        static string SilverlightStatus()
        {
            try
            {
                using (RegistryKey k = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Classes\CLSID\{DFEAF541-F3E1-4C24-ACAC-99C30715084A}\InprocServer32"))
                {
                    if (k != null)
                    {
                        string dll = k.GetValue(null) as string;
                        if (!string.IsNullOrEmpty(dll) && File.Exists(dll))
                        {
                            string ver = "";
                            using (RegistryKey s = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Silverlight"))
                                if (s != null) ver = " " + (s.GetValue("Version") as string);
                            return "Silverlight" + ver + " ready ✓";
                        }
                    }
                }
            }
            catch { }
            return "Silverlight NOT found ✗  (upload will fail)";
        }

        static ToolStripButton MakeButton(string text, EventHandler onClick)
        {
            ToolStripButton b = new ToolStripButton(text);
            b.DisplayStyle = ToolStripItemDisplayStyle.Text;
            b.Click += onClick;
            return b;
        }
    }
}
