# DOB NOW Trident Host

NYC's DOB NOW portal still needs Silverlight to upload objection attachments, and no modern browser runs Silverlight anymore. The usual answer is IE Tab, which costs about $20 a year. This is a small WinForms app that hosts the same Windows MSHTML/Trident control IE Tab uses, so the upload works again. Free, one file, no admin needed.

## What it does

- Hosts the built-in `WebBrowser` (Trident) control and points it at the DOB NOW citizen access page.
- Registers itself under `FEATURE_BROWSER_EMULATION` in HKCU so the control renders as IE11 instead of the default IE7 mode. HKCU means no admin rights.
- Checks whether the 32-bit Silverlight ActiveX is actually registered on the machine and shows "Silverlight ready ✓" or "Silverlight NOT found ✗" in the toolbar, so you find out before you waste time on a failed upload.
- Turns on IE form/password autocomplete so it offers to save your DOB login. Cookies persist through the normal WinINET store, so the site's "Remember me" box works.
- Toolbar: back, forward, reload, DOB Home, and an address bar if you need to go somewhere else.

## Requirements

- Windows with Silverlight 5 installed (32-bit). Without it the app still runs, but the upload control won't render.
- Must be built as **x86**. The Silverlight ActiveX is 32-bit only.

## Build

```
csc /target:winexe /platform:x86 /out:DOBNowTrident.exe Program.cs
```

`installer.iss` is the Inno Setup script that wraps the built exe into a per-user installer (no UAC prompt).

## Note

You can pass a URL as the first argument to open something other than the DOB home page:

```
DOBNowTrident.exe https://example.com
```

Script errors are suppressed, because the DOB pages throw plenty of them and the popups get old fast.
