using Anotar.Serilog;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Interop.Plugins;
using SuperMemoAssistant.Interop.SuperMemo.Content.Contents;
using SuperMemoAssistant.Interop.SuperMemo.Content.Models;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
using SuperMemoAssistant.Interop.SuperMemo.Elements.Models;
using SuperMemoAssistant.Plugins.QuickQA.UI;
using SuperMemoAssistant.Services;
using SuperMemoAssistant.Services.IO.HotKeys;
using SuperMemoAssistant.Services.IO.Keyboard;
using SuperMemoAssistant.Services.UI.Configuration;
using SuperMemoAssistant.Sys.IO.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   4/23/2021 2:56:39 PM
// Modified By:  james

#endregion




namespace SuperMemoAssistant.Plugins.QuickQA
{
  // ReSharper disable once UnusedMember.Global
  // ReSharper disable once ClassNeverInstantiated.Global
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
  public class QuickQAPlugin : SMAPluginBase<QuickQAPlugin>
  {
    #region Constructors

    /// <inheritdoc />
    public QuickQAPlugin() { }

    #endregion




    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "QuickQA";

    /// <inheritdoc />
    public override bool HasSettings => true;

    public QuickQACfg Config { get; private set; }


    #endregion




    #region Methods Impl

    private void LoadConfig()
    {
      Config = Svc.Configuration.Load<QuickQACfg>() ?? new QuickQACfg();
    }

    /// <inheritdoc />
    protected override void OnSMStarted(bool wasSMAlreadyStarted)
    {
      LoadConfig();
      Svc.HotKeyManager
       .RegisterGlobal(
        "CreateQAWithQuestion",
        "Create QA with selected text as question",
        HotKeyScopes.SMBrowser,
        new HotKey(Key.Q, KeyModifiers.AltShift),
        CreateQAWithQuestion
      )
       .RegisterGlobal(
        "CreateQAWithAnswer",
        "Create QA with selected text as answer",
        HotKeyScopes.SMBrowser,
        new HotKey(Key.A, KeyModifiers.AltShift),
        CreateQAWithAnswer
      );

      Application.Current.Dispatcher.Invoke(() =>
      {
        var wdw = new QuickQAWdw("", "");
        wdw.Width = 1;
        wdw.Height = 1;
        wdw.ShowAndActivate();
        wdw.Close();
      });

      base.OnSMStarted(wasSMAlreadyStarted);
    }

    private void CreateQAWithAnswer()
    {
      var selText = ContentUtils.GetSelectedText() ?? string.Empty;
      var wdw = Application.Current.Dispatcher.Invoke(() => new QuickQAWdw(string.Empty, selText));
      CreateQA(wdw);
    }

    private void CreateQA(QuickQAWdw wdw)
    {
      Application.Current.Dispatcher.BeginInvoke((Action)(() =>
      {
        wdw.ShowAndActivate();
      }));
    }

    private void CreateQAWithQuestion()
    {
      var selText = ContentUtils.GetSelectedText() ?? string.Empty;
      var wdw = Application.Current.Dispatcher.Invoke(() => new QuickQAWdw(selText, string.Empty));
      CreateQA(wdw);
    }

    public override void ShowSettings()
    {
      ConfigurationWindow.ShowAndActivate("Quick QA Settings", HotKeyManager.Instance, Config);
    }

    #endregion

    #region Methods


    #endregion
  }
}
